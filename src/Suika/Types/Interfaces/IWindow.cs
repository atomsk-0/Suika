// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using System.Numerics;
using Suika.Data;

namespace Suika.Types.Interfaces;

public interface IWindow
{
    public void Create(in WindowOptions options);
    public void SetupImGui();
    public void Render();
    public void Destroy();

    public void Normalize();
    public void Maximize();
    public void Minimize();
    public void Activate();

    public void DragWindow();
    public void SimulateLeftMouseClick();

    public void Show();
    public void Hide();

    public float GetTitleBarPadding();
    public float GetTitleBarTopOffset();
    public float GetTitleBarHeight();
    public float GetCaptionButtonWidth();
    public RectF GetTitleBarRect();
    public Vector2 GetViewSize();

    public bool IsMaximized();

    public void SetTitle(string title);
}