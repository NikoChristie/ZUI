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

public class TableLayout : Layout {

    private float PaddingVertial, PaddingHorizontal;

    public TableLayout(Texture2D texture) : base(texture) { }

    protected override Component ReArangeChildren() {

        // Resize & Place all children
        if(this.Children.Count > 0) {
            
            // How many children we will fit in each row/column
            float width = (float)Math.Ceiling(Math.Sqrt((this.Rectangle.Width / Rectangle.Height) * this.Children.Count));
            float height = (float)Math.Ceiling(Math.Sqrt((this.Rectangle.Height / this.Rectangle.Width) * this.Children.Count));

            // Find the largest child and give each child that much space
            float largestChildWidth = this.Children.Select(c => c.Rectangle.Width).Max();
            float largestChildHeight = this.Children.Select(c => c.Rectangle.Height).Max();

            Vector2 newChildSize = 
                new Vector2(
                    Math.Min(
                        this.Rectangle.Width  / (2 * this.PaddingHorizontal + ((this.PaddingHorizontal + largestChildWidth)  * width )),
                        this.Rectangle.Height / (2 * this.PaddingVertial + ((this.PaddingVertial + largestChildHeight) * height))
                    )
                );

            float newX = this.PaddingHorizontal * newChildSize.X;
            float newY = this.PaddingVertial * newChildSize.Y;

            int childIndex = 0;

            for(int row = 0; row < height; row++) {
                if(childIndex < this.Children.Count) {

                    Component currentChild = null;

                    for(int column = 0; column < width; column++) {

                        if(childIndex < this.Children.Count) {
                            currentChild = this.Children[childIndex];

                            currentChild.Position = new Vector2(newX, newY);
                            currentChild.Scale = newChildSize;
                            
                            newX += newChildSize.X * (largestChildWidth + this.PaddingHorizontal);
                            childIndex++;
                        }
                    }
                    newX = this.PaddingHorizontal * newChildSize.X;
                    newY += newChildSize.Y * (largestChildHeight + this.PaddingVertial);
                }
            }
        }

        return this;
    }

    public Component SetPadding(float newPaddingVertical, float newPaddingHorizontal) {
        this.PaddingVertial = newPaddingVertical;
        this.PaddingHorizontal = newPaddingHorizontal;
        return this;
    }
}