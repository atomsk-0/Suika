// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Util;

namespace Suika.Components;

// TODO: Add "WithIcon" method to display an icon next to the text.

public static unsafe class Tooltip
{
    public static void Normal(string text, in TooltipStyleNormal style)
    {
        Normal(text, style.Font, style.BackgroundColor, style.TextColor, style.Padding, style.BorderColor, style.BorderThickness, style.Rounding);
    }

    public static void Normal(string text, Font font, Color backgroundColor, Color textColor, Vector2 padding, Color borderColor = default, float borderThickness = default, float rounding = 0)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, padding);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, rounding);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, borderThickness);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, backgroundColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Text, textColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Border, borderColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.BorderShadow, Color.Black.ToVector4Color());
        ImGui.PushFont(font.ImFont);
        ImGui.SetItemTooltip(text);
        ImGui.PopFont();
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(4);
    }
}

public struct TooltipStyleNormal
{
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }
    public Vector2 Padding { get; set; }
    public Color BorderColor { get; set; }
    public float BorderThickness { get; set; }
    public float Rounding { get; set; }
    public Font Font { get; set; }
}