// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Suika.Components;

namespace SuikaLab;

public class ModalTest() : Modal("modal_test", "Test Modal", true, new Vector2(270, 445), new ModalStyle
{
    TitleFont = Program.MAIN_FONT,
    BackgroundColor = Color.FromArgb(81, 81, 81),
    DimColor = Color.Black,
    WindowRounding = 6f,
    BorderThickness = 0f,
    ItemSpacing = new Vector2(4, 8),
    TitlebarColor = Color.FromArgb(24, 24, 24),
    TitlebarTextColor = Color.FromArgb(255, 255, 255)
})
{
    protected override void Content()
    {
        Text.Normal("Hello World", Program.MAIN_FONT, Color.White);
    }

    public static readonly ModalTest INSTANCE = new();
}