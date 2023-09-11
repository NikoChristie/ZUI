using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;
using System.Linq;
using System.Threading;

namespace ZUI.FileSystem;

public class FileComponent : TableLayout {

    public string PathName {get; private set;}
    private BitmapFont Font;

    // A filename cannot contain any of the following characters: \ / : * ? " < > | 
    public FileComponent (Texture2D texture, BitmapFont font) : base(texture) {
        this.Font = font;
    }

    public FileComponent SetPathName(string path) {
        this.Name = path; // TODO: this should be PathName instead
        return this;
    }

    private const float TextFadeRange = 10f;

    public override void Draw(SpriteBatch spriteBatch) {
        if(this.InView(spriteBatch)) {
            base.Draw(spriteBatch);
            
            if(this.Children.Count == 0 || (this.WorldScale.X < TextFadeRange || this.WorldScale.Y < TextFadeRange)) { // Draw if in range or last element
                string fileName = 
                    File.Exists(this.Name) ?
                    Path.GetFileName(this.Name) :
                    new DirectoryInfo(this.Name).Name;

                // Calculate Text Scale
                spriteBatch.Begin();

                Vector2 size = new(this.Bounds.Size.X / this.Font.MeasureString(fileName).Width);

                spriteBatch.DrawString(
                    this.Font,
                    fileName,
                    this.WorldPosition,
                    Color.Black,
                    0f,
                    Vector2.One,
                    size,
                    SpriteEffects.None,
                    1f);
                spriteBatch.End();
            }
        }
    }
}