// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.InteropServices;
using Suika.Data;
using Suika.Platforms.Windows.Backends;
using Suika.Types.Enums;
using Suika.Types.Interfaces;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416

namespace Suika.Platforms.Windows;

public unsafe partial class Window : IWindow
{
    [LibraryImport("Mochi.DearImGui.Native.dll")]
    private static partial LRESULT ImGui_ImplWin32_WndProcHandler(HWND hWnd, uint msg, WPARAM wParam, LPARAM lParam);

    private const string class_name = "Suika::Window";
    private const byte loop_timer_id = 1;

    private WNDCLASSEXW wndclass;
    private HWND handle;

    private AppOptions options;
    private IBackend backend = null!;

    private bool running = true;

    private delegate LRESULT WndProcDelegate(HWND window, uint msg, WPARAM wParam, LPARAM lParam);
    private WndProcDelegate wndProcDelegate = null!;

    public void Create(in AppOptions appOptions)
    {
        options = appOptions;

        backend = options.RenderBackend switch
        {
            RenderBackend.DirectX9 => new D3D9Backend(),
            RenderBackend.DirectX12 => new D3D12Backend(),
            RenderBackend.OpenGl => new Win32OpenGl(),
            _ => throw new ArgumentOutOfRangeException()
        };

        wndProcDelegate = winProc;

        int x = options.StartPos.X;
        int y = options.StartPos.Y;
        if (x == -1 && y == -1)
        {
            var screenSize = Platform.GetScreenSize();
            x = (screenSize.Width - options.WindowSize.Width) / 2;
            y = (screenSize.Height - options.WindowSize.Height) / 2;
        }

        fixed (char* classNamePtr = class_name)
        {
            var lWndc = new WNDCLASSEXW
            {
                cbSize = (uint)sizeof(WNDCLASSEXW),
                style = CS.CS_CLASSDC,
                lpfnWndProc = (delegate* unmanaged<HWND, uint, WPARAM, LPARAM, LRESULT>)Marshal.GetFunctionPointerForDelegate(wndProcDelegate),
                hInstance = GetModuleHandleW(null),
                hCursor = HCURSOR.NULL,
                hbrBackground = HBRUSH.NULL,
                lpszClassName = classNamePtr
            };
            RegisterClassExW(&lWndc);
            wndclass = lWndc;

            fixed (char* titlePtr = options.Title)
            {
                handle = CreateWindowExW(0, classNamePtr, titlePtr, WS.WS_OVERLAPPEDWINDOW, x, y, options.WindowSize.Width, options.WindowSize.Height, HWND.NULL, HMENU.NULL, wndclass.hInstance, null);
            }
        }

        if (backend.Setup(this, options) == false)
        {
            throw new Exception("Failed to setup renderApi"); //TODO: Create custom exception's
        }

        ShowWindow(handle, SW.SW_SHOWDEFAULT);
        UpdateWindow(handle);
    }

    public void Render()
    {
        while (running)
        {
            MSG msg;
            while (PeekMessageW(&msg, HWND.NULL, 0, 0, PM.PM_REMOVE))
            {
                TranslateMessage(&msg);
                DispatchMessageW(&msg);
                if (msg.message == WM.WM_QUIT) running = false;
            }
            if (running == false) break;
            backend.Render(() => {});
        }
    }

    public void Destroy()
    {
        backend.Destroy();
        DestroyWindow(handle);
        UnregisterClassW(wndclass.lpszClassName, wndclass.hInstance);
    }


    public nint GetHandle()
    {
        return handle;
    }

    public Action<int, int>? OnResize { get; set; }

    private LRESULT winProc(HWND window, uint msg, WPARAM wParam, LPARAM lParam)
    {
        if (ImGui_ImplWin32_WndProcHandler(window, msg, wParam, lParam) > 0) return 1;
        switch (msg)
        {
            case WM.WM_SIZE:
            {
                if (wParam == SIZE_MINIMIZED) return 0;
                OnResize?.Invoke(LOWORD(lParam), HIWORD(lParam));
                return 0;
            }
            case WM.WM_SYSCOMMAND:
            {
                if ((wParam & 0xFFF0) == SC.SC_KEYMENU) return 0;
                break;
            }
            case WM.WM_ENTERSIZEMOVE | WM.WM_ENTERMENULOOP:
            {
                nuint ret = SetTimer(handle, loop_timer_id, USER_TIMER_MINIMUM, null);
                if (ret == 0)
                {
                    throw new Exception("Failed to set timer");
                }
                return 0;
            }
            case WM.WM_EXITSIZEMOVE | WM.WM_EXITMENULOOP:
            {
                KillTimer(handle, loop_timer_id);
                return 0;
            }
            case WM.WM_TIMER:
            {
                if (wParam == loop_timer_id)
                {
                    backend.Render(() => {});
                    return 0;
                }
                return 0;
            }
            case WM.WM_DESTROY:
            {
                PostQuitMessage(0);
                return 0;
            }
        }
        return DefWindowProcW(window, msg, wParam, lParam);
    }
}