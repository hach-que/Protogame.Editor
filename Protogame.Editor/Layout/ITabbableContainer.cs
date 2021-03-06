﻿using Protogame;

namespace Protogame.Editor.Layout
{
    public interface ITabbableContainer : IContainer
    {
        string Title { get; set; }

        IAssetReference<TextureAsset> Icon { get; set; }

        bool Visible { get; set; }

        bool Enabled { get; set; }
    }
}