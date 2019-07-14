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

namespace Hellscape.NormalGameplay
{
    public class GameController
    {
        private event EventHandler PlayerMoveProposal;

        private ContentManager Content { get; set; }
        private MapContainer MapContainer { get; set; }
        private ActorPlayer Player { get; set; }
        private Camera2D GameCamera { get; set; }
        private BoxingViewportAdapter GameViewport { get; set; }
        private TiledMapRenderer MapRenderer { get; set; }
        private TiledMap ActiveMap { get; set; }
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
            Content = Global.Content;

            MapContainer = new MapContainer();
            Player = new ActorPlayer(0, PlayerIndex.One, new Vector2(0, 0));

            Player.PlayerMove += PlayerMoveProposal;

            Mode = GameMode.Normal;
            LoadState = MapLoadState.Preparing;
        }

        public void Update(GameTime gameTime)
        {
            Player.Update(gameTime);

            foreach(EntityCollisionSolid solid in MapContainer.CollisionSolids)
            {
                if(Player.CollisionMask.Intersects(solid.CollisionMask) == true)
                {
                    Rectangle _collision = Rectangle.Intersect(Player.CollisionMask, solid.CollisionMask);
                    Player.AddCollision(_collision);
                }
            }
        }
        public void Draw()
        {
            
        }

        public void OnPlayerMove()
        {
            // Check for Collisions against ProposedPosition on Player

        }
    }
}
