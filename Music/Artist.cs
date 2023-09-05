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

public class Artist {

    public readonly string Name;
    private List<Album> Albums;

    public Artist(string Name) {
        this.Name = Name;
        this.Albums = new List<Album>();
    }

    public Song AddSong(Tag tag) {
        return this.GetOrCreateAlbum(tag.Album ?? "Unkown Album").AddSong(tag);
    }

    public Album GetAlbum(string albumName) {
        return this.Albums.Where(a => a.Name.Equals(albumName)).FirstOrDefault();
    }

    private Album GetOrCreateAlbum(string albumName) {
        Album album = this.GetAlbum(albumName);
        if(album is null) {
            album = new Album(albumName);
            this.Albums.Add(album);
        }
        return album;
    }

    public Component ToComponent(Texture2D texture) {
        Component albumLayout = new TableLayout(texture);

        foreach(Album album in this.Albums) {
            Console.WriteLine($"\tAlbum {album.Name}");
            albumLayout.AttachChild(album.ToComponent(texture).SetName(album.Name));
        }

        return albumLayout;
    }
}