// Copyright (c) atomsk <baddobatsu@protonmail.com>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Suika.Components;

namespace Suika.Util;

internal static class ModalManager
{
    private static readonly LinkedList<Modal> modals = [];

    internal static void RegisterModal(Modal modal)
    {
        modals.AddLast(modal);
    }

    internal static void RenderModals()
    {
        foreach (var modal in modals)
        {
            modal.Render();
        }
    }
}