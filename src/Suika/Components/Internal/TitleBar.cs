// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Types.Interfaces;

namespace Suika.Components.Internal;

internal static unsafe class TitleBar
{
    internal static bool IsDragging;
    internal static bool TrackLock;
    private static IWindow platformWindow = null!;

    internal static void WindowsTitleBar()
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems)
        {
            return;
        }

        var platformTitleBarRect = platformWindow.GetTitleBarRect();
        ImRect titleBarRect = new ImRect(platformTitleBarRect.Left, platformTitleBarRect.Top, platformTitleBarRect.Right, platformTitleBarRect.Bottom);
        window->DrawList->AddRectFilled(titleBarRect.Min, titleBarRect.Max, ImGui.GetColorU32(ImGuiCol.TitleBgActive));

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
    }

    internal static void SetWindow(IWindow window)
    {
        platformWindow = window;
    }
}