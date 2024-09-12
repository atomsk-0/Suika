// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Platforms.Windows;
using Suika.Types.Interfaces;
using Suika.Util;

namespace Suika.Components.Internal;

internal static unsafe class InternalTitleBar
{
    internal static bool IsDragging;
    internal static bool TrackLock;
    private static IWindow platformWindow = null!;

    internal static void WindowsTitleBar(in Color backgroundColor)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems)
        {
            return;
        }

        var platformTitleBarRect = platformWindow.GetTitleBarRect();
        ImRect titleBarRect = new ImRect(platformTitleBarRect.Left, platformTitleBarRect.Top, platformTitleBarRect.Right, platformTitleBarRect.Bottom);
        window->DrawList->AddRectFilled(titleBarRect.Min, titleBarRect.Max, backgroundColor.ToUint32Color());

        uint titleBarId = window->GetID("title_bar");
        ImGui.SetNextItemAllowOverlap();
        ImGuiInternal.ItemSize(titleBarRect, 0);
        if (ImGuiInternal.ItemAdd(titleBarRect, titleBarId) == false)
        {
            return;
        }

        bool hovered, held;
        ImGuiInternal.ButtonBehavior(titleBarRect, titleBarId, &hovered, &held);
        if (hovered && held && IsDragging == false)
        {
            IsDragging = true;
        }
        else if (held == false)
        {
            IsDragging = false;
        }

        float captionButtonWidth = platformWindow.GetCaptionButtonWidth();
        if (platformWindow.IsMaximized())
        {
            captionButtonWidth += Platform.MX_PADDING;
        }

        ImGui.SetCursorPos(new Vector2(window->Size.X - captionButtonWidth, 0));
        if (windowsTitlebarButton(Platform.CLOSE_ICON))
        {
            platformWindow.Close();
        }
        ImGui.SetCursorPos(new Vector2(window->Size.X - captionButtonWidth * 2, 0));
        if (platformWindow.IsMaximized())
        {
            if (windowsTitlebarButton(Platform.RESTORE_ICON, !platformWindow.CanResize()))
            {
                platformWindow.Restore();
            }
        }
        else
        {
            if (windowsTitlebarButton(Platform.MAXIMIZE_ICON, !platformWindow.CanResize()))
            {
                platformWindow.Maximize();
            }
        }
        ImGui.SetCursorPos(new Vector2(window->Size.X - captionButtonWidth * 3, 0));
        if (windowsTitlebarButton(Platform.MINIMIZE_ICON))
        {
            platformWindow.Minimize();
        }

        ImGui.SetCursorPos(new Vector2(50, 50));
        ImGui.BeginGroup();
        ImGui.Text(platformWindow.GetCaptionButtonWidth().ToString());
        ImGui.Text(platformWindow.GetTitleBarHeight().ToString());
        ImGui.Text(platformWindow.GetTitleBarPadding().ToString());
        ImGui.Text(platformWindow.GetTitleBarTopOffset().ToString());
        ImGui.EndGroup();
    }


    private static bool windowsTitlebarButton(string icon, bool disabled = false)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return false;

        ImGuiContext* g = *ImGuiInternal.GImGui;
        ImGuiStyle style = g->Style;
        Vector2 pos = window->DC.CursorPos;
        Vector2 size = new Vector2(platformWindow.GetCaptionButtonWidth(), platformWindow.GetTitleBarHeight());
        uint id = window->GetID($"caption_button_{icon}");

        ImRect rect = new ImRect(pos, pos + size);
        ImGuiInternal.ItemSize(rect, style.FramePadding.Y);
        if (ImGuiInternal.ItemAdd(rect, id) == false) return false;

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, id, &hovered, &held);

        if (disabled)
        {
            hovered = held = pressed = false;
        }

        if (hovered)
        {
            window->DrawList->AddRectFilled(rect.Min, rect.Max, icon == Platforms.Windows.Platform.CLOSE_ICON ? Color.FromArgb(196, 43, 28).ToUint32Color() : Color.White.ToUint32Color(25));
            window->DrawList->AddText(Platforms.Windows.Platform.SystemFont, 10f, rect.Min + new Vector2((platformWindow.GetCaptionButtonWidth() - 10) / 2, (platformWindow.GetTitleBarHeight() - (platformWindow.IsMaximized() ? 0 :  10f)) / 2), Color.White.ToUint32Color(), icon);
        }
        else
        {
            window->DrawList->AddText(Platforms.Windows.Platform.SystemFont, 10f, rect.Min + new Vector2((platformWindow.GetCaptionButtonWidth() - 10) / 2, (platformWindow.GetTitleBarHeight() - (platformWindow.IsMaximized() ? 0 :  10f)) / 2), disabled ? Color.White.ToUint32Color(125) : Color.White.ToUint32Color(), icon);
        }

        return pressed;
    }

    internal static void SetWindow(IWindow window)
    {
        platformWindow = window;
    }
}