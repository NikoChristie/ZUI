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

public class Album {
    public readonly string Name;
    private List<Song> Songs;

    private Texture2D texture;

    public Album(string name) {
        this.Name = name;
        this.Songs = new List<Song>();
    }

    public Song AddSong(Tag tag) {
        Song song = new Song(tag);
        this.Songs.Add(song);

        IPicture picture = tag.Pictures.FirstOrDefault();
        if(picture is not null) {
            MemoryStream stream = new MemoryStream(picture.Data.Data);
            this.texture = Texture2D.FromStream(ZoomingUI.GlobalGraphicsDevice, stream);
        }

        return song;
    }

    public Song GetSong(string songName) {
        return this.Songs.Where(s => s.Name.Equals(songName)).FirstOrDefault();
    }
    public Component ToComponent(Texture2D texture) {

        texture = this.texture ?? texture;

        Component songLayout = new TableLayout(texture).SetPadding(10f, 10f);

        foreach(Song song in this.Songs) {
            Console.WriteLine($"\t\tSong {song.Name}");
            songLayout.AttachChild(new Component(texture).SetName(song.Name));
        }

        return songLayout;
    }
}