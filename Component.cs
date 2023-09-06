using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;
using System.Diagnostics;

using ZUI;

public class Component : Transform2 {

    // TODO: Add padding to end

    public string Name {get; protected set;}

    public Color Color {get; private set;}
    //protected const float MinimumChildDisplayScale = 0.1f;
    protected float MinimumChildDisplayScale {
        get {
            return Math.Min(1 / this.Texture.Width, 1 / this.Texture.Height);
        }
    }
    protected const int MaximumDepth = 3;
    protected Texture2D Texture;

    protected Component ComponentParent;
    protected List<Component> Children = new List<Component>();

    public int Depth {
        get {
            return 1 + (this.ComponentParent?.Depth ?? 0);
        }
    }

    public Component(Texture2D texture) {
        this.Color = new Color(Random.Shared.Next(0, 255), Random.Shared.Next(0, 255), Random.Shared.Next(0, 255));
        this.Texture = texture;
        this.Name = this.GetHashCode().ToString();
    }

    public RectangleF Rectangle {
        get {
            return new RectangleF(this.WorldPosition, new Point(this.Texture.Width, this.Texture.Height)); // FIXME: Might have to scale idk
        }
    }

    public Rectangle Bounds {
        get {
            return new Rectangle(this.WorldPosition.ToPoint(), 
                (this.WorldScale * new Vector2(this.Texture.Width, this.Texture.Height)).ToPoint());
        }
    }

    public virtual Component AttachToParent(Component parent) {
        this.ComponentParent = parent;
        this.Parent = (BaseTransform<Matrix2>)parent;
        parent.Children.Add(this);
        return this;
    }

    public virtual Component AttachChild(Component child) {
        child.AttachToParent(this);
        return this;
    }

    public virtual Component AttachChildren(List<Component> children) {

        foreach(Component child in children) {
            this.AttachChild(child);
        }

        return this;
    }

    public virtual Component DetachChild(Component child) {
        child.ComponentParent = null;
        child.Parent = (BaseTransform<Matrix2>)null;
        Debug.Assert(this.Children.Remove(child));
        return this;
    }

    /*
    public virtual void DetachFromParent() {
        Debug.Assert(this.ComponentParent.Children.Any(x => x == this));
        this.ComponentParent.DetachChild(this);
    }
    */

    public Component SetPosition(Vector2 newPosition) {
        this.Position = newPosition;
        return this;
    }
    public Component SetPosition(float newX, float newY) {
        return SetPosition(new Vector2(newX, newY));
    }

    public Component SetScale(float newScale) {
        this.Scale = new Vector2(newScale);
        return this;
    }

    public bool InView(SpriteBatch spriteBatch) {
        return 
            (this.WorldScale.X > this.MinimumChildDisplayScale || this.WorldScale.Y > this.MinimumChildDisplayScale) &&
            //this.Depth <= Component.MaximumDepth &&
            ZoomingUI.GetScreenSize(spriteBatch).Intersects(this.Bounds);        
    }

    public virtual void Draw(SpriteBatch spriteBatch) {
        /*
        Rectangle screenSize = new Rectangle(0, 0, spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Width, spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Height);

        if(this.WorldScale.X > Component.MinimumChildDisplayScale || this.WorldScale.Y > Component.MinimumChildDisplayScale) {
            if(screenSize.Intersects(this.Bounds)) {
                // Normal Drawing Technique
                spriteBatch.Begin();
                spriteBatch.Draw(this.Texture, this.WorldPosition, null, this.Color, this.WorldRotation, Vector2.One, this.WorldScale, SpriteEffects.None, 1f);
                spriteBatch.End();

                foreach(var child in this.Children) {
                    child.Draw(spriteBatch);
                }
            }
        }
        */
        if(this.InView(spriteBatch)) {
            spriteBatch.Begin();
            spriteBatch.Draw(this.Texture, this.WorldPosition, null, this.Color, this.WorldRotation, Vector2.One, this.WorldScale, SpriteEffects.None, 1f);
            spriteBatch.End();

            foreach(var child in this.Children) {
                child.Draw(spriteBatch);
            }
        }
    }

    public virtual Component Update() {

        foreach(Component child in this.Children){
            child.Update();
        }

        return this;
    }

    public Component GetComponentAt(float x, float y) {
        if(this.Bounds.Contains(new Point((int)x, (int)y))) {
            List<Component> inChildren = this.Children.Select(c => c.GetComponentAt(x, y)).Where(c => c != null).ToList();
            return inChildren.FirstOrDefault() ?? this;
        }
        return null;
    }

    public Component GetComponentByName(string name) {
        if(this.Name == name) {
            return this;
        }

        foreach(Component child in this.Children) {
            if(child.Name == name) {
                return child;
            }
            else {
                Component next = child.GetComponentByName(name);
                if(next != null) {
                    return next;
                }
            }
        }
        return null;
    }

    public override string ToString() {
        return this.Name;
    }

    public Component SetName(string newName) {
        this.Name = newName;
        return this;
    }
}