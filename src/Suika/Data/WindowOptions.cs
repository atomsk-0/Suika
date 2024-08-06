// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;

namespace Suika.Data;

public struct WindowOptions
{
    // Window title
    public string Title { get; init; }
    public Size Size { get; init; }

    // Default
    public static WindowOptions Default => new()
    {
        Title = "Suika Window",
        Size = new Size(800, 600)
    };
}