using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using MonoGame.Extended;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System.Linq;

namespace Hellscape
{
    public class GameController
    {
        private ContentManager Content;
        private MapContainer MapContainer;
        private ActorPlayer Player;
        private Camera2D GameCamera;
        private BoxingViewportAdapter GameViewport;
        private TiledMapRenderer MapRenderer;

        private List<TileSceneObject> TileSceneObjects;
        private Texture2D PauseScreen;
        private SpriteFont PauseFont;

        private GameMode Mode;
        private MapLoadState LoadState;
        enum MapLoadState
        {
            Preparing,
            Loaded,
            Clearing,
            Debug
        }
        enum GameMode
        {
            Paused,
            Normal,
            Cutscene,
            Dialogue,
            Inventory,
            Debug
        }

        public GameController()
        {
            Initialize();
        }

        private void Initialize()
        {
            LoadState = MapLoadState.Preparing;
            Mode = GameMode.Paused;

            Content = Global.Content;

            NewGameSetup();

            MapContainer = new MapContainer();

            InputManager.StartPressed += OnStartPressed;

            Player = new ActorPlayer(0, PlayerIndex.One, new Vector2(32, 56));
            Player.PlayerMoved += OnPlayerMove;
            Player.PlayerInteracted += OnPlayerInteract;
            Player.Inventory.ItemUsed += OnPlayerItemUsed;

            MapRenderer = new TiledMapRenderer(Global.Graphics.GraphicsDevice);

            GameViewport = new BoxingViewportAdapter(Global.Window, Global.Graphics.GraphicsDevice, 256, 144);

            GameCamera = new Camera2D(GameViewport);
            GameCamera.Origin = new Vector2(0, 0);
            GameCamera.Position = new Vector2(0, 0);

            MapContainer = new MapContainer();
            MapContainer.MapLoaded += OnMapLoad;
            MapContainer.LoadMap("DebugRoom1");

            TileSceneObjects = new List<TileSceneObject>();

            PauseFont = Content.Load<SpriteFont>("GFX/Fonts/PauseFont");
            PauseScreen = Content.Load<Texture2D>("GFX/SinglePixel");
        }

        public void Update(GameTime gameTime)
        {
            switch (LoadState)
            {
                case MapLoadState.Preparing:
                    {

                        break;
                    }

                case MapLoadState.Loaded:
                    {
                        switch (Mode)
                        {
                            case GameMode.Normal:
                                {
                                    MapRenderer.Update(MapContainer.ActiveMap, gameTime);
                                    MapContainer.Update();

                                    if(TileSceneObjects.Count > 0)
                                    {
                                        foreach(TileSceneObject t in TileSceneObjects)
                                        {
                                            t.Update();
                                        }
                                    }

                                    Player.Update(gameTime);
                                    break;
                                }
                            case GameMode.Paused:
                                {
                                    InputManager.ProcessInputGamePad(PlayerIndex.One);
                                    break;
                                }
                        }
                        break;
                    }

                case MapLoadState.Clearing:
                    {

                        break;
                    }
                case MapLoadState.Debug:
                    {

                        break;
                    }
            }
        }
        public void Draw()
        {
            switch (LoadState)
            {
                case MapLoadState.Preparing:
                    {

                        break;
                    }

                case MapLoadState.Loaded:
                    {
                        var transformMatrix = GameCamera.GetViewMatrix();

                        switch (Mode)
                        {
                            case GameMode.Normal:
                                {
                                    Global.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

                                    MapRenderer.Draw(MapContainer.ActiveMap, GameCamera.GetViewMatrix());
                                    MapContainer.Draw();

                                    if (TileSceneObjects.Count > 0)
                                    {
                                        for(int i = TileSceneObjects.Count - 1; i >= 0; i--)
                                        {
                                            TileSceneObject obj = TileSceneObjects.ElementAt(i);
                                            obj.Draw();
                                            if(obj.IsDisposable == true)
                                            {
                                                TileSceneObjects.RemoveAt(i);
                                            }
                                        }
                                    }

                                    Player.Draw();

                                    Global.SpriteBatch.End();

                                    break;
                                }
                            case GameMode.Paused:
                                {
                                    Global.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

                                    MapRenderer.Draw(MapContainer.ActiveMap, GameCamera.GetViewMatrix());
                                    MapContainer.Draw();

                                    if (TileSceneObjects.Count > 0)
                                    {
                                        foreach (TileSceneObject t in TileSceneObjects)
                                        {
                                            t.Draw();
                                        }
                                    }

                                    Player.Draw();

                                    Vector2 pauseStringPosition = PauseFont.MeasureString("PAUSED");
                                    float pauseStringX = pauseStringPosition.X / 2;
                                    float pauseStringY = pauseStringPosition.Y / 2;
                                    pauseStringPosition = new Vector2(GameCamera.BoundingRectangle.Right / 2 - pauseStringX, GameCamera.BoundingRectangle.Bottom / 2 - pauseStringY);

                                    Global.SpriteBatch.Draw(PauseScreen, (Rectangle)GameCamera.BoundingRectangle, Color.Black * 0.5f);
                                    Global.SpriteBatch.DrawString(PauseFont, "PAUSED", pauseStringPosition, Color.White);

                                    Global.SpriteBatch.End();

                                    break;
                                }
                        }
                        break;
                    }

                case MapLoadState.Clearing:
                    {

                        break;
                    }
                case MapLoadState.Debug:
                    {

                        break;
                    }
            }

            
        }

