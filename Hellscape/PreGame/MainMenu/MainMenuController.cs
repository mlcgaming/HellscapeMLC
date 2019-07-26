using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.ViewportAdapters;

namespace Hellscape
{
    public class MainMenuController
    {
        public EventHandler GameStarted;
        public EventHandler GameExited;

        private Texture2D MainMenuTexture;
        private List<MainMenuItem> Items;
        private int SelectedItem, LastItem;
        private float InputTimer;

        private BoxingViewportAdapter GameViewport;
        private Camera2D GameCamera;

        public MainMenuController()
        {
            MainMenuTexture = Global.Content.Load<Texture2D>("GFX/MainMenuTest");
            Items = new List<MainMenuItem>();
            SelectedItem = 0;
            InputTimer = 0.25f;

            InputManager.UpPressed += OnUpPressed;
            InputManager.DownPressed += OnDownPressed;
            InputManager.VerticalReleased += OnVerticalReleased;

            GameViewport = new BoxingViewportAdapter(Global.Window, Global.Graphics, 512, 288, 74, 70);

            GameCamera = new Camera2D(GameViewport);
            GameCamera.Origin = new Vector2(0, 0);
            GameCamera.Position = new Vector2(0, 0);

            Initialize();

            LastItem = Items.Count - 1;
        }

        private void Initialize()
        {
            BitmapFont menuItemFont = Global.Content.Load<BitmapFont>("GFX/Fonts/MainMenuItemFont");
            Vector2 measureString = menuItemFont.MeasureString("|");

            MainMenuItem startGame = new MainMenuItem("Start Game", new Vector2(GameViewport.VirtualWidth / 2, GameViewport.VirtualHeight / 2));
            MainMenuItem networkGame = new MainMenuItem("Multiplayer", new Vector2(GameViewport.VirtualWidth / 2, GameViewport.VirtualHeight / 2 + (measureString.Y * 1)));
            MainMenuItem optionsMenu = new MainMenuItem("Options", new Vector2(GameViewport.VirtualWidth / 2, GameViewport.VirtualHeight / 2 + (measureString.Y * 2)));
            MainMenuItem exitGame = new MainMenuItem("Quit Game", new Vector2(GameViewport.VirtualWidth / 2, GameViewport.VirtualHeight / 2 + (measureString.Y * 3)));

            startGame.ItemSelected += OnItemSelected;
            networkGame.ItemSelected += OnItemSelected;
            optionsMenu.ItemSelected += OnItemSelected;
            exitGame.ItemSelected += OnItemSelected;

            Items.Add(startGame);
            Items.Add(networkGame);
            Items.Add(optionsMenu);
            Items.Add(exitGame);

            Items.ElementAt(SelectedItem).Select();
        }
        public void Update()
        {
            float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;

            if(InputTimer > 0)
            {
                InputTimer -= deltaTime;
            }

            InputManager.ProcessInputGamePad(PlayerIndex.One);
        }
        public void Draw()
        {
            var transformMatrix = GameCamera.GetViewMatrix();
            Global.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            Global.SpriteBatch.Draw(MainMenuTexture, new Rectangle(0, 0, GameViewport.VirtualWidth, GameViewport.VirtualHeight), Color.White);

            foreach(MainMenuItem item in Items)
            {
                item.Draw();
            }

            Global.SpriteBatch.End();
        }
        public void Dispose()
        {
            for(int i = LastItem; i >= 0; i--)
            {
                Items.RemoveAt(i);
            }

            GameCamera = null;
            GameViewport = null;

            InputManager.DownPressed -= OnDownPressed;
            InputManager.UpPressed -= OnUpPressed;
            InputManager.VerticalReleased -= OnVerticalReleased;
        }

        private void OnDownPressed(object source, MoveInputEventArgs args)
        {
            if(InputTimer <= 0)
            {
                if(SelectedItem == LastItem)
                {
                    SelectedItem = 0;
                    Items.ElementAt(LastItem).Deselect();
                    Items.ElementAt(SelectedItem).Select();
                }
                else
                {
                    Items.ElementAt(SelectedItem).Deselect();
                    SelectedItem += 1;
                    Items.ElementAt(SelectedItem).Select();
                }

                InputTimer = 0.25f;
            }
        }
        private void OnUpPressed(object source, MoveInputEventArgs args)
        {
            if (InputTimer <= 0)
            {
                if (SelectedItem == 0)
                {
                    SelectedItem = LastItem;
                    Items.ElementAt(0).Deselect();
                    Items.ElementAt(SelectedItem).Select();
                }
                else
                {
                    Items.ElementAt(SelectedItem).Deselect();
                    SelectedItem -= 1;
                    Items.ElementAt(SelectedItem).Select();
                }

                InputTimer = 0.25f;
            }
        }
        private void OnVerticalReleased(object source, EventArgs args)
        {
            InputTimer = 0f;
        }
        private void OnItemSelected(object source, ItemSelectedEventArgs args)
        {
            switch (args.ItemName)
            {
                case "Start Game":
                    {
                        OnGameStarted();
                        break;
                    }
                case "Multiplayer":
                    {

                        break;
                    }
                case "Options":
                    {

                        break;
                    }
                case "Quit Game":
                    {
                        OnGameExit();
                        break;
                    }
            }
        }

        private void OnGameStarted()
        {
            GameStarted?.Invoke(null, EventArgs.Empty);
        }
        private void OnGameExit()
        {
            GameExited?.Invoke(null, EventArgs.Empty);
        }
    }
}
