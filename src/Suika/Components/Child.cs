// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika.Util;

namespace Suika.Components;

public static class Child
{
    public static void Normal(string id, in Vector2 size, Action action, ChildNormalStyle style)
    {
        Normal(id, size, action, style.BackgroundColor, style.BorderColor, style.BorderThickness, style.Rounding);
    }

    public static void Normal(string id, in Vector2 size, Action content, Color backgroundColor = default, Color borderColor = default, float borderThickness = 0, float rounding = 0)
    {
        ImGui.PushStyleColor(ImGuiCol.ChildBg, backgroundColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Border, borderColor.ToVector4Color());
        ImGui.PushStyleVar(ImGuiStyleVar.ChildRounding,  rounding);
        ImGui.PushStyleVar(ImGuiStyleVar.ChildBorderSize, borderThickness);
        ImGui.BeginChild($"nc_{id}", size, borderThickness > 0 ? ImGuiChildFlags.Border : ImGuiChildFlags.None);
        content();
        ImGui.EndChild();
        ImGui.PopStyleVar(2);
        ImGui.PopStyleColor(2);
    }
}

public struct ChildNormalStyle
{
    public Color BackgroundColor { get; set; }
    public Color BorderColor { get; set; }
    public float BorderThickness { get; set; }
    public float Rounding { get; set; }
}