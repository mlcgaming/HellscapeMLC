using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System.Linq;

namespace Hellscape
{
    public class GameController
    {
        public event EventHandler GameExited;

        private ContentManager Content;
        private MapContainer MapContainer;
        private ActorPlayer Player;
        private Camera2D GameCamera;
        private BoxingViewportAdapter GameViewport;
        private TiledMapRenderer MapRenderer;

        private List<TileSceneObject> TileSceneObjects;
        private Texture2D PauseScreen;
        private BitmapFont PauseFont;

        private Song BGM;

        private GameMode Mode;
        private MapLoadState LoadState;
        enum MapLoadState
        {
            Preparing,
            FadeIn,
            Loaded,
            FadeOut,
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
            InputManager.SelectPressed += OnSelectPressed;

            Player = new ActorPlayer(0, PlayerIndex.One, new Vector2(224, 192));
            Player.PlayerMoved += OnPlayerMove;
            Player.PlayerInteracted += OnPlayerInteract;
            Player.Inventory.ItemUsed += OnPlayerItemUsed;

            MapRenderer = new TiledMapRenderer(Global.Graphics.GraphicsDevice);

            GameViewport = new BoxingViewportAdapter(Global.Window, Global.Graphics.GraphicsDevice, 512, 288, 148, 140);

            GameCamera = new Camera2D(GameViewport);
            GameCamera.Origin = new Vector2(0, 0);
            GameCamera.Position = new Vector2(0, 0);

            MapContainer = new MapContainer();
            MapContainer.MapLoaded += OnMapLoad;
            MapContainer.LoadMap("DebugRoom3");

            TileSceneObjects = new List<TileSceneObject>();

            PauseFont = Content.Load<BitmapFont>("GFX/Fonts/PauseFont");
            PauseScreen = Content.Load<Texture2D>("GFX/SinglePixel");

            BGM = Content.Load<Song>("Audio/BGM/MansionFirstFloor");
            MediaPlayer.Volume = 0.15f;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Play(BGM);
        }

