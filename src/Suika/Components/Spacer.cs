// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Mochi.DearImGui;

namespace Suika.Components;

public static class Spacer
{
    public static void Horizontal(float width)
    {
        ImGui.Dummy(new Vector2(width, 0));
    }

    public static void Vertical(float height)
    {
        ImGui.Dummy(new Vector2(0, height));
    }

    public static void SameLine(float spacing = 0)
    {
        ImGui.SameLine(0f, spacing);
    }

    public static void Both(float width, float height)
    {
        ImGui.Dummy(new Vector2(width, height));
    }
}