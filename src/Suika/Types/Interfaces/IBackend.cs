// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika.Data;

namespace Suika.Types.Interfaces;

public interface IBackend
{
    public bool Setup(IWindow windowInstance, in AppOptions appOptions);
    public void Reset();
    public void Destroy();

    public void Render(Action renderAction);

    public nint LoadTextureFromFile(string path);
    public nint LoadTextureFromMemory(Stream stream);
}