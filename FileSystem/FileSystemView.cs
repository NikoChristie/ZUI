using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended.SceneGraphs;
using MonoGame.Extended.Sprites;

namespace ZUI.FileSystem;

public class FileSystemView : TableLayout {

    public string Root { get; private set;}
    private FileComponent RootComponent;
    private FileSystemWatcher watcher;

    public bool IncludeHiddenFiles = false;

    public FileSystemView(Texture2D texture) : base(texture) {

    }

    public FileSystemView SetRootDirectory(string root) {
        // Update Watcher and rebuild file view
        this.Root = root;
        this.RootComponent = GenerateFileTree(root);
        this.AttachChild(this.RootComponent);

        // TODO: add all that cool watcher stuff
        this.watcher = new FileSystemWatcher(root);
        /*
        this.watcher.NotifyFilter = NotifyFilters.DirectoryName 
                                  | NotifyFilters.FileName;
        */

        // TODO: find out wich of these are necessary
        this.watcher.NotifyFilter = NotifyFilters.Attributes
                                  | NotifyFilters.CreationTime
                                  | NotifyFilters.DirectoryName
                                  | NotifyFilters.FileName
                                  | NotifyFilters.LastAccess
                                  | NotifyFilters.LastWrite
                                  | NotifyFilters.Security
                                  | NotifyFilters.Size;

        this.watcher.Created += this.OnCreated;
        this.watcher.Deleted += this.OnDeleted;
        this.watcher.Renamed += this.OnRenamed;

        this.watcher.IncludeSubdirectories = true;
        this.watcher.EnableRaisingEvents = true;

        return this;
    }

    private FileComponent GenerateFileTree(string path) {
        
        Console.WriteLine(path);

        FileComponent current;

        if(Directory.Exists(path)) {
            
            current = (FileComponent)new FileComponent(this.Texture)
                .SetPathName(path)
                .SetPadding(7f, 7f);

            try {
                string[] children = 
                    Directory
                        .GetFileSystemEntries(path, "*", SearchOption.TopDirectoryOnly)
                        .Where(f => !((File.GetAttributes(f) & FileAttributes.Hidden) == FileAttributes.Hidden)) // TODO: add conditional
                        .ToArray();

                if(children.Length > 0) {
                    current.AttachChildren(children.Select(c => (Component)GenerateFileTree(c)).ToList());
                }
            }
            catch(UnauthorizedAccessException) {

            }
            catch(DirectoryNotFoundException) {

            }
        }
        else {
            current = new FileComponent(this.Texture).SetPathName(path);
        }

        return current;
    }

    /*
    OnChange  -> Nothing?
    OnCreated -> Append
    OnDeleted -> Remove
    OnRenamed -> Change Name
    */

    // !!! Important that directories **don't** end in '/' !!!
    private void OnCreated(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"Created {e.FullPath} (check if this ends in an '/' if its a folder)");

        string path = e.FullPath;
        string parent = Path.GetDirectoryName(e.FullPath);
        Console.WriteLine($"{path}'s parent is {parent}");

        if(parent != null) {
            Debug.Assert(this.RootComponent.GetComponentByName(parent) != null, $"RootComponents has no child component named \"{parent}\"");
            this.RootComponent
                .GetComponentByName(parent)
                .AttachChild(
                    new FileComponent(this.Texture).SetName(path)
                );
        }
    }

    private void OnDeleted(object sender, FileSystemEventArgs e) {
        Console.WriteLine($"Deleted {e.FullPath}");

        string path = e.FullPath;
        string parent = Path.GetDirectoryName(e.FullPath);
        Console.WriteLine($"{path}'s parent is {parent}");

        if(parent != null) {
            Debug.Assert(this.RootComponent.GetComponentByName(parent) != null, $"RootComponents has no child component named \"{parent}\"");
            this.RootComponent
                .GetComponentByName(parent)
                .DetachChild(this.RootComponent.GetComponentByName(path));
        }
    }


    private void OnRenamed(object sender, RenamedEventArgs e) {
        Console.WriteLine($"Renamed {e.OldFullPath} to {e.FullPath}");

        string path = e.OldFullPath;
        string newPath = e.FullPath;

        Component component = this.RootComponent.GetComponentByName(path);

        if(component != null) {
            component.SetName(newPath);
        }

            //.SetName(newPath);
    }
}