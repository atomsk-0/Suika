// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using Suika.Components;
using Suika.Components.Base;

namespace Suika.Data;

public struct WindowOptions
{
    public string Title { get; set; }
    public Size Size { get; set; }
    public bool TitleBar { get; set; }
    public bool AllowResize { get; set; }
    public bool VSync { get; set; }
    public int MaxFps { get; set; }
    public TitleBar TitleBarComponent { get; set; }

    // Default
    public static WindowOptions Default => new()
    {
        Title = "Suika Window",
        Size = new Size(800, 600),
        TitleBar = true,
        AllowResize = true,
        MaxFps = -1,
        VSync = true,
        TitleBarComponent = new DefaultTitleBar()
    };
}