// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Util;

namespace Suika.Components;

public static unsafe class Spacer
{
    public static void Horizontal(float width)
    {
        ImGui.Dummy(new Vector2(width, 0));
    }

    public static void Vertical(float height)
    {
        ImGui.Dummy(new Vector2(0, height));
    }

    public static void SameLine(float spacing = 0, float offset = 0)
    {
        var g = *ImGuiInternal.GImGui;
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return;

        if (spacing < 0)
            spacing = g->Style.ItemSpacing.X;

        window->DC.CursorPos.X = window->DC.CursorPosPrevLine.X + spacing;
        window->DC.CursorPos.Y = window->DC.CursorPosPrevLine.Y - offset;

        window->DC.CurrLineSize = window->DC.PrevLineSize;
        window->DC.CurrLineTextBaseOffset = window->DC.PrevLineTextBaseOffset;
        window->DC.IsSameLine = true;
    }

    public static void Both(float width, float height)
    {
        ImGui.Dummy(new Vector2(width, height));
    }

    public static void Line(Color color, float thickness = 1f, float topSpacing = 0, float bottomSpacing = 0)
    {
        ImGui.Dummy(new Vector2(0, topSpacing));
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return;


        float x1 = window->DC.CursorPos.X;
        float x2 = window->WorkRect.Max.X;

        float thicknessForLayout = thickness == 1.0f ? 0.0f : thickness;
        ImRect bb = new ImRect(new Vector2(x1, window->DC.CursorPos.Y), new Vector2(x2, window->DC.CursorPos.Y + thickness));

        ImGuiInternal.ItemSize(new Vector2(0.0f, thicknessForLayout));
        if (ImGuiInternal.ItemAdd(bb, 0))
        {
            window->DrawList->AddRectFilled(bb.Min, bb.Max, color.ToUint32Color());
        }
        ImGui.Dummy(new Vector2(0, bottomSpacing));

    }
}