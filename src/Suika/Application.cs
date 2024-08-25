﻿// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika.Data;
using Suika.Types.Interfaces;

namespace Suika;

public class Application
{
    private readonly IWindow window;

    public Application(in AppOptions appOptions)
    {
        if (OperatingSystem.IsWindows())
        {
            window = new Platforms.Windows.Window();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
        window.Create(appOptions);
    }

    public void SetView(Action view)
    {
        window.View = view;
    }

    public void Run()
    {
        window.Render();
        window.Destroy();
    }
}