        /// <summary>
        /// Copies SceneObject JSON files from Content folders to AppData local saves
        /// </summary>
        private void NewGameSetup()
        {
            string fileName;
            string sourcePath = Content.RootDirectory + "../../../../../../../../Hellscape/Hellscape/Content/Data/maps/SceneObjects";
            string targetPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/HellscapeDebug/save/001/";

            string[] files = System.IO.Directory.GetFiles(sourcePath);

            foreach(string s in files)
            {
                fileName = System.IO.Path.GetFileName(s);
                string destFile = System.IO.Path.Combine(targetPath, fileName);
                System.IO.File.Copy(s, destFile, true);
            }
        }

        private void OnStartPressed(object source, EventArgs args)
        {
            if(Mode == GameMode.Paused)
            {
                Mode = GameMode.Normal;
            }
            else
            {
                Mode = GameMode.Paused;
            }
        }
        public void OnPlayerMove(object source, EventArgs e)
        {
            // Check for Collisions against ProposedPosition on Player
            foreach (EntityCollisionSolid solid in MapContainer.CollisionSolids)
            {
                if (Player.CollisionMask.Intersects(solid.CollisionMask) == true)
                {
                    Rectangle _collision = Rectangle.Intersect(Player.CollisionMask, solid.CollisionMask);
                    Player.AddCollision(_collision);
                }
            }

            if(Player.IsGrounded == true)
            {
                Rectangle belowPlayerMask = new Rectangle(Player.CollisionMask.X, Player.CollisionMask.Bottom, Player.CollisionMask.Width, 1);

                foreach (EntityCollisionSolid solid in MapContainer.CollisionSolids)
                {
                    if (belowPlayerMask.Intersects(solid.CollisionMask) == true)
                    {
                        return;
                    }
                }

                Player.PlayerFalling();
            }
        }
        private void OnPlayerInteract(object source, EventArgs args)
        {
            foreach(TileEntitySceneObject obj in MapContainer.TileSceneObjects)
            {
                if (Player.CollisionMask.Intersects(obj.CollisionMask) == true)
                {
                    bool wasItemAdded = Player.Inventory.AddItem(obj);
                    if(wasItemAdded == true)
                    {
                        SoundEffect pickupSFX = Content.Load<SoundEffect>("Audio/SFX/Keys1");
                        pickupSFX.Play(0.75f, 0.5f, 0.5f);
                        MapContainer.RemoveTileSceneObject(obj);
                        return;
                    }
                }
            }

            foreach(TransitionHandler t in MapContainer.TransitionHandlers)
            {
                if(t.CollisionMask.Intersects(Player.CollisionMask) == true)
                {
                    if(t.IsKeyLocked == true)
                    {
                        foreach(InventoryItem i in Player.Inventory.Items)
                        {
                            if(i.Item.ShortName == t.Key.ShortName && i.Quantity >= t.KeyQuantity)
                            {
                                SoundEffect doorOpenSFX = Content.Load<SoundEffect>("Audio/SFX/TurningKey");
                                doorOpenSFX.Play(0.75f, 0.5f, 0.5f);

                                Player.Move(t.TransitionPosition);
                                MapContainer.LoadMap(t.TransitionMapID);
                                return;
                            }
                        }

                        SoundEffect doorLockSFX = Content.Load<SoundEffect>("Audio/SFX/lockeddoor");
                        doorLockSFX.Play(0.25f, 0.5f, 0.5f);

                        TileSceneObject doorLockIndicator = new TileSceneObject(new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(32,0)), new Vector2(t.Position.X, t.Position.Y - 16), new Vector2(0, -8));
                        TileSceneObjects.Add(doorLockIndicator);
                    }
                    else if(t.IsTriggerLocked == true)
                    {
                        if(Global.DoorTriggers[t.TriggerKey] == true)
                        {
                            SoundEffect doorOpenSFX = Content.Load<SoundEffect>("Audio/SFX/DoorOpen");
                            doorOpenSFX.Play(0.25f, 0.5f, 0.5f);

                            Player.Move(t.TransitionPosition);
                            MapContainer.LoadMap(t.TransitionMapID);
                            return;
                        }

                        SoundEffect doorLockSFX = Content.Load<SoundEffect>("Audio/SFX/lockeddoor");
                        doorLockSFX.Play(0.25f, 1.0f, 1.0f);

                        TileSceneObject doorLockIndicator = new TileSceneObject(new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(32, 0)), new Vector2(t.Position.X, t.Position.Y - 16), new Vector2(0, -8));
                        TileSceneObjects.Add(doorLockIndicator);
                    }
                    else
                    {
                        SoundEffect doorOpenSFX = Content.Load<SoundEffect>("Audio/SFX/DoorOpen");
                        doorOpenSFX.Play(0.25f, 0.5f, 0.5f);

                        Player.Move(t.TransitionPosition);
                        MapContainer.LoadMap(t.TransitionMapID);
                        return;
                    }
                    return;
                }
            }
        }
        private void OnPlayerItemUsed(object source, ItemUsedEventArgs args)
        {

        }
        public void OnMapLoad(object source, EventArgs e)
        {
            LoadState = MapLoadState.Loaded;
            Mode = GameMode.Normal;
        }
    }
}
