// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Data;
using Suika.Util;

namespace Suika.Components;

public static unsafe class Text
{
    public static void Normal(string text, Font font, Color color)
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

        window->DrawList->AddText(font.ImFont, font.Size, position, color.ToUint32Color(), text);
    }
}