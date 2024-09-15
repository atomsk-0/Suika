// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Suika.Data;
using Suika.Types.Interfaces;

namespace Suika;

public class Application
{
    public readonly IWindow Window;
    private readonly AppOptions appOptions;

    public Application(in AppOptions appOptions)
    {
        this.appOptions = appOptions;
        if (OperatingSystem.IsWindows())
        {
            Window = new Platforms.Windows.Window();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void SetTitlebarStyle(in Color backgroundColor, in Color borderColor, float borderThickness)
    {
        Window.SetTitlebarStyle(backgroundColor, borderColor, borderThickness);
    }

    public void AddFont(Font font)
    {
        Window.AddFont(font);
    }

    public void AddFonts(params Font[] fonts)
    {
        foreach (Font font in fonts)
        {
            AddFont(font);
        }
    }

    public void AddFont(string path, float size)
    {
        AddFont(new Font(path, size));
    }

    public Texture LoadTextureFromFile(string path)
    {
        return Window.GetBackend().LoadTextureFromFile(path);
    }

    public Vector2 GetTitlebarWorkArea()
    {
        var titleBarRect = Window.GetTitleBarRect();
        titleBarRect.Right -= Window.GetCaptionButtonWidth() * 4; // Hardcoded for now, should be calculated
        return new Vector2(titleBarRect.Right, titleBarRect.Bottom);
    }

    public void SetTitlebarView(Action titlebarView)
    {
        Window.TitlebarView = titlebarView;
    }

    public void SetView(Action view)
    {
        Window.View = view;
    }

    public void CreateWindow()
    {
        Window.Create(appOptions);
    }

    public void Run()
    {
        Window.Render();
        Window.Destroy();
    }
}