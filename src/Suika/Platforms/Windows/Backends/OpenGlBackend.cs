// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika.Types.Interfaces;

namespace Suika.Platforms.Windows.Backends;

public class OpenGlBackend : IBackend
{
    public bool Setup(IWindow window)
    {
        throw new NotImplementedException();
    }


    public void Reset()
    {
        throw new NotImplementedException();
    }


    public void Destroy()
    {
        throw new NotImplementedException();
    }


    public void Render(Action renderAction)
    {
        throw new NotImplementedException();
    }


    public IntPtr LoadImageFromFile(string path)
    {
        throw new NotImplementedException();
    }


    public IntPtr LoadImageFromMemory(Stream stream)
    {
        throw new NotImplementedException();
    }
}