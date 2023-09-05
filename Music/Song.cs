using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;
using TagLib;

namespace ZUI.Music;

public class Song {

    public readonly Tag Tag;
    public readonly string Name;

    private Texture2D texture;

    public Song(Tag tag) {
        this.Tag = tag;
        this.Name = tag.Title ?? "Unkown Song";
    }
}