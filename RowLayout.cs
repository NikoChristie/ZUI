using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;

namespace ZUI;

public class RowLayout : Layout {

    private float Padding = 0f;

    public RowLayout(Texture2D texture) : base(texture) { }

    // FIXME: Wide Load Clips with others
    protected override Component ReArangeChildren() {
        // Resize & Place all children
        float largestChildWidth = this.Children.Select(c => c.Rectangle.Width).Max();
        Vector2 newChildSize = new Vector2(this.Rectangle.Width / (this.Padding + ((this.Padding + largestChildWidth) * this.Children.Count)));

        float newX = this.Padding * newChildSize.X;
        float newY = this.Rectangle.Height / 2;

        for(int i = 0; i < this.Children.Count; i++) {
            Component currentChild = this.Children[i];

            currentChild.Position = new Vector2(newX, newY);
            currentChild.Scale = newChildSize;

            newX += newChildSize.X * (largestChildWidth + this.Padding);
        }

        return this;
    }

    public Component SetPadding(float newPadding) {
        this.Padding = newPadding;
        return this;
    }
}