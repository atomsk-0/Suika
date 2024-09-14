// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Mochi.DearImGui;

namespace Suika.Data;

public unsafe class Font(string path, float size, char? rangeStart = null, char? rangeEnd = null, float glyphMinAdvanceX = 0)
{
    public readonly string Path = path;
    public readonly float Size = size;
    public readonly char? RangeStart = rangeStart;
    public readonly char? RangeEnd = rangeEnd;
    public readonly float GlyphMinAdvanceX = glyphMinAdvanceX;
    public ImFont* ImFont;
}