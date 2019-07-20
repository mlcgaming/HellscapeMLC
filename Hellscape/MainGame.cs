﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System;

namespace Hellscape
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        WindowState State;

        GameController GameController;

        public enum WindowState
        {
            MainMenu,
            NormalPlay
        }

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            if(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug") == false)
            {
                SetupAppData();
            }

            LoadContent();
            Global.InitializeDefaults(Content, spriteBatch, graphics, Window);
            InputManager.Initialize();

            graphics.PreferredBackBufferWidth = (int)Global.GLOBAL_OPT_WINDOW_SIZE.X;
            graphics.PreferredBackBufferHeight = (int)Global.GLOBAL_OPT_WINDOW_SIZE.Y;
            graphics.IsFullScreen = Global.GLOBAL_OPT_FULLSCREEN_ENABLED;
            graphics.ApplyChanges();

            GameController = new GameController();

            State = WindowState.NormalPlay;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Global.SetGameTime(gameTime);

            switch (State)
            {
                case WindowState.MainMenu:
                    {

                        break;
                    }
                case WindowState.NormalPlay:
                    {
                        GameController.Update(gameTime);
                        break;
                    }
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            switch (State)
            {
                case WindowState.MainMenu:
                    {

                        break;
                    }
                case WindowState.NormalPlay:
                    {
                        GameController.Draw();
                        break;
                    }
            }

            base.Draw(gameTime);
        }

        private void SetupAppData()
        {
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/bin");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/data");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/save");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/save/001");
        }
    }
}
