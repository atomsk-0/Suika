// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Drawing;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.Windows.Windows;

namespace Suika.Platforms.Windows;

#pragma warning disable CA1416


internal static class Platform
{
    /// <summary>
    /// Get the primary monitor screen size
    /// </summary>
    /// <returns>Primary monitor screen size</returns>
    internal static Size GetScreenSize()
    {
        return new Size(GetSystemMetrics(SM.SM_CXSCREEN), GetSystemMetrics(SM.SM_CYSCREEN));
    }
}