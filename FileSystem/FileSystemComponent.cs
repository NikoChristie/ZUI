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
public abstract class FileSystemComponent : TableLayout {

    protected const float Padding = 10f;
    public string Path {
        get {
            if(this.ComponentParent is FolderComponent) {
                return @$"{(this.ComponentParent as FolderComponent).Path}\{this.Name}";
            }
            return @$"{this.Name}";
        }
    }

    protected string _Name;

    public virtual string Name {
        get {
            return this._Name;
        }
        set {
            this._Name = value;
            if(this.watcher is not null) {
                this.watcher.Path = this.Path;
            }
        }
    }

    protected FileSystemWatcher watcher;

    public FileSystemComponent(Texture2D texture, string name) : base(texture) { 
        this.Name = name;
    }

    /*
    public override Component AttachChild(Component child) {
        base.AttachChild(child);

        this.watcher = new FileSystemWatcher(this.Path, this.Name);
        this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

        this.watcher.Created += new FileSystemEventHandler(OnCreated);
        this.watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        this.watcher.Renamed += new RenamedEventHandler(OnRenamed);
        this.watcher.Changed += new FileSystemEventHandler(OnChanged);

        return this;
    }
    */

    protected abstract void OnRenamed(object sender, RenamedEventArgs e);
    protected abstract void OnDeleted(object sender, FileSystemEventArgs e);
    //protected abstract void OnChanged(object sender, RenamedEventArgs e);

    /*
    protected virtual void OnRenamed(object sender, RenamedEventArgs e) {
        //Console.WriteLine($"Sender {(sender as FileSystemWatcher).Path}");
        Console.WriteLine($"RENAME {e.OldFullPath} -> {e.FullPath}");
        this.Name = e.Name;
    }

    protected virtual void OnDeleted(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"DELETE {e.FullPath}");
        this.ComponentParent.DetachChild(this);
        (sender as FileSystemWatcher).Dispose();
    }

    //protected abstract void OnCreated(object sender, FileSystemEventArgs e);

    /*
    protected virtual void OnCreated(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"CREATE {e.FullPath} THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN THIS SHOULDN'T HAPPEN ");
        //this.AttachChild(new FolderComponent(this.Texture, e.Name).SetPadding(FolderComponent.Padding, FolderComponent.Padding));
    }
    */

    /*
    protected virtual void OnChanged(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"{this.Path} -> {e.FullPath} was *{e.ChangeType.ToString()}*");
    }
    */
}