﻿// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Mochi.DearImGui;
using Suika.Types.Interfaces;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using IDirect3DDevice9 = TerraFX.Interop.DirectX.IDirect3DDevice9;

namespace Suika.Platforms.Windows.Backends;

#pragma warning disable CA1416


public unsafe class D3D9Backend : IBackend
{
    private IDirect3D9* d3d9;
    private D3DPRESENT_PARAMETERS d3dpp;
    private IDirect3DDevice9* device;

    private bool deviceLost;
    private bool imGuiInitialized;

    private IWindow window = null!;
    private int backendWidth, backendHeight;
    private Vector2 imguiWindowSize;

    public bool Setup(IWindow windowInstance)
    {
        if ((d3d9 = DirectX.Direct3DCreate9(D3D.D3D_SDK_VERSION)) == null) return false;

        window = windowInstance;
        nint windowHandle = window.GetHandle();

        window.OnResize = onWindowResize;

        var lD3dpp = new D3DPRESENT_PARAMETERS
        {
            Windowed = true,
            SwapEffect = D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
            BackBufferFormat = D3DFORMAT.D3DFMT_UNKNOWN,
            EnableAutoDepthStencil = true,
            AutoDepthStencilFormat = D3DFORMAT.D3DFMT_D16,
            PresentationInterval = D3DPRESENT.D3DPRESENT_INTERVAL_ONE
        };
        IDirect3DDevice9* lDevice = null;

        if (d3d9->CreateDevice(DirectX.D3DADAPTER_DEFAULT, D3DDEVTYPE.D3DDEVTYPE_HAL, (HWND)windowHandle, D3DCREATE.D3DCREATE_HARDWARE_VERTEXPROCESSING, &lD3dpp, &lDevice) < 0) return false;

        d3dpp = lD3dpp;
        device = lDevice;

        ImGui.CreateContext();
        ImGui.ImGui_ImplWin32_Init((void*)windowHandle);
        ImGui.ImGui_ImplDX9_Init((Mochi.DearImGui.IDirect3DDevice9*)device);
        imGuiInitialized = true;

        return true;
    }

    public void Reset()
    {
        Console.WriteLine("Resetting device");
        ImGui.ImGui_ImplDX9_InvalidateDeviceObjects();
        var ld3dpp = d3dpp;
        HRESULT hr = device->Reset(&ld3dpp);
        d3dpp = ld3dpp;
        if (hr == D3DERR.D3DERR_INVALIDCALL)
        {
            throw new Exception("Failed to reset device");
        }
        ImGui.ImGui_ImplDX9_CreateDeviceObjects();
    }

    public void Destroy()
    {
        if (imGuiInitialized)
        {
            ImGui.DestroyContext();
            ImGui.ImGui_ImplDX9_Shutdown();
            ImGui.ImGui_ImplWin32_Shutdown();
        }
        if (device != null)
        {
            device->Release();
            device = null;
        }
        if (d3d9 != null)
        {
            d3d9->Release();
            d3d9 = null;
        }
    }
    public void Render(Action renderAction)
    {
        if (deviceLost)
        {
            HRESULT hr = device->TestCooperativeLevel();
            if (hr == D3DERR.D3DERR_DEVICELOST)
            {
                Thread.Sleep(10);
                return;
            }
            if (hr == D3DERR.D3DERR_DEVICENOTRESET) Reset();
            deviceLost = false;
        }

        if (backendWidth != 0 && backendHeight != 0)
        {
            d3dpp.BackBufferWidth = (uint)backendWidth;
            d3dpp.BackBufferHeight = (uint)backendHeight;
            imguiWindowSize = new Vector2(backendWidth, backendHeight);
            backendHeight = backendWidth = 0;
            Console.WriteLine("Resized");
            Reset();
        }

        // Frame Render
        ImGui.ImGui_ImplDX9_NewFrame();
        ImGui.ImGui_ImplWin32_NewFrame();
        ImGui.NewFrame();

        ImGui.SetNextWindowPos(Vector2.Zero, ImGuiCond.Once, Vector2.Zero);
        ImGui.SetNextWindowSize(imguiWindowSize, ImGuiCond.Always);
        ImGui.Begin("suika_imgui_window", null, ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoDecoration);
        ImGui.End();

        ImGui.EndFrame();

        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ZENABLE, 0);
        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ALPHABLENDENABLE, 0);
        device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_SCISSORTESTENABLE, 0);
        device->Clear(0, null, D3DCLEAR.D3DCLEAR_TARGET | D3DCLEAR.D3DCLEAR_ZBUFFER, 0, 1.0f, 0);

        if (device->BeginScene() >= 0)
        {
            ImGui.Render();
            ImGui.ImGui_ImplDX9_RenderDrawData(ImGui.GetDrawData());
            device->EndScene();
        }

        HRESULT result = device->Present(null, null, HWND.NULL, null);
        if (result == D3DERR.D3DERR_DEVICELOST) deviceLost = true;
    }


    public nint LoadImageFromFile(string path)
    {
        throw new NotImplementedException();
    }

    public nint LoadImageFromMemory(Stream stream)
    {
        throw new NotImplementedException();
    }

    private void onWindowResize(int width, int height)
    {
        backendWidth = width;
        backendHeight = height;
    }
}