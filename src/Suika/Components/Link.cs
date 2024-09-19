// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Data;
using Suika.Util;

namespace Suika.Components;

public static unsafe class Link
{
    public static void Normal(string text, Font font, Color color, Color hoverColor, string url, bool underLine = true)
    {
        var window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return;

        Vector2 position = window->DC.CursorPos;
        uint id = window->GetID(text);

        ImGui.PushFont(font.ImFont);
        Vector2 size = ImGui.CalcTextSize(text, true);
        ImGui.PopFont();

        ImRect rect = new ImRect(position, position + size);
        ImGuiInternal.ItemSize(size, 0);
        if (ImGuiInternal.ItemAdd(rect, id) == false) return;

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, id, &hovered, &held);

        if (hovered)
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        if (underLine)
        {
            Vector2 underlineStart = position with { Y = position.Y + size.Y - 1 };
            Vector2 underlineEnd = new Vector2(position.X + size.X, position.Y + size.Y - 1);
            window->DrawList->AddLine(underlineStart, underlineEnd, hovered ? hoverColor.ToUint32Color() : color.ToUint32Color());
        }

        window->DrawList->AddText(font.ImFont, font.Size, position, hovered ? hoverColor.ToUint32Color() : color.ToUint32Color(), text);

        if (pressed)
        {
            Process.Start(url);
        }
    }

    public static void Normal(string text, Font font, Color color, Color hoverColor, Action action, bool underLine = true)
    {
        var window = ImGuiInternal.GetCurrentWindow();
        if (window->SkipItems) return;

        Vector2 position = window->DC.CursorPos;
        uint id = window->GetID(text);

        ImGui.PushFont(font.ImFont);
        Vector2 size = ImGui.CalcTextSize(text, true);
        ImGui.PopFont();

        ImRect rect = new ImRect(position, position + size);
        ImGuiInternal.ItemSize(size, 0);
        if (ImGuiInternal.ItemAdd(rect, id) == false) return;

        bool hovered, held;
        bool pressed = ImGuiInternal.ButtonBehavior(rect, id, &hovered, &held);

        if (hovered)
        {
            ImGui.SetMouseCursor(ImGuiMouseCursor.Hand);
        }

        if (underLine)
        {
            Vector2 underlineStart = position with { Y = position.Y + size.Y - 1 };
            Vector2 underlineEnd = new Vector2(position.X + size.X, position.Y + size.Y - 1);
            window->DrawList->AddLine(underlineStart, underlineEnd, hovered ? hoverColor.ToUint32Color() : color.ToUint32Color());
        }

        window->DrawList->AddText(font.ImFont, font.Size, position, hovered ? hoverColor.ToUint32Color() : color.ToUint32Color(), text);

        if (pressed)
        {
            action();
        }
    }
}