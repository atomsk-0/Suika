// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace Suika.Types.Interfaces;

public interface IBackend
{
    public bool Setup(IWindow windowInstance);
    public void Reset();
    public void Destroy();

    public void Render(Action renderAction);

    public nint LoadImageFromFile(string path);
    public nint LoadImageFromMemory(Stream stream);
}