        public void Update()
        {
            switch (LoadState)
            {
                case MapLoadState.Preparing:
                    {

                        break;
                    }
                case MapLoadState.FadeIn:
                    {

                        break;
                    }
                case MapLoadState.Loaded:
                    {
                        switch (Mode)
                        {
                            case GameMode.Normal:
                                {
                                    MapRenderer.Update(MapContainer.ActiveMap, Global.GameTime);
                                    MapContainer.Update();

                                    if(TileSceneObjects.Count > 0)
                                    {
                                        for (int i = TileSceneObjects.Count - 1; i >= 0; i--)
                                        {
                                            TileSceneObject obj = TileSceneObjects.ElementAt(i);
                                            obj.Update();
                                            if (obj.IsDisposable == true)
                                            {
                                                TileSceneObjects.RemoveAt(i);
                                            }
                                        }
                                    }

                                    foreach(TransitionHandler th in MapContainer.TransitionHandlers)
                                    {
                                        if(th.IsInteractive == false)
                                        {
                                            th.Update();
                                        }
                                    }

                                    Player.Update();

                                    AdjustCamera();

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
                case MapLoadState.FadeOut:
                    {

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

                                    MapRenderer.Draw(MapContainer.ActiveMap, transformMatrix);
                                    MapContainer.Draw();

                                    if (TileSceneObjects.Count > 0)
                                    {
                                        foreach (TileSceneObject t in TileSceneObjects)
                                        {
                                            t.Draw();
                                        }
                                    }

                                    Player.Draw();

                                    Global.SpriteBatch.End();

                                    break;
                                }
                            case GameMode.Paused:
                                {
                                    Global.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

                                    MapRenderer.Draw(MapContainer.ActiveMap, transformMatrix);
                                    MapContainer.Draw();

                                    if (TileSceneObjects.Count > 0)
                                    {
                                        foreach (TileSceneObject t in TileSceneObjects)
                                        {
                                            t.Draw();
                                        }
                                    }

                                    Player.Draw();

                                    Vector2 pausePosition = Common.AdjustText(new Vector2(GameCamera.BoundingRectangle.Width / 2 + GameCamera.Position.X, GameCamera.BoundingRectangle.Height / 2 +  GameCamera.Position.Y), PauseFont, "PAUSED", Common.TextHalign.Center, Common.TextValign.Middle);
                                    
                                    Global.SpriteBatch.Draw(PauseScreen, (Rectangle)GameCamera.BoundingRectangle, Color.Black * 0.5f);
                                    Global.SpriteBatch.DrawString(PauseFont, "PAUSED", pausePosition, Color.White);

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

        private void AdjustCamera()
        {
            Vector2 _adjustPosition;

            int cameraX = Player.CollisionMask.Center.X - (GameViewport.BoundingRectangle.Width / 2);
            int cameraY = Player.CollisionMask.Center.Y - (GameViewport.BoundingRectangle.Height / 2);

            _adjustPosition = new Vector2(cameraX, cameraY);
            GameCamera.Position = _adjustPosition;

            if (GameCamera.BoundingRectangle.Bottom > MapContainer.BoundingBox.Height)
            {
                cameraX = (int)GameCamera.Position.X;
                cameraY = MapContainer.BoundingBox.Height - (int)GameCamera.BoundingRectangle.Height;
                _adjustPosition = new Vector2(cameraX, cameraY);
                GameCamera.Position = _adjustPosition;
            }
            else if (GameCamera.BoundingRectangle.Top < 0)
            {
                cameraX = (int)GameCamera.Position.X;
                cameraY = 0;
                _adjustPosition = new Vector2(cameraX, cameraY);
                GameCamera.Position = _adjustPosition;
            }

            if (GameCamera.BoundingRectangle.Right > MapContainer.BoundingBox.Width)
            {
                cameraX = MapContainer.BoundingBox.Width - (int)GameCamera.BoundingRectangle.Width;
                cameraY = (int)GameCamera.Position.Y;
                _adjustPosition = new Vector2(cameraX, cameraY);
                GameCamera.Position = _adjustPosition;
            }
            else if (GameCamera.BoundingRectangle.Left < 0)
            {
                cameraX = 0;
                cameraY = (int)GameCamera.Position.Y;
                _adjustPosition = new Vector2(cameraX, cameraY);
                GameCamera.Position = _adjustPosition;
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
        private void OnSelectPressed(object source, EventArgs args)
        {
            if(Mode == GameMode.Paused)
            {
                OnGameExit();
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

            if(MapContainer.TransitionHandlers.Count > 0)
            {
                foreach (TransitionHandler th in MapContainer.TransitionHandlers)
                {
                    if (th.IsSilent == true)
                    {
                        if (Player.CollisionMask.Intersects(th.CollisionMask) == true)
                        {
                            MapContainer.LoadMap(th.TransitionMapID);
                            Player.Move(th.TransitionPosition);
                            return;
                        }
                    }
                }
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
                if(t.IsInteractive == true)
                {
                    if (t.CollisionMask.Intersects(Player.CollisionMask) == true)
                    {
                        if (t.IsKeyLocked == true)
                        {
                            foreach (InventoryItem i in Player.Inventory.Items)
                            {
                                if (i.Item.ShortName == t.Key.ShortName && i.Quantity >= t.KeyQuantity)
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

                            TileSceneObject doorLockIndicator = new TileSceneObject(new Animation(Content.Load<Texture2D>("GFX/Tiles/SceneObjects"), 1, 16, 16, 0f, new Vector2(32, 0)), new Vector2(t.Position.X, t.Position.Y - 16), new Vector2(0, -8));
                            TileSceneObjects.Add(doorLockIndicator);

                            t.StartInteraction();
                        }
                        else if (t.IsTriggerLocked == true)
                        {
                            if (Global.DoorTriggers[t.TriggerKey] == true)
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

                            t.StartInteraction();
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
        }
        private void OnPlayerItemUsed(object source, ItemUsedEventArgs args)
        {

        }
        public void OnMapLoad(object source, EventArgs e)
        {
            LoadState = MapLoadState.Loaded;
            Mode = GameMode.Normal;
        }
        public void OnGameExit()
        {
            GameExited?.Invoke(null, EventArgs.Empty);
        }
    }
}
