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
    private static Texture testTexture;

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
        testTexture = app.LoadTextureFromFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory + "test.png"));
        app.Run();
    }


    private static void titlebarView()
    {
        Spacer.Both(5, 10);
        Text.Normal("Suika testing", mainFont, Color.White);
    }


    private static void renderView()
    {
        Spacer.Vertical(8f);
        Text.Normal("Normal text", mainFont, Color.White);
        Spacer.Vertical(8f);
        Button.Normal(label: "Normal Button",
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
        Button.WithIcon(label: "Icon Button with text",
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
            id: "icon_button",
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
            radius: 3f);

        ImGui.Image((void*)testTexture.Handle, new Vector2(testTexture.Width, testTexture.Height), Vector2.Zero, Vector2.One, Vector4.One, Vector4.Zero);
    }
}