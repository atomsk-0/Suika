// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Runtime.InteropServices;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Platforms.Windows.Backends;
using Suika.Types.Enums;
using Suika.Types.Interfaces;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

#pragma warning disable CA1416

namespace Suika.Platforms.Windows;

public unsafe class Window : IWindow
{
    private const string class_name = "Suika::Window";

    private WNDCLASSEXW wndclass;
    private HWND handle;

    private AppOptions options;
    private IBackend backend = null!;

    private bool running = true;

    public void Create(in AppOptions appOptions)
    {
        options = appOptions;

        backend = options.RenderBackend switch
        {
            RenderBackend.DirectX9 => new D3D9Backend(),
            _ => throw new ArgumentOutOfRangeException()
        };

        fixed (char* classNamePtr = class_name)
        {
            var lWndc = new WNDCLASSEXW
            {
                cbSize = (uint)sizeof(WNDCLASSEXW),
                style = CS.CS_CLASSDC,
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
                handle = CreateWindowExW(0, classNamePtr, titlePtr, WS.WS_POPUP, 0, 0, options.Width, options.Height, HWND.NULL, HMENU.NULL, wndclass.hInstance, null);
            }
        }

        if (backend.Setup(handle) == false)
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

    [UnmanagedCallersOnly]
    private static LRESULT winProc(HWND window, uint message, WPARAM wParam, LPARAM lParam)
    {
        if (ImGui.ImGui_ImplWin32_WndProcHandler((int*)window, message, wParam, lParam) > 0) return 1;
        switch (message)
        {
            case WM.WM_SIZE:
            {
                if (wParam == SIZE_MINIMIZED) return 0;
                return 0;
            }
            case WM.WM_SYSCOMMAND:
            {
                if ((wParam & 0xFFF0) == SC.SC_KEYMENU) return 0;
                break;
            }
            case WM.WM_DESTROY:
            {
                PostQuitMessage(0);
                return 0;
            }
        }
        return DefWindowProcW(window, message, wParam, lParam);
    }
}