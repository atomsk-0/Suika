// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Suika.Data;
using Suika.Types.Interfaces;

namespace Suika;

public class Application
{
    private readonly IWindow window;
    private readonly AppOptions appOptions;

    public Application(in AppOptions appOptions)
    {
        this.appOptions = appOptions;
        if (OperatingSystem.IsWindows())
        {
            window = new Platforms.Windows.Window();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    public void SetTitlebarStyle(in Color backgroundColor, in Color borderColor, float borderThickness)
    {
        window.SetTitlebarStyle(backgroundColor, borderColor, borderThickness);
    }

    public void AddFont(Font font)
    {
        window.AddFont(font);
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
        return window.GetBackend().LoadTextureFromFile(path);
    }

    public Vector2 GetViewSize()
    {
        return window.GetViewSize();
    }

    public void SetTitlebarView(Action titlebarView)
    {
        window.TitlebarView = titlebarView;
    }

    public void SetView(Action view)
    {
        window.View = view;
    }

    public void CreateWindow()
    {
        window.Create(appOptions);
    }

    public void Run()
    {
        window.Render();
        window.Destroy();
    }
}