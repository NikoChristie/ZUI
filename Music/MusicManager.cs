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

public class MusicManager {

    private List<Artist> Artists;

    public MusicManager(string path) {

        this.Artists = new List<Artist>();
        foreach(string song in Directory.EnumerateFiles(path, "*.mp3", SearchOption.AllDirectories)) {
            try {
                var tagFile = TagLib.File.Create(song);
                Console.WriteLine($"{tagFile.Tag.Title}");
                Console.WriteLine($"{tagFile.Tag.FirstAlbumArtist ?? tagFile.Tag.FirstPerformer}");
                CreateSong(tagFile.Tag);
            }
            catch (CorruptFileException) {

            }
        }
    }

    private void CreateSong(Tag tag) {
        this.GetOrCreateArtist(tag.FirstAlbumArtist ?? tag.FirstPerformer ?? "Unkown Artist").AddSong(tag);
    }

    private Artist GetOrCreateArtist(string artistName) {
        Artist artist = this.Artists.Where(a => a.Name.Equals(artistName)).FirstOrDefault();
        if(artist is null) {
            artist = new Artist(artistName);
            this.Artists.Add(artist);
        }
        return artist;
    }

    public Component ToComponent(Texture2D texture) {
        Component artistLayout = new TableLayout(texture).SetPadding(10f, 10f);

        foreach(Artist artist in this.Artists) {
            Console.WriteLine($"Artist {artist.Name}");
            artistLayout.AttachChild(artist.ToComponent(texture).SetName(artist.Name));
        }

        return artistLayout;
    }
}