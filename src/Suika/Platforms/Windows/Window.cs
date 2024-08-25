// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using System.Runtime.InteropServices;
using Mochi.DearImGui;
using Suika.Components.Internal;
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
    private const byte border_width = 8;

    private WNDCLASSEXW wndclass;
    private HWND handle;

    private AppOptions options;
    private IBackend backend = null!;

    private bool running = true;

    private delegate LRESULT WndProcDelegate(HWND window, uint msg, WPARAM wParam, LPARAM lParam);
    private WndProcDelegate wndProcDelegate = null!;

    private readonly Action internalViewAction;

    public Window()
    {
        internalViewAction = internalView;
    }

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
                handle = CreateWindowExW(0, classNamePtr, titlePtr, options.AllowResize ? WS.WS_OVERLAPPEDWINDOW : WS.WS_POPUP, x, y, options.WindowSize.Width, options.WindowSize.Height, HWND.NULL, HMENU.NULL, wndclass.hInstance, null);
            }
        }

        if (backend.Setup(this, options) == false)
        {
            throw new Exception("Failed to setup renderApi"); //TODO: Create custom exception's
        }

        ShowWindow(handle, SW.SW_SHOWDEFAULT);
        UpdateWindow(handle);

        Platform.SetWindowStyle(handle, options);
        TitleBar.SetWindow(this);

        OnResize?.Invoke(options.WindowSize.Width, options.WindowSize.Height);
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
            backend.Render(internalViewAction);
        }
    }


    private void internalView()
    {
        TitleBar.WindowsTitleBar();
        ImGui.SetCursorPosY(GetTitleBarHeight());
        View?.Invoke();
    }

    public void Destroy()
    {
        backend.Destroy();
        DestroyWindow(handle);
        UnregisterClassW(wndclass.lpszClassName, wndclass.hInstance);
    }


    public void Maximize()
    {
        ShowWindowAsync(handle, SW.SW_MAXIMIZE);
    }


    public void Restore()
    {
        ShowWindowAsync(handle, SW.SW_RESTORE);
    }


    public void Minimize()
    {
        ShowWindowAsync(handle, SW.SW_MINIMIZE);
    }


    public void Show()
    {
        ShowWindowAsync(handle, SW.SW_SHOWDEFAULT);
    }


    public void Hide()
    {
        ShowWindowAsync(handle, SW.SW_HIDE);
    }


    public float GetTitleBarPadding()
    {
        return GetSystemMetricsForDpi(SM.SM_CXPADDEDBORDER, USER_DEFAULT_SCREEN_DPI);
    }


    public float GetTitleBarTopOffset()
    {
        return IsMaximized() ? GetTitleBarPadding() * 2 : 0.0f;
    }


    public float GetTitleBarHeight()
    {
        /*int titleBarHeight = GetSystemMetricsForDpi(SM_CYCAPTION, USER_DEFAULT_SCREEN_DPI);
        return titleBarHeight;*/
        return 32.0f + GetTitleBarTopOffset();
    }


    public float GetCaptionButtonWidth()
    {
        return 36.0f;
    }


    public RectF GetTitleBarRect()
    {
        float height = GetTitleBarHeight();
        RECT rect = default;
        GetClientRect(handle, &rect);
        rect.bottom = rect.top + (int)height;
        return new RectF(rect.left, rect.right, rect.top, rect.bottom);
    }


    public Vector2 GetViewSize()
    {
        RECT rect = default;
        GetClientRect(handle, &rect);
        return new Vector2(rect.right - rect.left, rect.bottom - rect.top);
    }


    public void Activate()
    {
        SetActiveWindow(handle);
        SetFocus(handle);
        SetForegroundWindow(handle);
    }


    public void DragWindow()
    {
        ReleaseCapture();
        SendMessageW(handle, WM.WM_NCLBUTTONDOWN, HTCAPTION, 0);
    }


    public bool IsMaximized()
    {
        return false; // TODO: Implement
    }


    public nint GetHandle()
    {
        return handle;
    }

    public Action<int, int>? OnResize { get; set; }
    public Action? View { get; set; }


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
            case WM.WM_GETMINMAXINFO:
            {
                MINMAXINFO* minmax = (MINMAXINFO*)lParam;
                minmax->ptMinTrackSize.x = options.MinSize.Width;
                minmax->ptMinTrackSize.y = options.MinSize.Height;
                return 0;
            }
            case WM.WM_NCCALCSIZE:
            {
                return 0;
            }
            case WM.WM_SYSCOMMAND:
            {
                if (wParam == SC.SC_MOVE || wParam == SC.SC_SIZE)
                {
                    nint style = GetWindowLongPtrW(handle, GWL.GWL_STYLE);
                    SetWindowLongPtrW(handle, GWL.GWL_STYLE, style | WS.WS_CAPTION);
                    DefWindowProcW(handle, msg, wParam, lParam);
                    SetWindowLongPtrW(handle, GWL.GWL_STYLE, style);
                    return 0;
                }
                if ((wParam & 0xFFF0) == SC.SC_KEYMENU) return 0; // Disable ALT application menu
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
                    backend.Render(internalViewAction);
                    return 0;
                }
                return 0;
            }
            case WM.WM_NCHITTEST:
            {
                POINT point = new POINT(GET_X_LPARAM(lParam), GET_Y_LPARAM(lParam));
                if (options.AllowResize)
                {
                    RECT rc;
                    GetWindowRect(handle, &rc);
                    if (point.y >= rc.top && point.y < rc.top + border_width) {
                        if (point.x >= rc.left && point.x < rc.left + border_width) {
                            return HTTOPLEFT;
                        }
                        if (point.x >= rc.right - border_width && point.x < rc.right) {
                            return HTTOPRIGHT;
                        }
                        return HTTOP;
                    }

                    if (point.y >= rc.bottom - border_width && point.y < rc.bottom) {
                        if (point.x >= rc.left && point.x < rc.left + border_width) {
                            return HTBOTTOMLEFT;
                        }
                        if (point.x >= rc.right - border_width && point.x < rc.right) {
                            return HTBOTTOMRIGHT;
                        }
                        return HTBOTTOM;
                    }

                    if (point.x >= rc.left && point.x < rc.left + border_width) {
                        return HTLEFT;
                    }
                    if (point.x >= rc.right - border_width && point.x < rc.right) {
                        return HTRIGHT;
                    }
                }
                return HTCLIENT;
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