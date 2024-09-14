// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Util;
using TerraFX.Interop.Windows;

namespace Suika.Components;

// TODO: Add multi-line text input
// TODO: Add icon version of text input (left & right icon as options)

public static unsafe class TextInput
{
    private const ushort stack_allocation_size_limit = 2048;


    public static void Normal(string label, ref string text, uint maxLength, in TextInputNormalStyle style, string hint = "", int width = 0, bool showLabel = true, bool hideInput = false)
    {
        Normal(label, style.LabelFont, style.InputFont, ref text, maxLength, hint, style.TextColor, style.BackgroundColor, style.BorderColor, style.BorderThickness, style.Rounding, style.LabelColor, showLabel, style.Padding, width, hideInput);
    }

    public static void Normal(string label, Font labelFont, Font inputFont, ref string text, uint maxLength, string hint = "", Color textColor = default, Color backgroundColor = default,
        Color borderColor = default, float borderThickness = 0, float rounding = 0, Color labelColor = default, bool showLabel = true, Vector2 padding = default, int width = 0, bool hideInput = false)
    {
        if (showLabel) Text.Normal(label, labelFont, labelColor);
        if (width > 0) ImGui.SetNextItemWidth(width);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, backgroundColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Border, borderColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Text, textColor.ToVector4Color());
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, padding);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameBorderSize, borderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.FrameRounding, rounding);
        ImGui.PushFont(inputFont.ImFont);
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() - 2); // For some reason the cursor is off by 2 pixels
        managedInputTextWithHint($"##{label}_input", hint, ref text, maxLength, hideInput ? ImGuiInputTextFlags.Password : ImGuiInputTextFlags.None);
        ImGui.PopFont();
        ImGui.PopStyleVar(3);
        ImGui.PopStyleColor(3);
    }

    private static bool managedInputTextWithHint(string label, string hint, ref string input, uint maxLength, ImGuiInputTextFlags flags = ImGuiInputTextFlags.None)
    {
        int inputLength = Encoding.UTF8.GetByteCount(input);
        int bufferSize = Math.Max((int)maxLength + 1, inputLength + 1);

        byte* inputBuffer;
        byte* ogInputBuffer;
        if (bufferSize > stack_allocation_size_limit)
        {
            inputBuffer = (byte*)NativeMemory.Alloc((nuint)bufferSize);
            ogInputBuffer = (byte*)NativeMemory.Alloc((nuint)bufferSize);
        }
        else
        {
            byte* stackBuffer = stackalloc byte[bufferSize];
            byte* ogStackBuffer = stackalloc byte[bufferSize];
            inputBuffer = stackBuffer;
            ogInputBuffer = ogStackBuffer;
        }

        getUtf8(input, inputBuffer, bufferSize);
        uint clearLength = (uint)(bufferSize - inputLength);
        Unsafe.InitBlockUnaligned(inputBuffer + inputLength, 0, clearLength);
        Unsafe.CopyBlock(ogInputBuffer, inputBuffer, (uint)bufferSize);

        bool result = ImGui.InputTextWithHint(label, hint, inputBuffer, (nuint)bufferSize, flags);

        if (areStringsEqual(ogInputBuffer, bufferSize, inputBuffer) == false)
        {
            input = stringFromPtr(inputBuffer);
        }

        if (bufferSize > stack_allocation_size_limit)
        {
            NativeMemory.Free(inputBuffer);
            NativeMemory.Free(ogInputBuffer);
        }

        return result;
    }


    private static int getUtf8(string str, byte* buffer, int length)
    {
        fixed (char* stPtr = str)
        {
            return Encoding.UTF8.GetBytes(stPtr, str.Length, buffer, length);
        }
    }

    private static string stringFromPtr(byte* ptr)
    {
        int chars = 0;
        while(ptr[chars] != 0) chars++;
        return Encoding.UTF8.GetString(ptr, chars);
    }

    private static bool areStringsEqual(byte* a, int aLength, byte* b)
    {
        for (int i = 0; i < aLength; i++)
        {
            if (a[i] != b[i]) return false;
        }
        return b[aLength] == 0;
    }
}

public struct TextInputNormalStyle
{
    public Font LabelFont { get; set; }
    public Font InputFont { get; set; }
    public Color TextColor { get; set; }
    public Color BackgroundColor { get; set; }
    public Color BorderColor { get; set; }
    public float BorderThickness { get; set; }
    public float Rounding { get; set; }
    public Color LabelColor { get; set; }
    public Vector2 Padding { get; set; }
}