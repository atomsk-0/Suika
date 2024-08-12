// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika;
using Suika.Data;
using Suika.Types.Enums;

namespace SuikaLab;

internal static class Program
{
    // This is project to test stuff while in early stage of development

    public static void Main()
    {
        var options = AppOptions.DefaultWindows;
        options.VSync = true;
        options.RenderBackend = RenderBackend.DirectX9;
        Application app = new Application(options);
        app.Run();
    }
}