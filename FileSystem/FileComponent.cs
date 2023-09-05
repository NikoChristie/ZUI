using System;
using System.Linq;
using System.IO;
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

namespace ZUI.FileSystem;

// A View into a folder that updates lazily
public class FileComponent : FileSystemComponent {

    public override string Name {
        get {
            return this._Name;
        }
        set {
            this._Name = value;
            if(this.watcher is not null) {
                this.watcher.Path = new FileInfo(this.Path).Directory.FullName;
            }
        }
    }

    public FileComponent(Texture2D texture, string name) : base(texture, name) { }

    public override Component AttachToParent(Component parent) {
        base.AttachToParent(parent);

        this.watcher = new FileSystemWatcher((this.ComponentParent as FileSystemComponent).Path, this.Name); // FIXME: IDK if it should be name or path
        this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

        //this.watcher.Created += new FileSystemEventHandler(OnCreated);
        this.watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        this.watcher.Renamed += new RenamedEventHandler(OnRenamed);
        this.watcher.Changed += new FileSystemEventHandler(OnChanged);
        this.watcher.Created += new FileSystemEventHandler(OnCreated);

        this.watcher.EnableRaisingEvents = true;

        return this;
    }

    protected override void OnRenamed(object sender, RenamedEventArgs e) {
        //Console.WriteLine($"Sender {(sender as FileSystemWatcher).Path}");
        Console.WriteLine($"FILE RENAME {e.OldFullPath} -> {e.FullPath}");
        this.Name = e.Name;
    }

    protected override void OnDeleted(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"FILE DELETE {e.FullPath}");
        this.ComponentParent.DetachChild(this);
        (sender as FileSystemWatcher).Dispose();
    }

    protected void OnCreated(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"FILE CREATE {e.FullPath} THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN ");
        //this.AttachChild(new FolderComponent(this.Texture, e.Name).SetPadding(FolderComponent.Padding, FolderComponent.Padding));
    }

    protected void OnChanged(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"{this.Path} -> {e.FullPath} was *{e.ChangeType.ToString()}*");
    }
}