// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using TerraFX.Interop.DirectX;

namespace Suika.Data;

public readonly struct Texture(IntPtr handle, uint width, uint height)
{
    public readonly IntPtr Handle = handle;
    public readonly uint Width = width;
    public readonly uint Height = height;
}