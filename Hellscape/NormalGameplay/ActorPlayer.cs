using System;
using System.Collections.Generic;
using System.Linq;
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
        public event EventHandler PlayerInteracted;

        public int ID { get; protected set; }
        public PlayerIndex Controller { get; protected set; }
        public Texture2D Sprite { get; protected set; }
        public Vector2 Position { get; protected set; }
        public Vector2 ProposedPosition { get; protected set; }
        private Vector2 Velocity { get; set; }
        private Vector2 FallVector { get; set; }
        public Rectangle CollisionMask { get; protected set; }
        private ContentManager Content;
        private float MoveSpeed;
        private float WalkSpeed;
        private float RunSpeed;
        private List<Rectangle> Collisions;
        private bool IsColliding;
        public bool IsGrounded { get; protected set; }
        private ActorFacing Facing;
        enum ActorFacing
        {
            Left,
            Right
        }

        private AnimationManager AnimationManager;
        private Dictionary<string, Animation> AnimationLibrary;

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

            Collisions = new List<Rectangle>();
            IsColliding = false;
            IsGrounded = true;

            CreateCollisionMask(Position, 16, 24);

            AnimationManager = new AnimationManager();
            AnimationLibrary = new Dictionary<string, Animation>();
            Facing = ActorFacing.Right;

            LoadContent();

            AnimationManager.Play(AnimationLibrary["idle"]);

            InputManager.LeftPressed += OnLeftPress;
            InputManager.RightPressed += OnRightPress;
            InputManager.JumpPressed += OnJumpPress;
            InputManager.RunPressed += OnRunPress;
            InputManager.RunReleased += OnRunRelease;
            InputManager.InteractPressed += OnInteractPress;
        }

        public void LoadContent()
        {
            Animation idle = new Animation(Content.Load<Texture2D>("GFX/Characters/MainCharacterTemplate"), 4, 24, 16, 0.075f, new Vector2(0, 0));
            Animation jump = new Animation(Content.Load<Texture2D>("GFX/Characters/MainCharacterTemplate"), 1, 24, 16, 0f, new Vector2(0, 48));
            AnimationLibrary.Add("idle", idle);
            AnimationLibrary.Add("jump", jump);
        }
        public void Update(GameTime gameTime)
        {
            Velocity = new Vector2(0, 0);
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            ProcessInput();

            if(IsGrounded == false)
            {
                // Add Gravity
                FallVector = FallVector + new Vector2(0, Global.GRAVITY_RATE * deltaTime);

                if (FallVector.Y > 4f)
                {
                    FallVector = new Vector2(0, 6f);
                }

                Velocity = Velocity + FallVector;
                AnimationManager.Play(AnimationLibrary["jump"]);
            }

            if (Velocity != new Vector2(0, 0))
            {
                ProposedPosition = Position + Velocity;
                CreateCollisionMask(ProposedPosition, 16, 24);
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
                CreateCollisionMask(Position, 16, 24);
            }

            AnimationManager.Update(gameTime);
        }
        public void Draw()
        {
            if(Facing == ActorFacing.Right)
            {
                AnimationManager.Draw(CollisionMask, SpriteEffects.None);
            }
            else
            {
                AnimationManager.Draw(CollisionMask, SpriteEffects.FlipHorizontally);
            }
            
            //spriteBatch.Draw(Sprite, Position, Color.White);
        }

        private void ProcessInput()
        {
            if(GamePad.GetState(PlayerIndex.One).IsConnected == true)
            {
                InputManager.ProcessInputGamePad(PlayerIndex.One);
            }
            else
            {
                InputManager.ProcessInputKeyboard();
            }

            if (Velocity.X > 0)
            {
                Facing = ActorFacing.Right;
            }
            else if (Velocity.X < 0)
            {
                Facing = ActorFacing.Left;
            }
        }

        private void CreateCollisionMask(Vector2 position, int width, int height)
        {
            CollisionMask = new Rectangle((int)position.X, (int)position.Y, width, height);
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
                    AnimationManager.Play(AnimationLibrary["idle"]);
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
        public void PlayerFalling()
        {
            IsGrounded = false;
        }
        public void Move(Vector2 position)
        {
            Position = position;
            CreateCollisionMask(Position, 16, 24);
        }

        private void OnLeftPress(object source, MoveInputEventArgs args)
        {
            float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
            float velocityRate = MoveSpeed * deltaTime;
            float adjustedMoveSpeed = (int)Math.Round(args.InputValue * velocityRate);
            Velocity = Velocity + new Vector2(adjustedMoveSpeed, 0);
        }
        private void OnRightPress(object source, MoveInputEventArgs args)
        {
            float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
            float velocityRate = MoveSpeed * deltaTime;
            float adjustedMoveSpeed = (int)Math.Round(args.InputValue * velocityRate);
            Velocity = Velocity + new Vector2(adjustedMoveSpeed, 0);
        }
        private void OnUpPress(object source, MoveInputEventArgs args)
        {

        }
        private void OnDownPress(object source, MoveInputEventArgs args)
        {

        }
        private void OnInteractPress(object source, EventArgs args)
        {
            PlayerInteracted?.Invoke(this, EventArgs.Empty);
        }
        private void OnJumpPress(object source, EventArgs args)
        {
            float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;

            if (IsGrounded == true)
            {
                float jumpRate = WalkSpeed * deltaTime;
                FallVector = new Vector2(0, -(int)Math.Round(jumpRate * 6f));
                IsGrounded = false;
                AnimationManager.Play(AnimationLibrary["jump"]);
            }
        }
        private void OnRunPress(object source, EventArgs args)
        {
            if(MoveSpeed < RunSpeed)
            {
                MoveSpeed = RunSpeed;
            }
        }
        private void OnRunRelease(object source, EventArgs args)
        {
            if (MoveSpeed == RunSpeed)
            {
                MoveSpeed = WalkSpeed;
            }
        }

        protected virtual void OnPlayerMoved()
        {
            PlayerMoved?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnPlayerInteracted()
        {
            PlayerInteracted?.Invoke(this, EventArgs.Empty);
        }
    }
}
