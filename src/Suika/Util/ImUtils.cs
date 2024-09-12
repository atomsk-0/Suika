// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;

namespace Suika.Util;

internal static class ImUtils
{
    internal static uint ToUint32Color(this Color color)
    {
        return ImGui.GetColorU32(color.ToVector4Color());
    }

    internal static Vector4 ToVector4Color(this Color color)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    internal static uint ToUint32Color(this Color color, byte alpha)
    {
        return ImGui.GetColorU32(color.ToVector4Color(alpha));
    }

    internal static Vector4 ToVector4Color(this Color color, byte alpha)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, alpha / 255f);
    }
}