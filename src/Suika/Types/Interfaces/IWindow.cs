// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika.Data;

namespace Suika.Types.Interfaces;

public interface IWindow
{
    public void Create(in AppOptions appOptions);
    public void Render();
    public void Destroy();
}