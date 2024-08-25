// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Numerics;
using Suika.Data;

namespace Suika.Types.Interfaces;

public interface IWindow
{
    public void Create(in AppOptions appOptions);
    public void Render();
    public void Destroy();

    public void Maximize();
    public void Restore();
    public void Minimize();
    public void Show();
    public void Hide();

    public float GetTitleBarPadding();
    public float GetTitleBarTopOffset();
    public float GetTitleBarHeight();
    public float GetCaptionButtonWidth();
    public RectF GetTitleBarRect();
    public Vector2 GetViewSize();

    public void Activate();
    public void DragWindow();

    public bool IsMaximized();
    public nint GetHandle();

    public Action<int, int>? OnResize { get; set; }
    public Action? View { get; set; }
}