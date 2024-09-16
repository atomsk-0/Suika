// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Types.Enums;
using Suika.Util;

namespace Suika.Components;

public static unsafe class ContextMenu
{
    public static void Normal(string id, Action content, ContextMenuPosition position, Vector2 windowPadding = default, Vector2 itemSpacing = default, Color backgroundColor = default, Color borderColor = default, float rounding = 0, float borderThickness = 1)
    {
        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, windowPadding);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupRounding, rounding);
        ImGui.PushStyleVar(ImGuiStyleVar.PopupBorderSize, borderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, itemSpacing);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, backgroundColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Border, borderColor.ToVector4Color());

        var lastItemSize = ImGui.GetItemRectSize();
        var lastItemMinRect = ImGui.GetItemRectMin();

        // Set position to be bottom right of last item

        Vector2 positionVec = position switch
        {
            ContextMenuPosition.Left => new Vector2(lastItemMinRect.X, lastItemMinRect.Y + lastItemSize.Y + 2),
            ContextMenuPosition.Center => new Vector2(lastItemMinRect.X + lastItemSize.X / 2, lastItemMinRect.Y + lastItemSize.Y + 2),
            ContextMenuPosition.Right => new Vector2(lastItemMinRect.X + lastItemSize.X, lastItemMinRect.Y + lastItemSize.Y + 2),
            _ => throw new ArgumentOutOfRangeException(nameof(position), position, null)
        };

        ImGui.SetNextWindowPos(positionVec, ImGuiCond.Always, new Vector2(0, 0));
        if (ImGui.BeginPopup(id, ImGuiWindowFlags.NoMove))
        {
            content();
            ImGui.EndPopup();
        }
        ImGui.PopStyleVar(4);
        ImGui.PopStyleColor(2);
    }


    public static void Show(string id)
    {
        ImGui.OpenPopup(id);
    }
}