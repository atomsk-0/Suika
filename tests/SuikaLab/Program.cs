// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Suika;
using Suika.Components;
using Suika.Data;
using Suika.Types.Enums;

namespace SuikaLab;

// This is project to test stuff while in early stage of development

internal static unsafe class Program
{
    private static readonly Font mainFont = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Inter-Medium.ttf"), 16f);
    private static readonly Font iconFont = new(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "fa-regular-400.ttf"), 15f);
    private static string testInput = "Hello World!";
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
        Spacer.Vertical(20);
        Child.Normal("test_child", new Vector2(200, 200), () =>
        {
            TextInput.Normal(label: "Hello World", labelFont: mainFont, inputFont: mainFont, text: ref testInput,
                maxLength: 600, hint: "Test Hint", textColor: Color.White, backgroundColor: Color.FromArgb(10, 10, 10), borderColor: Color.FromArgb(20, 20, 20),
                borderThickness: 1f, rounding: 3f, labelColor: Color.White, showLabel: true, padding: new Vector2(5, 5));
        }, backgroundColor: Color.FromArgb(30, 30, 30), borderColor: Color.FromArgb(60, 60, 60), borderThickness: 1f, rounding: 6f);
        /*Checkbox.Normal(id: "test_cb", size: mainFont.Size, font: mainFont, label: "Hello World", state: ref test,
            uncheckedBackgroundColor: Color.FromArgb(30, 30, 30),
            uncheckedTextColor: Color.FromArgb(200, 200, 200),
            uncheckedBorderColor: Color.FromArgb(61, 61, 61),
            hoveredBackgroundColor: Color.FromArgb(40, 40, 40),
            hoveredTextColor: Color.FromArgb(200, 200, 200),
            hoveredBorderColor: Color.FromArgb(71, 71, 71),
            checkedBackgroundColor: Color.White,
            checkedTextColor: Color.White,
            checkedMarkColor: Color.Black,
            rounding: 2f);
        Spacer.Line(Color.White, 1f, 5f, 5f);
        /*Checkbox.Normal(id: "test_cb1", size: mainFont.Size, font: mainFont, label: "Hello World 3", state: ref test,
            uncheckedBackgroundColor: Color.FromArgb(30, 30, 30),
            uncheckedTextColor: Color.FromArgb(200, 200, 200),
            uncheckedBorderColor: Color.FromArgb(61, 61, 61),
            hoveredBackgroundColor: Color.FromArgb(40, 40, 40),
            hoveredTextColor: Color.FromArgb(200, 200, 200),
            hoveredBorderColor: Color.FromArgb(71, 71, 71),
            rounding: 2f);
        Checkbox.Normal(id: "test_cb2", size: mainFont.Size, font: mainFont, label: "Hello World 4", state: ref test,
            uncheckedBackgroundColor: Color.FromArgb(30, 30, 30),
            uncheckedTextColor: Color.FromArgb(200, 200, 200),
            uncheckedBorderColor: Color.FromArgb(61, 61, 61),
            hoveredBackgroundColor: Color.FromArgb(40, 40, 40),
            hoveredTextColor: Color.FromArgb(200, 200, 200),
            hoveredBorderColor: Color.FromArgb(71, 71, 71),
            rounding: 2f);
        Checkbox.Normal(id: "test_cb3", size: mainFont.Size, font: mainFont, label: "Hello World 5", state: ref test,
            uncheckedBackgroundColor: Color.FromArgb(30, 30, 30),
            uncheckedTextColor: Color.FromArgb(200, 200, 200),
            uncheckedBorderColor: Color.FromArgb(61, 61, 61),
            hoveredBackgroundColor: Color.FromArgb(40, 40, 40),
            hoveredTextColor: Color.FromArgb(200, 200, 200),
            hoveredBorderColor: Color.FromArgb(71, 71, 71),
            rounding: 2f);
        Spacer.Vertical(8f);
        ImGui.Text($"HELLO WORLD: {DateTime.UtcNow.ToLongTimeString()}");
        ImGui.PushFont(mainFont.ImFont);
        Spacer.Vertical(8f);
        ImGui.Text($"HELLO WORLD: {DateTime.UtcNow.ToLongTimeString()}");
        ImGui.PopFont();
        ImGui.Text($"HELLO WORLD: {DateTime.UtcNow.ToLongTimeString()}");
        Spacer.Vertical(25f);
        Button.Normal(label: "Hello World",
            id: "test1",
            font: mainFont,
            backgroundColor: Color.FromArgb(30, 30, 30),
            textColor: Color.FromArgb(180, 180, 180),
            hoverBackgroundColor: Color.FromArgb(40, 40, 40),
            hoverTextColor: Color.FromArgb(255, 255, 255),
            borderThickness: 1f,
            borderColor: Color.FromArgb(50, 50, 50),
            padding: new Vector2(3, 3),
            radius: 3f);
        Spacer.Vertical(10f);
        Button.WithIcon(label: "Hello World",
            id: "test2",
            icon: "\u002b",
            iconFont: iconFont,
            labelFont: mainFont,
            spaceBetween: 6f,
            backgroundColor: Color.FromArgb(30, 30, 30),
            textColor: Color.FromArgb(180, 180, 180),
            hoverBackgroundColor: Color.FromArgb(40, 40, 40),
            hoverTextColor: Color.FromArgb(255, 255, 255),
            borderThickness: 1f,
            borderColor: Color.FromArgb(50, 50, 50),
            padding: new Vector2(3, 3),
            radius: 3f);
        Spacer.SameLine(10f);
        Button.WithIcon(label: "",
            id: "test3",
            icon: "\u002b",
            iconFont: iconFont,
            labelFont: mainFont,
            spaceBetween: 0f,
            backgroundColor: Color.FromArgb(30, 30, 30),
            textColor: Color.FromArgb(180, 180, 180),
            hoverBackgroundColor: Color.FromArgb(40, 40, 40),
            hoverTextColor: Color.FromArgb(255, 255, 255),
            borderThickness: 1f,
            borderColor: Color.FromArgb(50, 50, 50),
            padding: new Vector2(3, 3),
            radius: 3f);*/
    }
}