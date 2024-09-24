// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika;
using Suika.Components;
using Suika.Data;
using Suika.Types.Enums;
using TerraFX.Interop.Windows;

namespace SuikaLab;

// This is project to test stuff while in early stage of development

internal static unsafe class Program
{
    internal static readonly Font MAIN_FONT = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Inter-Medium.ttf"), 16f);
    private static readonly Font iconFont = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fa-regular-400.ttf"), 15f);
    private static string testInput = "Hello World!";
    private static bool testCheckbox = false;
    private static Application app;

    public static void Main()
    {
        var options = AppOptions.DefaultWindows;
        options.VSync = true;
        options.RenderBackend = RenderBackend.DirectX9;

        app = new Application(options);
        app.AddFonts(MAIN_FONT, iconFont);
        app.SetTitlebarStyle(Color.FromArgb(30, 30, 30), Color.FromArgb(50, 50, 50), 1f);
        app.SetTitlebarView(titlebarView);
        app.SetView(renderView);
        app.CreateWindow();
        app.Run();
    }


    private static void titlebarView()
    {
        Spacer.Both(10);
        Text.Normal("Suika testing", MAIN_FONT, Color.White);
    }


    private static void renderView()
    {
        Spacer.Horizontal(20);
        Link.Normal("Hello World!", MAIN_FONT, Color.White, Color.FromArgb(100, 100, 100), () =>
        {
            ModalTest.INSTANCE.Show();
        });
    }
}