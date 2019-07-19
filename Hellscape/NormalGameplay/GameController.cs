using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;

namespace Hellscape
{
    public class GameController
    {
        private ContentManager Content { get; set; }
        private MapContainer MapContainer { get; set; }
        private ActorPlayer Player { get; set; }
        private Camera2D GameCamera { get; set; }
        private BoxingViewportAdapter GameViewport { get; set; }
        private TiledMapRenderer MapRenderer { get; set; }
        //private TiledMap ActiveMap { get; set; }
        private List<TileEntitySceneObject> SceneObjects { get; set; }
        private GameMode Mode { get; set; }
        private MapLoadState LoadState { get; set; }
        
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

            MapContainer = new MapContainer();

            Player = new ActorPlayer(0, PlayerIndex.One, new Vector2(112, 104));
            Player.PlayerMoved += OnPlayerMove;

            MapRenderer = new TiledMapRenderer(Global.Graphics.GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Global.Window, Global.Graphics.GraphicsDevice, 256, 144);

            GameCamera = new Camera2D(viewportAdapter);
            GameCamera.Origin = new Vector2(0, 0);
            GameCamera.Position = new Vector2(0, 0);

            SceneObjects = new List<TileEntitySceneObject>();

            MapContainer = new MapContainer();
            MapContainer.MapLoaded += OnMapLoad;
            MapContainer.LoadMap("DebugRoom1");
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
                                    Player.Update(gameTime);
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

                        Global.SpriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

                        MapRenderer.Draw(MapContainer.ActiveMap, GameCamera.GetViewMatrix());
                        MapContainer.Draw();
                        Player.Draw();

                        Global.SpriteBatch.End();
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
        public void OnMapLoad(object source, EventArgs e)
        {
            LoadState = MapLoadState.Loaded;
            Mode = GameMode.Normal;
        }
    }
}
