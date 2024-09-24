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

    /// <summary>
    /// Initializes a new instance of the <see cref="Application"/> class with the specified application options.
    /// </summary>
    /// <param name="appOptions">The application options to use for initialization.</param>
    /// <exception cref="PlatformNotSupportedException">Thrown if the operating system is not Supported.</exception>
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


    /// <summary>
    /// Sets the style of the titlebar with the specified background color, border color, and border thickness.
    /// </summary>
    /// <param name="backgroundColor">The background color of the titlebar.</param>
    /// <param name="borderColor">The border color of the titlebar.</param>
    /// <param name="borderThickness">The thickness of the titlebar border.</param>
    public void SetTitlebarStyle(in Color backgroundColor, in Color borderColor, float borderThickness)
    {
        Window.SetTitlebarStyle(backgroundColor, borderColor, borderThickness);
    }


    /// <summary>
    /// Adds a font to the application.
    /// </summary>
    /// <param name="font">The font to add.</param>
    public void AddFont(Font font)
    {
        Window.AddFont(font);
    }


    /// <summary>
    /// Adds multiple fonts to the application.
    /// </summary>
    /// <param name="fonts">The fonts to add.</param>
    public void AddFonts(params Font[] fonts)
    {
        foreach (Font font in fonts)
        {
            AddFont(font);
        }
    }


    /// <summary>
    /// Adds a font to the application from the specified file path and size.
    /// </summary>
    /// <param name="path">The file path of the font.</param>
    /// <param name="size">The size of the font.</param>
    public void AddFont(string path, float size)
    {
        AddFont(new Font(path, size));
    }


    /// <summary>
    /// Loads a texture from the specified file path.
    /// </summary>
    /// <param name="path">The file path of the texture.</param>
    /// <returns>The loaded texture.</returns>
    public Texture LoadTextureFromFile(string path)
    {
        return Window.GetBackend().LoadTextureFromFile(path);
    }


    /// <summary>
    /// Gets the work area of the titlebar.
    /// </summary>
    /// <returns>A <see cref="Vector2"/> representing the work area of the titlebar.</returns>
    public Vector2 GetTitlebarWorkArea()
    {
        var titleBarRect = Window.GetTitleBarRect();

        // Hardcoded for now. Modify this once we start optimizing and refactoring the project closer to first release
        titleBarRect.Right -= Window.GetCaptionButtonWidth() * 4;
        if (OperatingSystem.IsWindows() && Window.IsMaximized())
        {
            titleBarRect.Right -= 26;
        }

        return new Vector2(titleBarRect.Right, titleBarRect.Bottom);
    }


    /// <summary>
    /// Sets the view render for the titlebar.
    /// </summary>
    /// <param name="titlebarView">The action to set as the titlebar view.</param>
    public void SetTitlebarView(Action titlebarView)
    {
        Window.TitlebarView = titlebarView;
    }


    /// <summary>
    /// Sets the main view render of the application.
    /// </summary>
    /// <param name="view">The action to set as the main view.</param>
    public void SetView(Action view)
    {
        Window.View = view;
    }


    /// <summary>
    /// Sets the callback to be invoked when ImGui is loaded. Use this to set your own ImGui io settings.
    /// </summary>
    /// <param name="callback">The callback action.</param>
    public void SetImGuiLoadCallback(Action callback)
    {
        Window.UserImGuiLoad = callback;
    }


    /// <summary>
    /// Creates the application window with the specified options.
    /// </summary>
    public void CreateWindow()
    {
        Window.Create(appOptions);
    }


    /// <summary>
    /// Starts the application rendering loop. When rendering loop breaks, the application window is destroyed.
    /// </summary>
    public void Run()
    {
        Window.Render();
        Window.Destroy();
    }
}