using Microsoft.Xna.Framework;
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

        MainMenuController MainMenu;
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

            MainMenu = new MainMenuController();

            MainMenu.GameStarted += OnGameStarted;
            MainMenu.GameExited += OnGameExited;

            State = WindowState.MainMenu;

            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }
        protected override void Update(GameTime gameTime)
        {
            Global.SetGameTime(gameTime);

            switch (State)
            {
                case WindowState.MainMenu:
                    {
                        MainMenu.Update();
                        break;
                    }
                case WindowState.NormalPlay:
                    {
                        GameController.Update();
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
                        MainMenu.Draw();
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
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/Data");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/Save");
            Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/Save/001");
        }

        private void OnGameStarted(object source, EventArgs args)
        {
            GameController = new GameController();
            State = WindowState.NormalPlay;
            GameController.GameExited += OnGameExited;

            MainMenu.Dispose();
            MainMenu.GameStarted -= OnGameStarted;
            MainMenu = null;
        }
        private void OnGameExited(object source, EventArgs args)
        {
            Exit();
        }
    }
}
