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
    //private SpriteFont SpriteFont;
    private BitmapFont Font;
    public string Root { get; private set;}
    private FileComponent RootComponent;
    private FileSystemWatcher watcher;
    public bool IncludeHiddenFiles = false;

    private Texture2D FolderTexture;
    private Texture2D FileTexture;
    public FileSystemView(Texture2D texture, Texture2D fileTexture, Texture2D folderTexture, BitmapFont font) : base(texture) {
        //this.SpriteFont = spriteFont;
        this.Font = font;
        this.FolderTexture = folderTexture;
        this.FileTexture = fileTexture;
    }

    public const string FileExplorerExecutable = "explorer.exe";

    public FileSystemView SetRootDirectory(string root) {
        // Update Watcher and rebuild file view
        this.Root = root;
        this.RootComponent = GenerateFileTree(root);
        this.AttachChild(this.RootComponent);

        this.watcher = new FileSystemWatcher(root);

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

        Console.WriteLine(GetFileComponentFromPath(@"C:\Users\12044\Documents\Streets of Rogue\CloudData"));

        return this;
    }

    public FileComponent GetFileComponentFromPath(string path) {
        // "/path/to/file/"
        //      --> path
        //            \
        //            to
        //            /
        //          file

        FileComponent target = null;

        Debug.Assert(path.StartsWith(this.Root));
        if (path.StartsWith(this.Root)) {
            path = path.Replace(this.Root, ""); // path.Skip(this.Root.Length).ToString(); // remove root from path 
            Console.WriteLine(path);
        }

        return target;

    }

    private FileComponent GenerateFileTree(string path) {

        // Console.WriteLine(path);
        Debug.WriteLine(path);

        FileComponent current;

        if(Directory.Exists(path)) {
            
            current = (FileComponent)new FileComponent(this.FolderTexture, this.Font)
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
            current = new FileComponent(this.FileTexture, this.Font).SetPathName(path);
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
                    new FileComponent(this.Texture, this.Font).SetName(path)
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

        // component?.SetName(newPath);
    }
}