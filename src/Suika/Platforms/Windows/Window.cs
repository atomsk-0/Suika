using System.Runtime.InteropServices;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Types.Interfaces;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.D3DPRESENT;
using static TerraFX.Interop.Windows.CS;
using static TerraFX.Interop.Windows.PM;
using static TerraFX.Interop.Windows.SC;
using static TerraFX.Interop.Windows.SW;
using static TerraFX.Interop.Windows.Windows;
using static TerraFX.Interop.Windows.WM;
using static TerraFX.Interop.Windows.WS;
using Exception = System.Exception;
using IDirect3DDevice9 = TerraFX.Interop.DirectX.IDirect3DDevice9;

namespace Suika.Platforms.Windows;

#pragma warning disable CA1416

public unsafe class Window : IWindow
{
    private const string class_name = "Suika::Window";

    private WNDCLASSEXW wndclass;
    private HWND handle;

    private IDirect3D9* d3d9;
    private D3DPRESENT_PARAMETERS d3dpp;
    private IDirect3DDevice9* device;
    private bool deviceLost;

    private WindowOptions windowOptions;

    private static uint resizeWidth, resizeHeight;

    public void Create(in WindowOptions options)
    {
        windowOptions = options;

        fixed (char* classNamePtr = class_name)
        {
            var lWndc = new WNDCLASSEXW
            {
                cbSize = (uint)sizeof(WNDCLASSEXW),
                style = CS_CLASSDC,
                lpfnWndProc = &winProc,
                hInstance = GetModuleHandleW(null),
                hCursor = HCURSOR.NULL,
                hbrBackground = HBRUSH.NULL,
                lpszClassName = classNamePtr
            };
            RegisterClassExW(&lWndc);
            wndclass = lWndc;

            fixed (char* titlePtr = options.Title)
            {
                handle = CreateWindowExW(0, classNamePtr, titlePtr, WS_POPUP, CW_USEDEFAULT, CW_USEDEFAULT, options.Size.Width, options.Size.Height, HWND.NULL, HMENU.NULL, wndclass.hInstance, null);
            }
        }

        if (createDeviceD3D9() == false)
        {
            throw new Exception("Failed to create Direct3D9 device"); //TODO: Create custom exception's
        }

        ShowWindow(handle, SW_SHOWDEFAULT);
        UpdateWindow(handle);
    }


    public void SetupImGui()
    {
        ImGui.CreateContext();
        ImGuiIO* io = ImGui.GetIO();

        io->ConfigFlags |= ImGuiConfigFlags.NavEnableKeyboard;

        ImGui.ImGui_ImplWin32_Init(handle);
        ImGui.ImGui_ImplDX9_Init((Mochi.DearImGui.IDirect3DDevice9*)device);
    }


    public void Render()
    {
        bool done = false;
        while (done == false)
        {
            MSG msg;
            while (PeekMessageW(&msg, HWND.NULL, 0, 0, PM_REMOVE))
            {
                TranslateMessage(&msg);
                DispatchMessageW(&msg);
                if (msg.message == WM_QUIT) done = true;
            }

            if (done) break;

            if (deviceLost)
            {
                HRESULT hr = device->TestCooperativeLevel();
                if (hr == D3DERR.D3DERR_DEVICELOST)
                {
                    Thread.Sleep(10);
                    continue;
                }

                if (hr == D3DERR.D3DERR_DEVICENOTRESET)
                {
                    resetDevice();
                }

                deviceLost = false;
            }

            if (resizeWidth != 0 && resizeHeight != 0)
            {
                d3dpp.BackBufferWidth = resizeWidth;
                d3dpp.BackBufferHeight = resizeHeight;
                resizeWidth = resizeHeight = 0;
                resetDevice();
            }

            ImGui.ImGui_ImplDX9_NewFrame();
            ImGui.ImGui_ImplWin32_NewFrame();
            ImGui.NewFrame();

            ImGui.ShowDemoWindow();

            ImGui.EndFrame();

            device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ZENABLE, 0);
            device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_ALPHABLENDENABLE, 0);
            device->SetRenderState(D3DRENDERSTATETYPE.D3DRS_SCISSORTESTENABLE, 0);
            device->Clear(0, null, D3DCLEAR.D3DCLEAR_TARGET | D3DCLEAR.D3DCLEAR_ZBUFFER, 0x00000000, 1.0f, 0);

