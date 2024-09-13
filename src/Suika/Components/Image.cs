// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Util;

namespace Suika.Components;

public static unsafe class Image
{
    public static void Normal(in Texture texture)
    {
        ImGui.Image((void*)texture.Handle, new Vector2(texture.Width, texture.Height), Vector2.Zero, Vector2.One, Vector4.One, Vector4.Zero);
    }

    public static void Normal(in Texture texture, in Vector2 size)
    {
        ImGui.Image((void*)texture.Handle, size, Vector2.Zero, Vector2.One, Vector4.One, Vector4.Zero);
    }

    public static void Normal(in Texture texture, in Vector2 size, in Color tintColor)
    {
        ImGui.Image((void*)texture.Handle, size, Vector2.Zero, Vector2.One, tintColor.ToVector4Color(), Vector4.Zero);
    }
}