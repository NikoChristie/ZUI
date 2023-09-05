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

public abstract class Layout : Component {
    public Layout(Texture2D texture) : base(texture) { }
    protected abstract Component ReArangeChildren();

    public override Component AttachChild(Component child) {
        base.AttachChild(child);

        this.ReArangeChildren();

        return this;
    }
    protected virtual Component AttachChildWithoutResize(Component child) {
        base.AttachChild(child);
        return this;
    }

    public override Component DetachChild(Component child){
        base.DetachChild(child);
        this.ReArangeChildren();
        return this;
    }

    // Optimization for attaching muliple children so we only resize once
    public override Component AttachChildren(List<Component> children) {
        foreach(Component child in children) {
            this.AttachChildWithoutResize(child);
        }

        this.ReArangeChildren();

        return this;
    }
}