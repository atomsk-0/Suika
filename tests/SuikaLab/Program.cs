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
        App app = new App();

        var options = WindowOptions.Default;
        options.Title = "SuikaLab";

        app.OpenWindow(options);
        app.Run();
    }
}