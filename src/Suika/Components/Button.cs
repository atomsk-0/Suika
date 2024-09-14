// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Data;
using Suika.Types.Enums;
using Suika.Util;

namespace Suika.Components;

public static unsafe class Button
{
    public static bool Normal(string id, string label, in ButtonNormalStyle style, bool disabled = false)
    {
        return Normal(id, label, style.Font, style.BackgroundColor, style.TextColor, style.BorderThickness, style.BorderColor, style.Radius, style.Padding, disabled, style.HoverBackgroundColor, style.HoverTextColor, style.HoverBorderColor, style.DisabledBorderColor, style.DisabledBackgroundColor, style.DisabledTextColor);
    }

    public static bool Normal(string id, string label, Font font, Color backgroundColor, Color textColor, float borderThickness = 0f,
        Color borderColor = default, float radius = 0f, Vector2 padding = default, bool disabled = false,
        Color hoverBackgroundColor = default, Color hoverTextColor = default, Color hoverBorderColor = default, Color disabledBorderColor = default, Color disabledBackgroundColor = default, Color disabledTextColor = default)
    {
        var window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems)
            return false;

        ImGuiContext* g = *ImGuiInternal.GImGui;
        ImGuiStyle style = g->Style;
        uint uId = window->GetID(id);
        Vector2 position = window->DC.CursorPos;

        ImGui.PushFont(font.ImFont);
        Vector2 size = ImGui.CalcTextSize(label, true) + new Vector2(padding.X * 2, padding.Y * 2);
        ImGui.PopFont();

        ImRect rect = new ImRect(position, position + size);
        ImGuiInternal.ItemSize(size, style.FramePadding.Y);
        if (ImGuiInternal.ItemAdd(rect, uId) == false) return false;

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, uId, &hovered, &held);

        if (disabled)
        {
            hovered = held = pressed = false;
        }

        window->DrawList->AddRectFilled(rect.Min, rect.Max, hovered ? hoverBackgroundColor.ToUint32Color() : backgroundColor.ToUint32Color(), radius);
        if (borderThickness > 0f)
        {
            window->DrawList->AddRect(rect.Min, rect.Max, hovered ? hoverBorderColor.ToUint32Color() : borderColor.ToUint32Color(), radius, ImDrawFlags.None, borderThickness);
        }

        Vector2 textPosition = new Vector2(position.X + padding.X, position.Y + padding.Y);

        window->DrawList->AddText(font.ImFont, font.Size, textPosition, hovered ? hoverTextColor.ToUint32Color() : textColor.ToUint32Color(), label);

        return pressed;
    }

    public static bool WithIcon(string id, string label, string icon, in ButtonWithIconStyle style, bool disabled = false)
    {
        return WithIcon(id, label, style.LabelFont, icon, style.IconFont, style.BackgroundColor, style.TextColor, style.SpaceBetween, style.BorderThickness, style.BorderColor, style.Radius, style.Padding, disabled, style.HoverBackgroundColor, style.HoverTextColor, style.HoverBorderColor, style.DisabledBorderColor, style.DisabledBackgroundColor, style.DisabledTextColor);
    }

    public static bool WithIcon(string id, string label, Font labelFont, string icon, Font iconFont,  Color backgroundColor, Color textColor, float spaceBetween = 2f, float borderThickness = 0f,
        Color borderColor = default, float radius = 0f, Vector2 padding = default, bool disabled = false,
        Color hoverBackgroundColor = default, Color hoverTextColor = default, Color hoverBorderColor = default, Color disabledBorderColor = default, Color disabledBackgroundColor = default, Color disabledTextColor = default)
    {
        var window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems)
            return false;

        ImGuiContext* g = *ImGuiInternal.GImGui;
        ImGuiStyle style = g->Style;
        uint uid = window->GetID(id);
        Vector2 position = window->DC.CursorPos;
        ImGui.PushFont(labelFont.ImFont);
        var labelSize = string.IsNullOrEmpty(label) ? Vector2.Zero : ImGui.CalcTextSize(label, true);
        ImGui.PopFont();
        ImGui.PushFont(iconFont.ImFont);
        var iconSize = ImGui.CalcTextSize(icon, true);
        ImGui.PopFont();

        Vector2 size = iconSize + labelSize + new Vector2(padding.X * 2, padding.Y * 2);
        size.X += spaceBetween;
        if (string.IsNullOrEmpty(label) == false)
        {
            size.Y -= iconSize.Y;
        }


        ImRect rect = new ImRect(position, position + size);
        ImGuiInternal.ItemSize(size, style.FramePadding.Y);
        if (ImGuiInternal.ItemAdd(rect, uid) == false) return false;

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, uid, &hovered, &held);

        if (disabled)
        {
            hovered = held = pressed = false;
        }

        window->DrawList->AddRectFilled(rect.Min, rect.Max, hovered ? hoverBackgroundColor.ToUint32Color() : backgroundColor.ToUint32Color(), radius);
        if (borderThickness > 0f)
        {
            window->DrawList->AddRect(rect.Min, rect.Max, hovered ? hoverBorderColor.ToUint32Color() : borderColor.ToUint32Color(), radius, ImDrawFlags.None, borderThickness);
        }

        Vector2 iconPosition = new Vector2(position.X + padding.X, position.Y + padding.Y);
        Vector2 textPosition = new Vector2(iconPosition.X + iconSize.X + spaceBetween, position.Y + padding.Y);

        window->DrawList->AddText(iconFont.ImFont, iconFont.Size, iconPosition, hovered ? hoverTextColor.ToUint32Color() : textColor.ToUint32Color(), icon);
        if (string.IsNullOrEmpty(label) == false)
        {
            window->DrawList->AddText(labelFont.ImFont, labelFont.Size, textPosition, hovered ? hoverTextColor.ToUint32Color() : textColor.ToUint32Color(), label);
        }

        return pressed;
    }
}

public struct ButtonNormalStyle
{
    public Font Font { get; set; }
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }
    public float BorderThickness { get; set; }
    public Color BorderColor { get; set; }
    public float Radius { get; set; }
    public Vector2 Padding { get; set; }
    public Color HoverBackgroundColor { get; set; }
    public Color HoverTextColor { get; set; }
    public Color HoverBorderColor { get; set; }
    public Color DisabledBorderColor { get; set; }
    public Color DisabledBackgroundColor { get; set; }
    public Color DisabledTextColor { get; set; }
}

public struct ButtonWithIconStyle
{
    public Font LabelFont { get; set; }
    public Font IconFont { get; set; }
    public Color BackgroundColor { get; set; }
    public Color TextColor { get; set; }
    public float SpaceBetween { get; set; }
    public float BorderThickness { get; set; }
    public Color BorderColor { get; set; }
    public float Radius { get; set; }
    public Vector2 Padding { get; set; }
    public Color HoverBackgroundColor { get; set; }
    public Color HoverTextColor { get; set; }
    public Color HoverBorderColor { get; set; }
    public Color DisabledBorderColor { get; set; }
    public Color DisabledBackgroundColor { get; set; }
    public Color DisabledTextColor { get; set; }
}