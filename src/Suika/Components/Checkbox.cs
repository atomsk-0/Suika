// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Data;
using Suika.Util;
using Color = System.Drawing.Color;

namespace Suika.Components;

public static unsafe class Checkbox
{
    public static bool Normal(string id, Font font, string label, ref bool state, float size = 10, float rounding = 0,
        Color uncheckedBackgroundColor = default, Color uncheckedBorderColor = default,
        Color uncheckedTextColor = default, Color checkedBackgroundColor = default,
        Color checkedBorderColor = default, Color checkedMarkColor = default, Color checkedTextColor = default,
        Color hoveredBackgroundColor = default, Color hoveredBorderColor = default, Color hoveredTextColor = default,
        Color disabledBackgroundColor = default, Color disabledBorderColor = default, Color disabledMarkColor = default, Color disabledTextColor = default, bool disabled = false)
    {
        ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return false;

        var g = *ImGuiInternal.GImGui;
        var style = g->Style;
        var pos = window->DC.CursorPos;
        uint uId = window->GetID(id);

        Vector2 checkboxSize = new Vector2(size, size);
        ImGui.PushFont(font.ImFont);
        Vector2 labelSize = ImGui.CalcTextSize(label, false, 0);
        ImGui.PopFont();

        ImRect checkboxRect = new ImRect(pos, pos + checkboxSize);
        ImRect clickRect = new ImRect(checkboxRect.Min, checkboxRect.Max + new Vector2(size + labelSize.X, 0));

        ImGuiInternal.ItemSize(clickRect, style.FramePadding.Y);
        if (ImGuiInternal.ItemAdd(clickRect, uId) == false) return false;

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(clickRect, uId, &hovered, &held);

        if (disabled)
        {
            hovered = held = pressed = false;
        }

        if (pressed)
        {
            state = !state;
        }

        if (state)
        {
            window->DrawList->AddRectFilled(checkboxRect.Min, checkboxRect.Max, disabled ? disabledBackgroundColor.ToUint32Color() : checkedBackgroundColor.ToUint32Color(), rounding);
            if (uncheckedBorderColor != default)
            {
                window->DrawList->AddRect(checkboxRect.Min, checkboxRect.Max, disabled ? disabledBorderColor.ToUint32Color() : checkedBorderColor.ToUint32Color(), rounding);
            }

            ImGuiInternal.RenderCheckMark(window->DrawList, checkboxRect.Min + new Vector2(2, 2), disabled ? disabledMarkColor.ToUint32Color() :checkedMarkColor.ToUint32Color(), size - 4);

            if (label.Length > 0)
            {
                Vector2 textPosition = new Vector2(pos.X + size + 5, pos.Y + (checkboxSize.Y - labelSize.Y) / 2);
                window->DrawList->AddText(font.ImFont, font.Size, textPosition, disabled ?  disabledTextColor.ToUint32Color() : checkedTextColor.ToUint32Color(), label);
            }
        }
        else if (hovered)
        {
            window->DrawList->AddRectFilled(checkboxRect.Min, checkboxRect.Max, hoveredBackgroundColor.ToUint32Color(), rounding);
            if (uncheckedBorderColor != default)
            {
                window->DrawList->AddRect(checkboxRect.Min, checkboxRect.Max, hoveredBorderColor.ToUint32Color(), rounding);
            }

            if (label.Length > 0)
            {
                Vector2 textPosition = new Vector2(pos.X + size + 5, pos.Y + (checkboxSize.Y - labelSize.Y) / 2);
                window->DrawList->AddText(font.ImFont, font.Size, textPosition, hoveredTextColor.ToUint32Color(), label);
            }
        }
        else
        {
            window->DrawList->AddRectFilled(checkboxRect.Min, checkboxRect.Max, uncheckedBackgroundColor.ToUint32Color(), rounding);
            if (uncheckedBorderColor != default)
            {
                window->DrawList->AddRect(checkboxRect.Min, checkboxRect.Max, disabled ? disabledBorderColor.ToUint32Color() : uncheckedBorderColor.ToUint32Color(), rounding);
            }

            if (label.Length > 0)
            {
                Vector2 textPosition = new Vector2(pos.X + size + 5, pos.Y + (checkboxSize.Y - labelSize.Y) / 2);
                window->DrawList->AddText(font.ImFont, font.Size, textPosition, disabled ? disabledTextColor.ToUint32Color() : uncheckedTextColor.ToUint32Color(), label);
            }
        }

        return pressed;
    }
}