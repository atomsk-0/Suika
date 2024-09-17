// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Mochi.DearImGui;
using Suika.Data;
using Suika.Util;
using Color = System.Drawing.Color;

namespace Suika.Components;

public static unsafe class Table
{
    public static void Normal(string id, in Vector2 size, string[] headers, ImGuiTableFlags flags, Action content, TableNormalStyle style)
    {
        Normal(id, size, headers, flags, content, style.HeaderFont, style.TableFont, style.HeaderBg, style.RowBg, style.AltRowBg, style.BorderColor, style.HoveredHeaderColor, style.ClickedHeaderColor, style.HoveredBorderColor, style.ClickedBorderColor);
    }

    public static void Normal(string id, in Vector2 size, string[] headers, ImGuiTableFlags flags, Action content, Font headerFont, Font tableFont,
        Color headerBg = default, Color rowBg = default, Color altRowBg = default, Color borderColor = default, Color hoveredHeaderColor = default,
        Color clickedHeaderColor = default, Color hoveredBorderColor = default, Color clickedBorderColor = default)
    {
        ImGui.PushFont(headerFont.ImFont);
        ImGui.PushStyleColor(ImGuiCol.TableHeaderBg, headerBg.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.TableRowBg, rowBg.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, altRowBg.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.TableBorderStrong, borderColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.HeaderHovered, hoveredHeaderColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.SeparatorHovered, hoveredBorderColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.HeaderActive, clickedHeaderColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.SeparatorActive, clickedBorderColor.ToVector4Color());
        if (ImGui.BeginTable(id, headers.Length, flags, size))
        {
            ImGui.TableSetupScrollFreeze(0, 1);
            for (int i = 0; i < headers.Length; i++)
            {
                ImGui.TableSetupColumn(headers[i]);
            }
            ImGui.TableHeadersRow();
            ImGui.PushFont(tableFont.ImFont);
            content();
            ImGui.PopFont();
            ImGui.EndTable();
        }
        ImGui.PopFont();
        ImGui.PopStyleColor(8);
    }

    public static void NewRow()
    {
        NextRow();
        NextColumn();
    }

    public static void NextRow()
    {
        ImGui.TableNextRow();
    }

    public static void NextColumn()
    {
        ImGui.TableNextColumn();
    }
}

public struct TableNormalStyle
{
    public Font HeaderFont { get; set; }
    public Font TableFont { get; set; }
    public Color HeaderBg { get; set; }
    public Color RowBg { get; set; }
    public Color AltRowBg { get; set; }
    public Color BorderColor { get; set; }
    public Color HoveredHeaderColor { get; set; }
    public Color ClickedHeaderColor { get; set; }
    public Color HoveredBorderColor { get; set; }
    public Color ClickedBorderColor { get; set; }
}