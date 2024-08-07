// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Data;
using Suika.Types.Interfaces;

namespace Suika.Components.Base;

public abstract unsafe class TitleBar
{
    protected IWindow Window;

    protected bool IsDragging;

    public virtual void Render()
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return;

        var platformTitleBarRect = Window.GetTitleBarRect();
        ImRect titleBarRect = new ImRect(platformTitleBarRect.Left, platformTitleBarRect.Top, platformTitleBarRect.Right, platformTitleBarRect.Bottom);
        window->DrawList->AddRectFilled(titleBarRect.Min, titleBarRect.Max, ImGui.GetColorU32(ImGuiCol.TitleBgActive));

        uint titleBarId = window->GetID("title_bar");
        ImGui.SetNextItemAllowOverlap();
        ImGuiInternal.ItemSize(titleBarRect, 0);
        ImGuiInternal.ItemAdd(titleBarRect, titleBarId);

        bool hovered, held;
        ImGuiInternal.ButtonBehavior(titleBarRect, titleBarId, &hovered, &held);
        if (hovered && held && IsDragging == false)
        {
            Window.DragWindow();
            IsDragging = true;
            Window.Activate();
            Window.SimulateLeftMouseClick(); // Trick to focus on ImGui window again
        }
        else if (held == false)
        {
            IsDragging = false;
        }
    }

    public void SetWindow(IWindow window)
    {
        Window = window;
    }
}