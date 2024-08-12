// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using Suika.Types.Enums;

namespace Suika.Data;

public struct AppOptions
{
    public string Title { get; set; }

    public Size WindowSize { get; set; }
    public Point StartPos { get; set; }

    public bool VSync { get; set; }
    public int MaxFps { get; set; }

    public RenderBackend RenderBackend { get; set; }

    public static AppOptions DefaultWindows => new()
    {
        Title = "Suika Window",
        WindowSize = new Size(800, 500),
        StartPos = new Point(-1, -1), // Centered
        VSync = true,
        MaxFps = -1,
        RenderBackend = RenderBackend.DirectX9
    };

    public static AppOptions DefaultLinux => new()
    {
        Title = "Suika Window",
        WindowSize = new Size(800, 500),
        StartPos = new Point(-1, -1), // Centered
        VSync = true,
        MaxFps = -1,
        RenderBackend = RenderBackend.OpenGl
    };
}