// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mochi.DearImGui;
using Mochi.DearImGui.Backends.Direct3D12;
using Mochi.DearImGui.Backends.Win32;
using Suika.Data;
using Suika.Types.Interfaces;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;
using D3D12_CPU_DESCRIPTOR_HANDLE = TerraFX.Interop.DirectX.D3D12_CPU_DESCRIPTOR_HANDLE;
using D3D12_GPU_DESCRIPTOR_HANDLE = TerraFX.Interop.DirectX.D3D12_GPU_DESCRIPTOR_HANDLE;
using ID3D12DescriptorHeap = TerraFX.Interop.DirectX.ID3D12DescriptorHeap;
using ID3D12Device = TerraFX.Interop.DirectX.ID3D12Device;
using ID3D12GraphicsCommandList = TerraFX.Interop.DirectX.ID3D12GraphicsCommandList;

namespace Suika.Platforms.Windows.Backends;

#pragma warning disable CA1416

// Experimental Direct3D 12 backend, not yet functional has some issues with the swap chain creation

public unsafe partial class D3D12Backend : IBackend
{
    [LibraryImport("Mochi.DearImGui.Native.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool ImGui_ImplDX12_Init(ID3D12Device* device, int numFramesInFlight, DXGI_FORMAT rtvFormat, ID3D12DescriptorHeap* cbvSrvHeap, D3D12_CPU_DESCRIPTOR_HANDLE fontSrvCpuDescHandle, D3D12_GPU_DESCRIPTOR_HANDLE fontSrvGpuDescHandle);

    [StructLayout(LayoutKind.Sequential)]
    public struct FrameContext
    {
        public ID3D12CommandAllocator* CommandAllocator;
        public ulong FenceValue;
    }

    private const int num_frames_in_flight = 3;
    private const int num_back_buffers = 3;

    private FrameContext[] frameContexts = new FrameContext[num_frames_in_flight];
    private uint frameIndex;

    private ID3D12Device* device;
    private ID3D12DescriptorHeap* rtvDescHeap;
    private ID3D12DescriptorHeap* srvDescHeap;
    private ID3D12CommandQueue* commandQueue;
    private ID3D12GraphicsCommandList* commandList;
    private ID3D12Fence* fence;
    private HANDLE fenceEvent;
    private ulong fenceLastSignaledValue;
    private IDXGISwapChain3* swapChain;
    private bool swapChainOccluded;
    private HANDLE swapChainWaitableObject;
    private ID3D12Resource** mainRenderTargetResource; // [num_back_buffers]
    private D3D12_CPU_DESCRIPTOR_HANDLE* mainRenderTargetDescriptor; // [num_back_buffers]

    private bool imguiInitialized;

    public bool Setup(IWindow windowInstance, in AppOptions options)
    {
        ID3D12Resource** tempMainRenderTargetResource = stackalloc ID3D12Resource*[num_back_buffers];
        mainRenderTargetResource = tempMainRenderTargetResource;

        D3D12_CPU_DESCRIPTOR_HANDLE* tempMainRenderTargetDescriptor = stackalloc D3D12_CPU_DESCRIPTOR_HANDLE[num_back_buffers];
        mainRenderTargetDescriptor = tempMainRenderTargetDescriptor;

        for (int i = 0; i < num_frames_in_flight; i++)
        {
            frameContexts[i] = new FrameContext();
        }

        var sd = new DXGI_SWAP_CHAIN_DESC1
        {
            BufferCount = num_back_buffers,
            Width = 20,
            Height = 20,
            Format = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM,
            Flags = (uint)DXGI_SWAP_CHAIN_FLAG.DXGI_SWAP_CHAIN_FLAG_FRAME_LATENCY_WAITABLE_OBJECT,
            BufferUsage = DXGI.DXGI_USAGE_RENDER_TARGET_OUTPUT,
            SampleDesc = new DXGI_SAMPLE_DESC(count: 1, quality: 0),
            SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_DISCARD,
            AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_UNSPECIFIED,
            Scaling = DXGI_SCALING.DXGI_SCALING_STRETCH,
            Stereo = false
        };

        D3D_FEATURE_LEVEL featureLevel = D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0;

        ID3D12Device* lDevice;
        if (DirectX.D3D12CreateDevice(null, featureLevel, __uuidof<ID3D12Device>(), (void**)&lDevice) > 0)
        {
            return false;
        }
        device = lDevice;

        {
            var desc = new D3D12_DESCRIPTOR_HEAP_DESC
            {
                Type = D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV,
                NumDescriptors = num_back_buffers,
                Flags = D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_NONE,
                NodeMask = 1
            };

            ID3D12DescriptorHeap* lRtvDescHeap;
            if (device->CreateDescriptorHeap(&desc, __uuidof<ID3D12DescriptorHeap>(), (void**)&lRtvDescHeap) > 0)
            {
                return false;
            }
            rtvDescHeap = lRtvDescHeap;

            uint rtvDescriptorSize = device->GetDescriptorHandleIncrementSize(D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_RTV);
            D3D12_CPU_DESCRIPTOR_HANDLE rtvHandle = rtvDescHeap->GetCPUDescriptorHandleForHeapStart();
            for (uint i = 0; i < num_back_buffers; i++)
            {
                mainRenderTargetDescriptor[i] = rtvHandle;
                rtvHandle.ptr += rtvDescriptorSize;
            }
        }

        {
            var desc = new D3D12_DESCRIPTOR_HEAP_DESC
            {
                Type = D3D12_DESCRIPTOR_HEAP_TYPE.D3D12_DESCRIPTOR_HEAP_TYPE_CBV_SRV_UAV,
                NumDescriptors = 1,
                Flags = D3D12_DESCRIPTOR_HEAP_FLAGS.D3D12_DESCRIPTOR_HEAP_FLAG_SHADER_VISIBLE
            };

            ID3D12DescriptorHeap* lSrvDescHeap;
            if (device->CreateDescriptorHeap(&desc, __uuidof<ID3D12DescriptorHeap>(), (void**)&lSrvDescHeap) > 0)
            {
                return false;
            }
            srvDescHeap = lSrvDescHeap;
        }

        {
            var desc = new D3D12_COMMAND_QUEUE_DESC
            {
                Type = D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT,
                Flags = D3D12_COMMAND_QUEUE_FLAGS.D3D12_COMMAND_QUEUE_FLAG_NONE,
                NodeMask = 1
            };
            ID3D12CommandQueue* lCommandQueue;
            if (device->CreateCommandQueue(&desc, __uuidof<ID3D12CommandQueue>(), (void**)&lCommandQueue) > 0)
            {
                return false;
            }
            commandQueue = lCommandQueue;
        }

        FrameContext[] lFrameContexts = frameContexts;
        for (uint i = 0; i < num_frames_in_flight; i++)
        {
            var commandAllocator = lFrameContexts[i].CommandAllocator;
            if (device->CreateCommandAllocator(D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT, __uuidof<ID3D12CommandAllocator>(), (void**)&commandAllocator) > 0)
            {
                return false;
            }
            lFrameContexts[i].CommandAllocator = commandAllocator;
        }
        frameContexts = lFrameContexts;

        ID3D12GraphicsCommandList* lCommandList;
        if (device->CreateCommandList(0, D3D12_COMMAND_LIST_TYPE.D3D12_COMMAND_LIST_TYPE_DIRECT, frameContexts[0].CommandAllocator, null, __uuidof<ID3D12GraphicsCommandList>(), (void**)&lCommandList) > 0 || lCommandList->Close() > 0)
        {
            return false;
        }

        commandList = lCommandList;

        ID3D12Fence* lFence;
        if (device->CreateFence(0, D3D12_FENCE_FLAGS.D3D12_FENCE_FLAG_NONE, __uuidof<ID3D12Fence>(), (void**)&lFence) > 0)
        {
            return false;
        }
        fence = lFence;

        fenceEvent = CreateEventW(null, BOOL.FALSE, false, null);
        if (fenceEvent == HANDLE.NULL)
        {
            return false;
        }

        {
            IDXGIFactory4* dxgiFactory = null;
            IDXGISwapChain1* swapChain1 = null;
            if (DirectX.CreateDXGIFactory1(__uuidof<IDXGIFactory4>(), (void**)&dxgiFactory) > 0)
            {
                return false;
            }


            HWND hanlde = (HWND)windowInstance.GetHandle();
            //Unsafe.AsPointer(ref Unsafe.AsRef(in swapChain1));
            ThrowIfFailed(dxgiFactory->CreateSwapChainForHwnd((IUnknown*)commandQueue, hanlde, &sd, null, null, &swapChain1)); // Broken here
            /*if (dxgiFactory->CreateSwapChainForHwnd((IUnknown*)commandQueue, hanlde, &sd, null, null, (IDXGISwapChain1**)swapChain1) > 0)
            {
                return false;
            }*/

            if (swapChain1 == null)
            {
                Console.WriteLine("Failed to create swap chain");
            }

            IDXGISwapChain3* lSwapChain;
            if (swapChain1->QueryInterface(__uuidof<IDXGISwapChain3>(), (void**)&lSwapChain) > 0)
            {
                return false;
            }
            swapChain = lSwapChain;

            swapChain1->Release();
            dxgiFactory->Release();
            swapChain->SetMaximumFrameLatency(num_back_buffers);
            swapChainWaitableObject = swapChain->GetFrameLatencyWaitableObject();
        }

        createRenderTarget();

        ImGui.CreateContext();
        Win32ImBackend.Init((void*)windowInstance.GetHandle());
        ImGui_ImplDX12_Init(device, num_frames_in_flight, DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM, srvDescHeap, srvDescHeap->GetCPUDescriptorHandleForHeapStart(), srvDescHeap->GetGPUDescriptorHandleForHeapStart());
        imguiInitialized = true;

        return true;
    }


    public void Reset()
    {
        throw new NotImplementedException();
    }


    public void Destroy()
    {
        if (imguiInitialized)
        {
            Direct3D12ImBackend.Shutdown();
            Win32ImBackend.Shutdown();
            ImGui.DestroyContext();
        }

        cleanUpRenderTarget();
        if (swapChain != null)
        {
            swapChain->SetFullscreenState(false, null);
            swapChain->Release();
            swapChain = null;
        }

        if (swapChainWaitableObject != HANDLE.NULL)
        {
            CloseHandle(swapChainWaitableObject);
        }

        for (uint i = 0; i < num_frames_in_flight; i++)
        {
            if (frameContexts[i].CommandAllocator != null)
            {
                frameContexts[i].CommandAllocator->Release();
                frameContexts[i].CommandAllocator = null;
            }
        }

        if (commandQueue != null)
        {
            commandQueue->Release();
            commandQueue = null;
        }

        if (commandList != null)
        {
            commandList->Release();
            commandList = null;
        }

        if (rtvDescHeap != null)
        {
            rtvDescHeap->Release();
            rtvDescHeap = null;
        }

        if (srvDescHeap != null)
        {
            srvDescHeap->Release();
            srvDescHeap = null;
        }

        if (fence != null)
        {
            fence->Release();
            fence = null;
        }

        if (fenceEvent != HANDLE.NULL)
        {
            CloseHandle(fenceEvent);
            fenceEvent = HANDLE.NULL;
        }

        if (device != null)
        {
            device->Release();
            device = null;
        }
    }


    public void Render(Action renderAction)
    {
        if (swapChainOccluded && swapChain->Present(0, DXGI.DXGI_PRESENT_TEST) == DXGI.DXGI_STATUS_OCCLUDED)
        {
            Thread.Sleep(10);
            return;
        }

        swapChainOccluded = false;

        Direct3D12ImBackend.NewFrame();
        Win32ImBackend.NewFrame();
        ImGui.NewFrame();

        ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.Once, Vector2.Zero);
        ImGui.SetNextWindowSize(new Vector2(500, 500), ImGuiCond.Always);
        ImGui.Begin("suika_imgui_window", null, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration);
        ImGui.End();

        ImGui.ShowDemoWindow();

        ImGui.EndFrame();

        ImGui.Render();

        FrameContext* frameContext = waitForNextFrameResources();
        uint backBufferIdx = swapChain->GetCurrentBackBufferIndex();
        frameContext->CommandAllocator->Reset();

        var barrier = new D3D12_RESOURCE_BARRIER
        {
            Type = D3D12_RESOURCE_BARRIER_TYPE.D3D12_RESOURCE_BARRIER_TYPE_TRANSITION,
            Flags = D3D12_RESOURCE_BARRIER_FLAGS.D3D12_RESOURCE_BARRIER_FLAG_NONE,
            Transition = new D3D12_RESOURCE_TRANSITION_BARRIER
            {
                pResource = mainRenderTargetResource[backBufferIdx],
                Subresource = D3D12.D3D12_RESOURCE_BARRIER_ALL_SUBRESOURCES,
                StateBefore = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT,
                StateAfter = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET
            }
        };
        commandList->Reset(frameContext->CommandAllocator, null);
        commandList->ResourceBarrier(1, &barrier);

        float[] clearColor = [0.0f, 0.0f, 0.0f, 1.0f];
        fixed (float* backColorPtr = clearColor)
        {
            commandList->ClearRenderTargetView(mainRenderTargetDescriptor[backBufferIdx], backColorPtr, 0, null);
        }
        commandList->OMSetRenderTargets(1, &mainRenderTargetDescriptor[backBufferIdx], false, null);
        var lSrvDescHeap = srvDescHeap;
        commandList->SetDescriptorHeaps(1, &lSrvDescHeap);
        srvDescHeap = lSrvDescHeap;
        barrier.Transition.StateBefore = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_RENDER_TARGET;
        barrier.Transition.StateAfter = D3D12_RESOURCE_STATES.D3D12_RESOURCE_STATE_PRESENT;
        commandList->ResourceBarrier(1, &barrier);
        commandList->Close();

        var lCommandList = commandList;
        commandQueue->ExecuteCommandLists(1, (ID3D12CommandList**)&lCommandList);
        commandList = lCommandList;

        HRESULT hr = swapChain->Present(1, 0);
        swapChainOccluded = hr == DXGI.DXGI_STATUS_OCCLUDED;

        ulong fenceValue = fenceLastSignaledValue++;
        commandQueue->Signal(fence, fenceValue);
        fenceLastSignaledValue = fenceValue;
        frameContext->FenceValue = fenceValue;
    }


    public IntPtr LoadTextureFromFile(string path)
    {
        throw new NotImplementedException();
    }


    public IntPtr LoadTextureFromMemory(Stream stream)
    {
        throw new NotImplementedException();
    }


    private void createRenderTarget()
    {
        for (uint i = 0; i < num_back_buffers; i++)
        {
            ID3D12Resource* backBuffer = null;
            swapChain->GetBuffer(i, __uuidof<ID3D12Resource>(), (void**)&backBuffer);
            device->CreateRenderTargetView(backBuffer, null, mainRenderTargetDescriptor[i]);
            mainRenderTargetResource[i] = backBuffer;
        }
    }


    private void cleanUpRenderTarget()
    {
        waitForLastSubmittedFrame();

        for (uint i = 0; i < num_back_buffers; i++)
        {
            if (mainRenderTargetResource[i] != null)
            {
                mainRenderTargetResource[i]->Release();
                mainRenderTargetResource[i] = null;
            }
        }
    }


    private void waitForLastSubmittedFrame()
    {
        fixed (FrameContext* pFrameContexts = frameContexts)
        {
            FrameContext* frameContext = &pFrameContexts[frameIndex % num_frames_in_flight];

            ulong fenceValue = frameContext->FenceValue;
            if (fenceValue == 0) return;

            frameContext->FenceValue = 0;
            if (fence->GetCompletedValue() >= fenceValue) return;

            fence->SetEventOnCompletion(fenceValue, fenceEvent);
            WaitForSingleObject(fenceEvent, INFINITE);
        }
    }

    private FrameContext* waitForNextFrameResources()
    {
        uint nextFrameIndex = frameIndex++;
        frameIndex = nextFrameIndex;

        HANDLE[] waitableObjects = [swapChainWaitableObject, HANDLE.NULL];
        uint numWaitableObjects = 1;

        fixed (FrameContext* pFrameContexts = frameContexts)
        {
            FrameContext* frameContext = &pFrameContexts[nextFrameIndex % num_frames_in_flight];
            ulong fenceValue = pFrameContexts->FenceValue;
            if (fenceValue != 0)
            {
                frameContext->FenceValue = 0;
                fence->SetEventOnCompletion(fenceValue, fenceEvent);
                waitableObjects[1] = fenceEvent;
                numWaitableObjects = 2;
            }

            fixed (HANDLE* pWaitableObjects = waitableObjects)
            {
                WaitForMultipleObjects(numWaitableObjects, pWaitableObjects, true, INFINITE);
            }

            return frameContext;
        }
    }
}