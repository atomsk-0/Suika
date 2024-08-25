// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace Suika.Data;

public struct RectF(float left, float right, float top, float bottom)
{
    public float Left { get; set; } = left;
    public float Right { get; set; } = right;
    public float Top { get; set; } = top;
    public float Bottom { get; set; } = bottom;
}