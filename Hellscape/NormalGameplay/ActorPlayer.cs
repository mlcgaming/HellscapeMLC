using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace Hellscape
{
    public class ActorPlayer
    {
        public event EventHandler PlayerMove;

        public int ID { get; protected set; }
        public PlayerIndex Controller { get; protected set; }
        public Texture2D Sprite { get; protected set; }
        public Vector2 Position { get; protected set; }
        private Vector2 Velocity { get; set; }
        private Vector2 FallVector { get; set; }
        public Rectangle CollisionMask { get; protected set; }
        public Rectangle ProposedMask { get; private set; }
        private ContentManager Content { get; set; }
        private float MoveSpeed { get; set; }
        private float WalkSpeed { get; set; }
        private float RunSpeed { get; set; }
        private List<Rectangle> Collisions { get; set; }
        private bool IsColliding { get; set; }
        private bool IsGrounded { get; set; }

        public ActorPlayer(int id, PlayerIndex controller, Vector2 position)
        {
            ID = id;
            Controller = controller;
            Position = position;
            Velocity = new Vector2(0);
            FallVector = new Vector2(0);
            Content = Global.Content;
            
            WalkSpeed = 32f;
            RunSpeed = 128f;
            MoveSpeed = WalkSpeed;

            Sprite = Content.Load<Texture2D>("GFX/Characters/MainCharacter");

            Collisions = new List<Rectangle>();
            IsColliding = false;
            IsGrounded = true;

            CreateCollisionMask(CollisionMask, 16, 24);
            CreateCollisionMask(ProposedMask, 16, 24);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (IsColliding == true)
            {
                for (int i = Collisions.Count - 1; i >= 0; i--)
                {
                    HandleCollision(Collisions.ElementAt(i));
                    Collisions.RemoveAt(i);
                }

                IsColliding = false;
            }

            ProcessInput(deltaTime);

            if(IsGrounded == false)
            {
                // Add Gravity
                FallVector = FallVector + new Vector2(0, Global.GRAVITY_RATE * deltaTime);

                Position = Position + FallVector;
            }
            
            Position = Position + Velocity;

            CreateCollisionMask(CollisionMask, 16, 24);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Sprite, Position, Color.White);
        }

        private void ProcessInput(float deltaTime)
        {
            Velocity = new Vector2(0, 0);
            float velocityRate = MoveSpeed * deltaTime;

            if(GamePad.GetState(Controller).IsConnected == true)
            {
                GamePadState gpState = GamePad.GetState(Controller);

                float adjustedMoveSpeed = (int)Math.Round(gpState.ThumbSticks.Left.X * velocityRate);
                Velocity = Velocity + new Vector2(adjustedMoveSpeed, 0);

                if(Velocity != new Vector2(0))
                {
                    OnPlayerMove();
                }

                if(IsGrounded == true && gpState.IsButtonDown(Buttons.A) == true)
                {
                    float jumpRate = WalkSpeed * deltaTime;
                    FallVector = new Vector2(0, -(int)Math.Round(jumpRate * 6f));
                    IsGrounded = false;
                    OnPlayerMove();
                }

                if(gpState.IsButtonDown(Buttons.X) == true)
                {
                    MoveSpeed = RunSpeed;
                }
                else
                {
                    MoveSpeed = WalkSpeed;
                }
            }
        }

        private void CreateCollisionMask(Rectangle rect, int width, int height)
        {
            rect = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }
        public void AddCollision(Rectangle collisionRectangle)
        {
            IsColliding = true;
            Collisions.Add(collisionRectangle);
        }
        private void HandleCollision(Rectangle collisionMask)
        {
            Vector2 adjustPosition = new Vector2(0, 0);
            float adjustedX, adjustedY;
            adjustedX = 0f;
            adjustedY = 0f;

            if(collisionMask.Width < collisionMask.Height)
            {
                // Process Collision HORIZONTALLY
                if (Position.X < collisionMask.X)
                {
                    adjustedX = -(collisionMask.Width);
                }
                else
                {
                    adjustedX = collisionMask.Width;
                }
            }
            else if (collisionMask.Width > collisionMask.Height)
            {
                // Process Collision VERTICALLY
                if (Position.Y + 12 < collisionMask.Y)
                {
                    adjustedY = -(collisionMask.Height);
                    IsGrounded = true;
                    FallVector = new Vector2(0);
                }
                else
                {
                    adjustedY = collisionMask.Height;
                }
            }
            else
            {
                // Process Collision HORIZONTALLY
                if (Position.X < collisionMask.X)
                {
                    adjustedX = -(collisionMask.Width);
                }
                else
                {
                    adjustedX = collisionMask.Width;
                }

                // Process Collision VERTICALLY
                if (Position.Y + 12 < collisionMask.Y)
                {
                    adjustedY = -(collisionMask.Height);
                    IsGrounded = true;
                    FallVector = new Vector2(0);
                }
                else
                {
                    adjustedY = collisionMask.Height;
                }
            }

            adjustPosition = new Vector2(adjustedX, adjustedY);
            Position = Position + adjustPosition;
            CreateCollisionMask(16, 24);
        }

        public void OnPlayerMove()
        {
            PlayerMove?.Invoke(this, EventArgs.Empty);
        }
    }
}
