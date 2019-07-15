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
        private TiledMap ActiveMap { get; set; }
        private GameMode Mode { get; set; }
        private MapLoadState LoadState { get; set; }
        private List<EntityCollisionSolid> MapCollisionMasks { get; set; }
        
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
            Content = Global.Content;

            MapContainer = new MapContainer();

            Player = new ActorPlayer(0, PlayerIndex.One, new Vector2(112, 104));
            Player.PlayerMoved += OnPlayerMove;

            MapRenderer = new TiledMapRenderer(Global.Graphics.GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Global.Window, Global.Graphics.GraphicsDevice, 256, 144);

            GameCamera = new Camera2D(viewportAdapter);
            GameCamera.Origin = new Vector2(0, 0);
            GameCamera.Position = new Vector2(0, 0);

            MapContainer = new MapContainer();
            ActiveMap = MapContainer.LoadMap("testStageRoom1");
            MapCollisionMasks = MapContainer.CollisionSolids;


            Mode = GameMode.Normal;
            LoadState = MapLoadState.Preparing;
        }

        public void Update(GameTime gameTime)
        {
            MapRenderer.Update(ActiveMap, gameTime);
            Player.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            var transformMatrix = GameCamera.GetViewMatrix();

            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            MapRenderer.Draw(ActiveMap, GameCamera.GetViewMatrix());
            Player.Draw(spriteBatch);

            spriteBatch.End();
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
        }
    }
}
