using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;
using MonoGame.Extended.ViewportAdapters;
using System.Collections.Generic;

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

        TiledMap ActiveMap;
        TiledMapRenderer MapRenderer;
        Camera2D MapCamera;
        ActorPlayer Player;
        MapContainer MapContainer;

        List<EntityCollisionSolid> MapCollisionMasks;

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
            Global.InitializeDefaults(Content, spriteBatch, graphics, Window);

            graphics.PreferredBackBufferWidth = (int)Global.GLOBAL_OPT_WINDOW_SIZE.X;
            graphics.PreferredBackBufferHeight = (int)Global.GLOBAL_OPT_WINDOW_SIZE.Y;
            graphics.IsFullScreen = Global.GLOBAL_OPT_FULLSCREEN_ENABLED;
            graphics.ApplyChanges();

            State = WindowState.NormalPlay;

            MapRenderer = new TiledMapRenderer(GraphicsDevice);

            var viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 256, 144);

            MapCamera = new Camera2D(viewportAdapter);
            MapCamera.Origin = new Vector2(0, 0);
            MapCamera.Position = new Vector2(0, 0);

            Player = new ActorPlayer(0, PlayerIndex.One, new Vector2(112, 104));

            MapContainer = new MapContainer();
            ActiveMap = MapContainer.LoadMap("testStageRoom1");
            MapCollisionMasks = MapContainer.CollisionSolids;

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

            switch (State)
            {
                case WindowState.MainMenu:
                    {

                        break;
                    }
                case WindowState.NormalPlay:
                    {
                        MapRenderer.Update(ActiveMap, gameTime);
                        Player.Update(gameTime);

                        foreach(EntityCollisionSolid ecs in MapCollisionMasks)
                        {
                            if(Player.CollisionMask.Intersects(ecs.CollisionMask) == true)
                            {
                                Rectangle _collisionRectangle = Rectangle.Intersect(Player.CollisionMask, ecs.CollisionMask);
                                Player.AddCollision(_collisionRectangle);
                            }
                        }

                        break;
                    }
            }

            base.Update(gameTime);
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            var transformMatrix = MapCamera.GetViewMatrix();

            spriteBatch.Begin(transformMatrix: transformMatrix, samplerState: SamplerState.PointClamp);

            switch (State)
            {
                case WindowState.MainMenu:
                    {

                        break;
                    }
                case WindowState.NormalPlay:
                    {
                        MapRenderer.Draw(ActiveMap, MapCamera.GetViewMatrix());
                        Player.Draw(spriteBatch);
                        break;
                    }
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