            if (device->BeginScene() >= 0)
            {
                ImGui.Render();
                ImGui.ImGui_ImplDX9_RenderDrawData(ImGui.GetDrawData());
                device->EndScene();
            }

            HRESULT result = device->Present(null, null, HWND.NULL, null);
            if (result == D3DERR.D3DERR_DEVICELOST)
            {
                deviceLost = true;
            }
        }
    }


    public void Destroy()
    {
        ImGui.ImGui_ImplDX9_Shutdown();
        ImGui.ImGui_ImplWin32_Shutdown();
        ImGui.DestroyContext();

        cleanupDeviceD3D9();
        DestroyWindow(handle);
        UnregisterClassW(wndclass.lpszClassName, wndclass.hInstance);
    }


    public void Normalize()
    {
        ShowWindowAsync(handle, SW_NORMAL);
    }

    public void Maximize()
    {
        ShowWindowAsync(handle, SW_MAXIMIZE);
    }

    public void Minimize()
    {
        ShowWindowAsync(handle, SW_MINIMIZE);
    }


    public void Activate()
    {
        SetActiveWindow(handle);
        SetFocus(handle);
        SetForegroundWindow(handle);
    }


    public void Show()
    {
        ShowWindowAsync(handle, SW_SHOWDEFAULT);
    }


    public void Hide()
    {
        ShowWindowAsync(handle, SW_HIDE);
    }


    public void SetTitle(string title)
    {
        fixed(char* titlePtr = title)
        {
            SetWindowTextW(handle, titlePtr);
        }
    }


    /* DirectX 9 Stuff */
    private bool createDeviceD3D9()
    {
        if ((d3d9 = DirectX.Direct3DCreate9(D3D.D3D_SDK_VERSION)) == null) return false;

        var lD3dpp = new D3DPRESENT_PARAMETERS
        {
            Windowed = true,
            SwapEffect = D3DSWAPEFFECT.D3DSWAPEFFECT_DISCARD,
            BackBufferFormat = D3DFORMAT.D3DFMT_UNKNOWN,
            EnableAutoDepthStencil = true,
            AutoDepthStencilFormat = D3DFORMAT.D3DFMT_D16,
            PresentationInterval = windowOptions.VSync ? D3DPRESENT_INTERVAL_ONE : D3DPRESENT_INTERVAL_IMMEDIATE
        };

        IDirect3DDevice9* lDevice = null;

        if (d3d9->CreateDevice(DirectX.D3DADAPTER_DEFAULT, D3DDEVTYPE.D3DDEVTYPE_HAL, handle, D3DCREATE.D3DCREATE_HARDWARE_VERTEXPROCESSING, &lD3dpp, &lDevice) < 0) return false;

        d3dpp = lD3dpp;
        device = lDevice;
        return true;
    }

    private void cleanupDeviceD3D9()
    {
        if (device is not null)
        {
            device->Release();
            device = null;
        }
        if (d3d9 is not null)
        {
            d3d9->Release();
            d3d9 = null;
        }
    }

    private void resetDevice()
    {
        var lD3dpp = d3dpp;
        device->Reset(&lD3dpp);
        d3dpp = lD3dpp;
    }

    [UnmanagedCallersOnly]
    private static LRESULT winProc(HWND window, uint message, WPARAM wParam, LPARAM lParam)
    {
        if (ImGui.ImGui_ImplWin32_WndProcHandler((int*)window, message, wParam, lParam) > 0) return 1;
        switch (message)
        {
            case WM_SIZE:
            {
                if (wParam == SIZE_MINIMIZED) return 0;
                resizeWidth = LOWORD(lParam);
                resizeHeight = HIWORD(lParam);
                return 0;
            }
            case WM_SYSCOMMAND:
            {
                if ((wParam & 0xFFF0) == SC_KEYMENU) return 0;
                break;
            }
            case WM_DESTROY:
            {
                PostQuitMessage(0);
                return 0;
            }
        }
        return DefWindowProcW(window, message, wParam, lParam);
    }
}