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

namespace ZUI.FileSystem;

public class FileComponent : TableLayout {

    public string PathName {get; private set;}

    // A filename cannot contain any of the following characters: \ / : * ? " < > | 
    public FileComponent (Texture2D texture) : base(texture) {

    }

    public FileComponent SetPathName(string path) {
        this.Name = path; // TODO: this should be PathName instead
        return this;
    }
}