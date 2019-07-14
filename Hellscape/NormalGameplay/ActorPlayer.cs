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
        public delegate void PlayerMoveEventHandler(object source, EventArgs e);
        public event PlayerMoveEventHandler PlayerMoved;

        public int ID { get; protected set; }
        public PlayerIndex Controller { get; protected set; }
        public Texture2D Sprite { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 ProposedPosition { get; protected set; }
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
            ProposedPosition = position;
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

            CreateCollisionMask(16, 24);
            CreateCollisionMask(16, 24);
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            ProcessInput(deltaTime);

            if(IsGrounded == false)
            {
                // Add Gravity
                FallVector = FallVector + new Vector2(0, Global.GRAVITY_RATE * deltaTime);

                if (FallVector.Y > 4f)
                {
                    FallVector = new Vector2(0, 6f);
                }

                Velocity = Velocity + FallVector;
            }

            if (Velocity != new Vector2(0, 0))
            {
                ProposedPosition = Position + Velocity;
                CreateProposalMask(16, 24);
                OnPlayerMoved();

                if (IsColliding == true)
                {
                    for (int i = Collisions.Count - 1; i >= 0; i--)
                    {
                        HandleCollision(Collisions.ElementAt(i));
                        Collisions.RemoveAt(i);
                    }

                    IsColliding = false;
                }

                Position = ProposedPosition;
                Velocity = new Vector2(0);
                CreateCollisionMask(16, 24);
            }
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

                if(IsGrounded == true && gpState.IsButtonDown(Buttons.A) == true)
                {
                    float jumpRate = WalkSpeed * deltaTime;
                    FallVector = new Vector2(0, -(int)Math.Round(jumpRate * 6f));
                    IsGrounded = false;
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

        private void CreateCollisionMask(int width, int height)
        {
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }
        private void CreateProposalMask(int width, int height)
        {
            ProposedMask = new Rectangle((int)ProposedPosition.X, (int)ProposedPosition.Y, width, height);
        }
        public void AddCollision(Rectangle collisionRectangle)
        {
            foreach(Rectangle r in Collisions)
            {
                if(r == collisionRectangle)
                {
                    return;
                }
            }

            IsColliding = true;
            Collisions.Add(collisionRectangle);
        }
        private void HandleCollision(Rectangle collisionMask)
        {
            Vector2 adjustPosition = new Vector2(0, 0);
            int adjustedX, adjustedY;
            adjustedX = 0;
            adjustedY = 0;

            if(collisionMask.Width < collisionMask.Height)
            {
                // Process Collision HORIZONTALLY
                if (ProposedPosition.X + 8 < collisionMask.Left)
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
                if (ProposedPosition.Y + 12 < collisionMask.Y)
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
                if (ProposedPosition.X < collisionMask.Left)
                {
                    adjustedX = -(collisionMask.Width);
                }
                else
                {
                    adjustedX = collisionMask.Width;
                }

                // Process Collision VERTICALLY
                if (ProposedPosition.Y + 12 < collisionMask.Y)
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
            ProposedPosition = ProposedPosition + adjustPosition;
        }

        protected virtual void OnPlayerMoved()
        {
            PlayerMoved?.Invoke(this, EventArgs.Empty);
        }
    }
}
