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
    private const string segoe_icons_font = "C:\\Windows\\Fonts\\SegoeIcons.ttf";
    private const string segmdl12_font = "C:\\Windows\\Fonts\\segmdl2.ttf";
    internal const string MINIMIZE_ICON = "\uE921";
    internal const string MAXIMIZE_ICON = "\uE922";
    internal const string RESTORE_ICON = "\uE923";
    internal const string CLOSE_ICON = "\uE8BB";
    internal const byte MX_PADDING = 8;

    internal static ImFont* SystemFont;

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
            SetWindows11Style(hWnd, options);
        }
        else
        {
            SetWindows10Style();
        }
    }

    internal static void SetWindows11Style(HWND hWnd, in AppOptions options)
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

        ImGuiIO* io = ImGui.GetIO();
        io->Fonts->AddFontDefault();

        ImGuiStyle* style = ImGui.GetStyle();
        style->WindowBorderSize = options.UseWindows11Border ? 0.0f : 1.0f;
        style->WindowRounding = 8.0f;
        style->ItemSpacing = new Vector2(0.0f, 0.0f);
        style->Colors[(int)ImGuiCol.Border] = new Vector4(1f, 1f, 1f, 0.2f);
        style->Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        // Load SegoeIcons font (Windows 11 icons)
        if (File.Exists(segoe_icons_font))
        {
            char[] iconRanges = ['\uE001', '\uF8CC', '\0'];
            var iconsConfig = new ImFontConfig
            {
                MergeMode = false,
                PixelSnapH = true,
                GlyphMinAdvanceX = 10f
            };

            fixed (char* iconRangesPtr = iconRanges)
            {
                SystemFont = io->Fonts->AddFontFromFileTTF(segoe_icons_font, 10f, &iconsConfig, iconRangesPtr);
            }
        }
        else if (File.Exists(segmdl12_font))
        {
            // Fallback to segmdl2 font
            char[] iconRanges = ['\uE001', '\uF8B3', '\0'];
            var iconCOnfig = new ImFontConfig
            {
                MergeMode = false,
                PixelSnapH = true,
                GlyphMinAdvanceX = 10f
            };

            fixed (char* iconRangesPtr = iconRanges)
            {
                SystemFont = io->Fonts->AddFontFromFileTTF(segmdl12_font, 10f, &iconCOnfig, iconRangesPtr);
            }
        }
    }

    internal static void SetWindows10Style()
    {
        ImGuiIO* io = ImGui.GetIO();
        io->Fonts->AddFontDefault();

        ImGuiStyle* style = ImGui.GetStyle();
        style->WindowBorderSize = 1.0f;
        style->WindowRounding = 0.0f;
        style->ItemSpacing = new Vector2(0.0f, 0.0f);
        style->Colors[(int)ImGuiCol.Border] = new Vector4(1f, 1f, 1f, 0.2f);
        style->Colors[(int)ImGuiCol.WindowBg] = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);

        if (File.Exists(segmdl12_font))
        {
            char[] iconRanges = ['\uE001', '\uF8B3', '\0'];
            var iconCOnfig = new ImFontConfig
            {
                MergeMode = false,
                PixelSnapH = true,
                GlyphMinAdvanceX = 10f
            };

            fixed (char* iconRangesPtr = iconRanges)
            {
                SystemFont = io->Fonts->AddFontFromFileTTF(segmdl12_font, 10f, &iconCOnfig, iconRangesPtr);
            }
        }
    }

    internal static void SimulateLeftMouseClick()
    {
        mouse_event(MOUSEEVENTF.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        mouse_event(MOUSEEVENTF.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}