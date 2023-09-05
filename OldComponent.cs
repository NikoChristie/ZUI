using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;

namespace ZUI;

public class OldComponent {
    protected SceneNode sceneNode;
    public Color Color {get; private set;}

    protected const float MinimumChildDisplayScale = 0.1f;

    public OldComponent(Texture2D texture) {
        this.Color = new Color(Random.Shared.Next(0, 255), Random.Shared.Next(0, 255), Random.Shared.Next(0, 255));
        this.sceneNode = new SceneNode(this.GetHashCode().ToString());
        
        Sprite sprite = new Sprite(texture);
        sprite.Color = this.Color;

        this.sceneNode.Entities.Add(new SpriteEntity(sprite));
    }

    protected SceneNodeCollection Children {
        get {
            return this.sceneNode.Children;
        }
    }

    public RectangleF Rectangle {
        get {
            //return this.sceneNode.BoundingRectangle;
            return this.sceneNode.Entities[0].BoundingRectangle;
        }
    }

    public Vector2 Position {
        get {
            return this.sceneNode.Position;
        }
        set {
            this.sceneNode.Position = value;
        }
    }
    public Vector2 Scale {
        get {
            return this.sceneNode.Scale;
        }
        set {
            this.sceneNode.Scale = value;
        }
    }

    public virtual OldComponent AttachChild(OldComponent child) {
        this.sceneNode.Children.Add(child.sceneNode);
        return this;
    }

    public OldComponent AttachToParent(OldComponent parent) {
        parent.AttachChild(this);
        return this;
    }

    public OldComponent SetPosition(Vector2 newPosition) {
        this.Position = newPosition;
        return this;
    }
    public OldComponent SetPosition(float newX, float newY) {
        return SetPosition(new Vector2(newX, newY));
    }

    public OldComponent SetScale(float newScale) {
        this.Scale = new Vector2(newScale);
        return this;
    }

    public void Draw(SpriteBatch spriteBatch) {
        Rectangle screenSize = new Rectangle(0, 0, spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Width, spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Height);

        // Don't draw if too small
        if(this.Scale.X > OldComponent.MinimumChildDisplayScale || this.Scale.Y > OldComponent.MinimumChildDisplayScale) {
            
            if(screenSize.Intersects(((Microsoft.Xna.Framework.Rectangle)this.Rectangle))) { // Only draw Components that are on-screen
                // Normal Drawing Technique
                spriteBatch.Begin();
                //spriteBatch.Draw(this.sceneNode.Entities[0], this.Position, null, this.Color, 0.0f, Vector2.Zero, this.Scale, SpriteEffects.None, 1.0f);
                this.sceneNode.Draw(spriteBatch);
                spriteBatch.End();

                /*
                // Matrix Transform Drawing
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, this.Transformation);
                spriteBatch.Draw(Texture, Vector2.Zero, this.Color);
                spriteBatch.End();
                */

                foreach(var child in this.Children) {
                    child.Draw(spriteBatch);
                }
            }
        }
    }
}