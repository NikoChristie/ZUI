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
/*
public class FolderComponent : TableLayout {

    private const float Padding = 10f;

    public string Path {
        get {
            if(this.ComponentParent is FolderComponent) {
                return @$"{(this.ComponentParent as FolderComponent).Path}\{this.Name}";
            }
            return @$"{this.Name}";
        }
    }

    private string _Name;

    public string Name {
        get {
            return this._Name;
        }
        private set {
            this._Name = value;
            if(this.watcher is not null) {
                this.watcher.Path = this.Path;
            }
        }
    }

    private FileSystemWatcher watcher;

    public FolderComponent(Texture2D texture, string name) : base(texture) {
        this.Name = name;
    }

    private void OnRenamed(object sender, RenamedEventArgs e) {
        Console.WriteLine($"Sender {(sender as FileSystemWatcher).Path}");
        Console.WriteLine($"RENAME {e.OldFullPath} -> {e.FullPath}");
        FolderComponent renamedFile = (this.Children.Where(c => (c as FolderComponent).Path == e.OldFullPath).FirstOrDefault() as FolderComponent);
        if(renamedFile is not null) {
            Console.WriteLine($"Sucessfully renamed {renamedFile.Name} to {e.Name}");
            renamedFile.Name = e.Name;
        }
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"DELETE {e.FullPath}");
        FolderComponent deletedChild = (this.Children.Where(c => (c as FolderComponent).Path == e.FullPath).FirstOrDefault() as FolderComponent);
        if(deletedChild is not null) {
            this.DetachChild(deletedChild);
        }
        (sender as FileSystemWatcher).Dispose();
    }

    private void OnCreated(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"CREATE {e.FullPath}");
        if(Directory.Exists(e.FullPath)) {
            this.AttachChild(new FolderComponent(this.Texture, e.Name).SetPadding(FolderComponent.Padding, FolderComponent.Padding));
        }
        else {
            this.AttachChild(new FileComponent(this.Texture, e.Name).SetPadding(FolderComponent.Padding, FolderComponent.Padding));
        }
    }

    private void OnChanged(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"{this.Path} -> {e.FullPath} was *{e.ChangeType.ToString()}*");
    }

    public override Component AttachToParent(Component parent) {
        base.AttachToParent(parent);

        Console.WriteLine($"Changing Watcher to {this.Path}");

        string filter = Directory.Exists(this.Path) ? "*" : this.Path;
        string path = File.Exists(this.Path) ? (this.ComponentParent as FolderComponent).Path : this.Path;

        this.watcher = new FileSystemWatcher(path, filter);

        this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

        /*
        this.watcher.NotifyFilter = NotifyFilters.Attributes
                                  | NotifyFilters.CreationTime
                                  | NotifyFilters.DirectoryName
                                  | NotifyFilters.FileName
                                  | NotifyFilters.LastAccess
                                  | NotifyFilters.LastWrite
                                  | NotifyFilters.Security
                                  | NotifyFilters.Size;
        /

        this.watcher.Created += new FileSystemEventHandler(OnCreated);
        this.watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        this.watcher.Renamed += new RenamedEventHandler(OnRenamed);
        this.watcher.Changed += new FileSystemEventHandler(OnChanged);

        /*
        this.watcher.Filter = this.Path;

        if(Directory.Exists(this.Path)) {
            this.watcher.Filter = "*";
        }

        //this.watcher.Filter = File.Exists(this.Path) ? this.Path : "*";
        if(Directory.Exists(this.Path)) {
            Console.WriteLine($"{this.Path} is a directory");
            this.watcher.Filter = "*";
        }
        else if(File.Exists(this.Path)){
            Console.WriteLine($"{this.Path} is a file");
            //this.watcher.Filter = Path.GetFileName(this.Path);
            this.watcher.Filter = System.IO.Path.GetFileName(this.Path);
        }
        else {
            Console.WriteLine("Uh oh");
        }
        this.watcher.Path = this.Path;
        /
        this.watcher.IncludeSubdirectories = false;
        this.watcher.EnableRaisingEvents = true;

        /*
        if(this.ComponentParent is FolderComponent) {
            (this.ComponentParent as FolderComponent).watcher.EnableRaisingEvents = false;
            //(this.ComponentParent as FolderComponent).watcher.EnableRaisingEvents = true;
        }
        /

        return this;
    }

    public override string ToString(){
        return $"{this.Path} @ depth {this.Depth}";
    }
}
*/

public class FolderComponent : FileSystemComponent {
    public FolderComponent(Texture2D texture, string name) : base(texture, name) { }

    public override Component AttachToParent(Component parent) {
        base.AttachToParent(parent);

        this.watcher = new FileSystemWatcher(this.Path, "*");
        this.watcher.NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName;

        //this.watcher.Created += new FileSystemEventHandler(OnCreated);
        this.watcher.Deleted += new FileSystemEventHandler(OnDeleted);
        this.watcher.Renamed += new RenamedEventHandler(OnRenamed);
        //this.watcher.Changed += new FileSystemEventHandler(OnChanged);
        this.watcher.Created += new FileSystemEventHandler(OnCreated);

        this.watcher.EnableRaisingEvents = true;

        return this;
    }

    protected void OnCreated(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"FOLDER CREATE {e.FullPath}");
        if(Directory.Exists(e.FullPath)) {
            this.AttachChild(new FolderComponent(this.Texture, e.Name).SetPadding(FolderComponent.Padding, FolderComponent.Padding));
        }
        else {
            this.AttachChild(new FileComponent(this.Texture, e.Name).SetPadding(FolderComponent.Padding, FolderComponent.Padding));
        }
    }
    protected override void OnDeleted(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"FOLDER DELETE {e.FullPath}");
        FileSystemComponent deletedChild = (this.Children.Where(c => (c as FileSystemComponent).Path == e.FullPath).FirstOrDefault() as FileSystemComponent);
        if(deletedChild is not null) {
            this.DetachChild(deletedChild);
        }
        (sender as FileSystemWatcher).Dispose();
    }

    protected override void OnRenamed(object sender, RenamedEventArgs e) {
        Console.WriteLine($"Sender {(sender as FileSystemWatcher).Path}");
        Console.WriteLine($"FOLDER RENAME {e.OldFullPath} -> {e.FullPath}");
        FileSystemComponent renamedFile = (this.Children.Where(c => (c as FileSystemComponent).Path == e.OldFullPath).FirstOrDefault() as FileSystemComponent);
        if(renamedFile is not null) {
            Console.WriteLine($"Sucessfully renamed {renamedFile.Name} to {e.Name}");
            renamedFile.Name = e.Name;
        }
    }
}