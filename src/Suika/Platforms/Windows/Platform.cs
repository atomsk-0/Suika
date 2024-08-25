// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika.Data;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace Suika.Platforms.Windows;

#pragma warning disable CA1416


internal static unsafe class Platform
{
    private const ushort windows_11_min_build = 22000;

    /// <summary>
    /// Get the primary monitor screen size
    /// </summary>
    /// <returns>Primary monitor screen size</returns>
    internal static Size GetScreenSize()
    {
        return new Size(GetSystemMetrics(SM.SM_CXSCREEN), GetSystemMetrics(SM.SM_CYSCREEN));
    }

    internal static bool IsWindows11()
    {
        return Environment.OSVersion.Version.Build >= windows_11_min_build;
    }

    internal static void SetWindowStyle(HWND hWnd, in AppOptions options)
    {
        if (IsWindows11())
        {
            SetWindows11Styling(hWnd, options);
        }
        else
        {
            SetWindows10Styling(hWnd);
        }
    }

    internal static void SetWindows11Styling(HWND hWnd, in AppOptions options)
    {
        // Round window corners
        uint val = (uint)DWM_WINDOW_CORNER_PREFERENCE.DWMWCP_ROUND;
        DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_WINDOW_CORNER_PREFERENCE, &val, sizeof(uint));

        if (options.UseWindows11Border == false)
        {
            //Disable window border drawing, we will draw our own border
            uint val2 = 0xFFFFFFFE;
            DwmSetWindowAttribute(hWnd, (uint)DWMWINDOWATTRIBUTE.DWMWA_BORDER_COLOR, &val2, sizeof(uint));
        }

        ImGuiStyle* style = ImGui.GetStyle();
        style->WindowBorderSize = options.UseWindows11Border ? 0.0f : 1.0f;
        style->WindowRounding = 8.0f;
        style->ItemSpacing = new Vector2(0.0f, 0.0f);
        style->Colors[(int)ImGuiCol.Border] = new Vector4(1f, 1f, 1f, 0.2f);
        style->Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    }

    internal static void SetWindows10Styling(HWND hWnd)
    {
        ImGuiStyle* style = ImGui.GetStyle();
        style->WindowBorderSize = 1.0f;
        style->WindowRounding = 0.0f;
        style->ItemSpacing = new Vector2(0.0f, 0.0f);
        style->Colors[(int)ImGuiCol.Border] = new Vector4(1f, 1f, 1f, 0.2f);
        style->Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
    }

    internal static void SimulateLeftMouseClick()
    {
        mouse_event(MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}