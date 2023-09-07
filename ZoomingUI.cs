﻿using System;
using System.Linq;
using System.IO;
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

namespace ZUI;

public class ZoomingUI : Game {
    private GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch;
    private Texture2D spaceHolder;
    private Texture2D placeHolder;
    private Texture2D wideload;
    private Texture2D longload;
    private Texture2D folder;
    
    private int lastMouseScroll = 0;
    private Point lastMousePosition;

    public static GraphicsDevice GlobalGraphicsDevice;

    private Component Root;
    /*
    private Component Column;
    private Component Row;
    private Component Table;
    */

    private Music.MusicManager musicManager;

    public static Rectangle GetScreenSize(SpriteBatch spriteBatch) {
        return new Rectangle(0, 0, spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Width, spriteBatch.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
    }

    public ZoomingUI() {
        graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        Window.AllowUserResizing = true;
    }

    protected override void Initialize() {
        base.Initialize();
    }


    protected override void LoadContent() {
        spriteBatch = new SpriteBatch(GraphicsDevice);
        ZoomingUI.GlobalGraphicsDevice = this.GraphicsDevice;

        this.placeHolder = Content.Load<Texture2D>("PlaceHolder");
        this.spaceHolder = Content.Load<Texture2D>("SpaceHolder");
        this.wideload = Content.Load<Texture2D>("WideLoad");
        this.longload = Content.Load<Texture2D>("LongLoad");
        this.folder = Content.Load<Texture2D>("Folder");

        //this.musicManager = new Music.MusicManager(@"E:\Downloads\Music\");
        ZUI.FileSystem.FileSystemView fileSystemManager = new ZUI.FileSystem.FileSystemView(this.folder);
        // !!! Its important that directory names dont end in '\'
        //fileSystemManager.SetRootDirectory("C:\\Users\\12044");
        fileSystemManager.SetRootDirectory("C:\\Users\\12044\\Documents"); 
    


        //this.musicManager = new Music.MusicManager(@"E:\Downloads\");


        this.Root = 
            new TableLayout(this.placeHolder)
                .AttachChild(fileSystemManager);
                //.AttachChild(musicManager.ToComponent(this.placeHolder));
                //.AttachChild(new FileSystem.FolderComponent(this.spaceHolder, @"C:\Users\nikol\ZUI\SandBox\Root").SetPadding(10f, 10f));
                //.AttachChild(new FolderComponent(this.placeHolder, @"C:\"));
                //.AttachChild(GenerateFileTree(@"E:\"));
                //.AttachChild(GenerateFileTree(@"C:\");
                //.AttachChild(GenerateFileTree(@"C:\Users\nikol\"));
                //.AttachChild(GenerateFileTree(@"C:\Users\nikol\.cargo\"));
                //.AttachChild(GenerateFileTree(@"C:\Users\12044\Desktop\Portfolio\ZUI\SandBox\"));
    }
   
    protected override void Update(GameTime gameTime) {
        if(this.IsActive) {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                Exit();
            }

            float offsetX = 0f;
            float offsetY = 0f;

            MouseState mouseState = Mouse.GetState();

            if(mouseState.RightButton == ButtonState.Pressed) {
                offsetX += mouseState.X - lastMousePosition.X;
                offsetY += mouseState.Y - lastMousePosition.Y;
            }

            if(mouseState.LeftButton == ButtonState.Pressed) {
                Console.WriteLine(this.Root.GetComponentAt(mouseState.X, mouseState.Y)?.ToString());
            }

            // Zoom
            float scale = 1.0f + (-0.1f * Math.Sign(lastMouseScroll - mouseState.ScrollWheelValue));

            // Calculate Offset so that we zoom on the Mouse 
            Vector2 zoomOffset = new Vector2(mouseState.X - (mouseState.X * scale), mouseState.Y - (mouseState.Y * scale));
            offsetX += zoomOffset.X;
            offsetY += zoomOffset.Y;

            // Appply Transformations to all Components, this will cascade into all of their children
            this.Root.Scale *= scale;
            this.Root.Position = (scale * this.Root.Position) + new Vector2(offsetX, offsetY);

            // Reset Scale
            if(Keyboard.GetState().IsKeyDown(Keys.Home)) {
                //this.Root.Scale = new Vector2(1f, 1f) ;
            }

            lastMousePosition = mouseState.Position;
            lastMouseScroll = mouseState.ScrollWheelValue;
        }

        /*
        if(mouseState.MiddleButton == ButtonState.Pressed){
            switch(Random.Shared.Next(0, 3)) {
                case 0:
                    this.Column.AttachChild(new Component(this.placeHolder));
                    this.Row.AttachChild(new Component(this.placeHolder));
                    this.Table.AttachChild(new Component(this.placeHolder));
                    break;
                case 1:
                    this.Column.AttachChild(new Component(this.wideload));
                    this.Row.AttachChild(new Component(this.wideload));
                    this.Table.AttachChild(new Component(this.wideload));
                    break;
                case 2:
                    this.Column.AttachChild(new Component(this.longload));
                    this.Row.AttachChild(new Component(this.longload));
                    this.Table.AttachChild(new Component(this.longload));
                    break;
            }
        }
        */

        this.Root.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime) {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        this.Root.Draw(this.spriteBatch);

        Debug.WriteLine(this.graphics.GraphicsDevice.Metrics.DrawCount);

        base.Draw(gameTime);
    }
}