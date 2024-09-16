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
    private static readonly Font mainFont = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Inter-Medium.ttf"), 16f);
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
        app.AddFonts(mainFont, iconFont);
        app.SetTitlebarStyle(Color.FromArgb(30, 30, 30), Color.FromArgb(50, 50, 50), 1f);
        app.SetTitlebarView(titlebarView);
        app.SetView(renderView);
        app.CreateWindow();
        app.Run();
    }


    private static void titlebarView()
    {
        Spacer.Both(5, 10);
        Text.Normal("Suika testing", mainFont, Color.White);
    }


    private static void renderView()
    {
        Spacer.Both(10, 10);
        if (Button.Normal(id: "button_0", label: "Open context menu", font: mainFont, backgroundColor: Color.White, textColor: Color.Black,padding: new Vector2(6),rounding: 3f, hoverBackgroundColor: Color.CornflowerBlue, hoverTextColor: Color.Black))
        {
            ContextMenu.Show("button_0_context_menu");
        }
        ContextMenu.Normal(id: "button_0_context_menu", content: () =>
        {
            Text.Normal("Context menu item 0", mainFont, Color.White);
        }, position: ContextMenuPosition.Left, windowPadding: new Vector2(4, 8), itemSpacing: new Vector2(4, 8), backgroundColor: Color.FromArgb(30, 30, 30), borderColor: Color.FromArgb(50, 50, 50), rounding: 3f, borderThickness: 1f);
        Spacer.Vertical(10);
        if (Checkbox.Normal(id: "checkbox_0", font: mainFont, label: "Checkbox 0", state: ref testCheckbox, size: mainFont.Size, 3f, uncheckedBackgroundColor: Color.FromArgb(30, 30, 30), checkedBackgroundColor: Color.White, checkedMarkColor: Color.Black, checkedTextColor: Color.White, uncheckedTextColor: Color.White, hoveredTextColor: Color.White, hoveredBackgroundColor: Color.FromArgb(50, 50, 50)))
        {
            Console.WriteLine("Checkbox 0 clicked!");
        }
        Tooltip.Normal(text: "This is a tooltip", font: mainFont, backgroundColor: Color.FromArgb(30, 30, 30), textColor: Color.White, padding: new Vector2(8, 8), borderColor: Color.FromArgb(60, 60, 60), borderThickness: 1f, rounding: 3f);
        Spacer.Vertical(10);

    }
}