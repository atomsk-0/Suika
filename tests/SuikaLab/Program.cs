// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.ComponentModel;
using System.Runtime.InteropServices;
using Suika;
using Suika.Data;
using Suika.Platforms.Windows;
using TerraFX.Interop.Windows;

namespace SuikaLab;

internal static class Program
{
    // This is project to test stuff while in early stage of development

    public static void Main()
    {
        Window window = new Window();
        window.Create(WindowOptions.Default);
        window.SetupImGui();
        window.Render();
        window.Destroy();
    }
}