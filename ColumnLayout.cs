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

public class ColumnLayout : Layout {

    private float Padding = 0f;
    public ColumnLayout(Texture2D texture) : base(texture) { }

    protected override Component ReArangeChildren() {

        // FIXME: Wide Load Clips with others
        // Resize & Place all children
        float largestChildHeight = this.Children.Select(c => c.Rectangle.Height).Max();
        Vector2 newChildSize = new Vector2(this.Rectangle.Height / (this.Padding + ((this.Padding + largestChildHeight) * this.Children.Count)));

        float newX = this.Rectangle.Width / 2;
        float newY = this.Padding * newChildSize.Y;

        for(int i = 0; i < this.Children.Count; i++) {
            Component currentChild = this.Children[i];

            currentChild.Position = new Vector2(newX, newY);
            currentChild.Scale = newChildSize;

            newY += newChildSize.Y * (largestChildHeight + this.Padding);
        }

        return this;
    }

    public Component SetPadding(float newPadding) {
        this.Padding = newPadding;
        return this;
    }
}