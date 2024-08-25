// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using Suika.Types.Enums;

namespace Suika.Data;

public struct AppOptions
{
    /// <summary>
    /// Window title
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// Window start size
    /// </summary>
    public Size WindowSize { get; set; }

    /// <summary>
    /// Minimum window size
    /// </summary>
    public Size MinSize { get; set; }

    /// <summary>
    /// Window start pos
    /// </summary>
    public Point StartPos { get; set; }

    /// <summary>
    /// If set to true, the window can be resized from frame borders, maximized and minimized is allowed too
    /// </summary>
    public bool AllowResize { get; set; }

    /// <summary>
    /// If set to true, the window will use Windows 11 border style and not custom one (Only works on Windows 11)
    /// </summary>
    public bool UseWindows11Border { get; set; }

    /// <summary>
    /// If set to true, the window will use VSync (Locked to monitor refresh rate)
    /// </summary>
    public bool VSync { get; set; }

    /// <summary>
    /// If set to -1, the window will not be locked to a specific frame rate (unlimited fps), Not recommended to set as unlimited
    /// </summary>
    public int MaxFps { get; set; }

    /// <summary>
    /// Set the render backend to use
    /// </summary>
    public RenderBackend RenderBackend { get; set; }

    public static AppOptions DefaultWindows => new()
    {
        Title = "Suika Window",
        WindowSize = new Size(800, 500),
        MinSize = new Size(400, 250),
        StartPos = new Point(-1, -1), // Centered,
        AllowResize = true,
        UseWindows11Border = true,
        VSync = true,
        MaxFps = -1,
        RenderBackend = RenderBackend.DirectX9
    };

    public static AppOptions DefaultLinux => new()
    {
        Title = "Suika Window",
        WindowSize = new Size(800, 500),
        MinSize = new Size(400, 250),
        StartPos = new Point(-1, -1), // Centered
        AllowResize = true,
        UseWindows11Border = false, // Can be any value as its ignored on Linux
        VSync = true,
        MaxFps = -1,
        RenderBackend = RenderBackend.OpenGl
    };
}