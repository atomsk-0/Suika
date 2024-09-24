// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Mochi.DearImGui;
using Mochi.DearImGui.Internal;
using Suika.Components.Internal;
using Suika.Data;
using Suika.Platforms.Windows;
using Suika.Util;

namespace Suika.Components;

public abstract unsafe class Modal
{
    private readonly string id;
    private readonly string title;
    private readonly bool titleBar;
    private readonly Vector2 size;
    private readonly ModalStyle modalStyle;

    private bool show;


    protected Modal(string id, string title, bool titleBar, Vector2 size, ModalStyle modalStyle)
    {
        this.id = id;
        this.title = title;
        this.titleBar = titleBar;
        this.size = size;
        this.modalStyle = modalStyle;

        ModalManager.RegisterModal(this);
    }

    public void Show()
    {
        show = true;
        ImGui.OpenPopup(id);
    }

    public void Close()
    {
        show = false;;
    }

    // Currently titlebar is hardcoded and windows only
    internal void Render()
    {
        bool localOpen = show;

        ImGui.PushStyleColor(ImGuiCol.PopupBg, modalStyle.BackgroundColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.ModalWindowDimBg, modalStyle.DimColor.ToVector4Color());
        ImGui.PushStyleColor(ImGuiCol.Border, modalStyle.BorderColor.ToVector4Color());
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, modalStyle.BorderThickness);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, modalStyle.WindowRounding);
        ImGui.PushStyleVar(ImGuiStyleVar.ItemSpacing, modalStyle.ItemSpacing);
        ImGui.SetNextWindowSize(size);

        if (ImGui.BeginPopupModal(id, &localOpen, ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoDecoration | ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoMove))
        {
            if (titleBar)
            {
                ImGuiWindow* window = ImGuiInternal.GetCurrentWindow();
                if (window->SkipItems) return;
                ImGui.SetCursorPos(new Vector2(0, 0));

                var pos = window->DC.CursorPos;
                window->DrawList->AddRectFilled(pos, new Vector2(pos.X + size.X, pos.Y + 32), modalStyle.TitlebarColor.ToUint32Color(), modalStyle.WindowRounding, ImDrawFlags.RoundCornersTop);
                window->DrawList->AddLine(pos with { Y = pos.Y + 32}, new Vector2(pos.X + size.X, pos.Y + 32), modalStyle.TitlebarBorderColor.ToUint32Color(), modalStyle.TitlebarBorderThickness);
                ImGui.SetCursorPos(new Vector2(10, 8));
                Text.Normal(title, modalStyle.TitleFont, modalStyle.TitlebarTextColor);
                ImGui.SetCursorPos(new Vector2(size.X - 32, 0));
                if (InternalTitleBar.WindowsTitlebarButton(Platform.CLOSE_ICON))
                {
                    ImGui.CloseCurrentPopup();
                }
            }
            ImGui.SetCursorPos(new Vector2(0, titleBar ? 35 : 0));
            Content();
            ImGui.EndPopup();
        }

        ImGui.PopStyleColor(3);
        ImGui.PopStyleVar(2);
    }


    protected abstract void Content();
}

public struct ModalStyle
{
    public Font TitleFont { get; set; }
    public Color BackgroundColor { get; set; }
    public Color BorderColor { get; set; }
    public Color DimColor { get; set; }
    public float WindowRounding { get; set; }
    public float BorderThickness { get; set; }
    public Vector2 ItemSpacing { get; set; }
    public Color TitlebarColor { get; set; }
    public Color TitlebarTextColor { get; set; }
    public Color TitlebarBorderColor { get; set; }
    public float TitlebarBorderThickness { get; set; }
}