using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace Hellscape
{
    public class PlayerClimbedEventArgs
    {
        public EntityLadder Ladder;
    }
    public class ActorPlayer
    {
        public delegate void PlayerMoveEventHandler(object source, EventArgs e);
        public delegate void PlayerClimbEventHandler(object source, PlayerClimbedEventArgs args);
        public event PlayerMoveEventHandler PlayerMoved;
        public event EventHandler PlayerInteracted;
        public event PlayerClimbEventHandler PlayerClimbed;

        public int ID { get; }
        public PlayerIndex Controller { get; private set; }
        public Texture2D Sprite { get; }
        public Vector2 Position { get; private set; }
        public Vector2 ProposedPosition { get; private set; }
        private Vector2 Velocity { get; set; }
        private Vector2 FallVector { get; set; }
        public Rectangle CollisionMask { get; private set; }
        private ContentManager Content;
        private float MoveSpeed;
        private float WalkSpeed;
        private float RunSpeed;
        private List<Rectangle> Collisions;
        private bool IsColliding;
        public bool IsGrounded { get; private set; }
        private EntityLadder ActiveLadder;

        private ActorFacing Facing;
        public MovementType Movement { get; private set; }
        enum ActorFacing
        {
            Left,
            Right
        }
        public enum MovementType
        {
            Idle,
            RunWalk,
            Crawl,
            Climb
        }

        private AnimationManager AnimationManager;
        private Dictionary<string, Animation> AnimationLibrary;

        public ActorInventory Inventory { get; private set; }

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

            CreateCollisionMask(Position, 32, 64);

            AnimationManager = new AnimationManager();
            AnimationLibrary = new Dictionary<string, Animation>();
            Facing = ActorFacing.Right;
            Movement = MovementType.RunWalk;

            ActiveLadder = null;

            LoadContent();

            AnimationManager.Play(AnimationLibrary["idle"]);

            Inventory = new ActorInventory();

            InputManager.DownPressed += OnDownPress;
            InputManager.UpPressed += OnUpPress;
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
            Animation climbLadder = new Animation(Content.Load<Texture2D>("GFX/Characters/MainCharacterTemplate"), 1, 24, 16, 0f, new Vector2(0, 48));
            AnimationLibrary.Add("idle", idle);
            AnimationLibrary.Add("jump", jump);
            AnimationLibrary.Add("climbLadder", climbLadder);
        }
        public void Update()
        {
            Velocity = new Vector2(0, 0);

            ProcessInput();

            switch (Movement)
            {
                case MovementType.Climb:
                    {
                        UpdateClimb();
                        break;
                    }

                case MovementType.Idle:
                    {

                        break;
                    }
                case MovementType.RunWalk:
                    {
                        UpdateRunWalk();
                        break;
                    }
            }

            AnimationManager.Update();
        }
        public void Draw()
        {
            switch (Movement)
            {
                case MovementType.RunWalk:
                    {
                        if (Facing == ActorFacing.Right)
                        {
                            AnimationManager.Draw(CollisionMask, SpriteEffects.None);
                        }
                        else
                        {
                            AnimationManager.Draw(CollisionMask, SpriteEffects.FlipHorizontally);
                        }
                        break;
                    }
                case MovementType.Climb:
                    {
                        AnimationManager.Draw(CollisionMask, SpriteEffects.None);
                        break;
                    }
            }

            Global.SpriteBatch.Draw(Global.DebugTexture, CollisionMask, Color.Blue * 0.5f);
            Global.SpriteBatch.Draw(Global.DebugTexture, new Rectangle(CollisionMask.X, CollisionMask.Bottom, CollisionMask.Width, 1), Color.Yellow * 0.5f);
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

            if(Movement != MovementType.Climb)
            {
                if (Velocity.X > 0)
                {
                    Facing = ActorFacing.Right;
                }
                else if (Velocity.X < 0)
                {
                    Facing = ActorFacing.Left;
                }
            }
        }

        private void UpdateRunWalk()
        {
            float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;

            if (IsGrounded == false)
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
                CreateCollisionMask(ProposedPosition, 32, 64);
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

                int propX = (int)ProposedPosition.X;
                int propY = (int)ProposedPosition.Y;

                ProposedPosition = new Vector2(propX, propY);

                Position = ProposedPosition;
                Velocity = new Vector2(0);
                CreateCollisionMask(Position, 32, 64);
            }
        }
        private void UpdateClimb()
        {
            if(Velocity != new Vector2(0, 0))
            {
                ProposedPosition = Position + Velocity;
                CreateCollisionMask(ProposedPosition, 32, 64);
                OnPlayerClimbed();

                if(Movement == MovementType.Climb)
                {
                    Position = ProposedPosition;
                    Velocity = new Vector2(0);
                    CreateCollisionMask(Position, 32, 64);
                }
                else
                {
                    UpdateRunWalk();
                }
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
                    if(FallVector.Y >= 0)
                    {
                        adjustedY = -(collisionMask.Height);
                        IsGrounded = true;
                        FallVector = new Vector2(0);
                        AnimationManager.Play(AnimationLibrary["idle"]);
                    }
                }
                else
                {
                    adjustedY = collisionMask.Height;
                    Vector2 fallAdjust = new Vector2(FallVector.X, 0);
                    FallVector = fallAdjust;
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
        public void GrabLadder(EntityLadder ladder)
        {
            Movement = MovementType.Climb;
            AnimationManager.Play(AnimationLibrary["climbLadder"]);
            ActiveLadder = ladder;
            MoveSpeed = WalkSpeed;
            Move(new Vector2(ladder.Position.X, Position.Y));
        }
        public void DismountLadder()
        {
            ProposedPosition = Position;
            Movement = MovementType.RunWalk;
            AnimationManager.Play(AnimationLibrary["idle"]);
            ActiveLadder = null;
            OnPlayerMoved();
        }
        public void Move(Vector2 position)
        {
            ProposedPosition = position;
            Position = position;
            CreateCollisionMask(Position, 32, 64);
            IsGrounded = true;
            OnPlayerMoved();
        }
        public void MoveSilent(Vector2 position)
        {
            ProposedPosition = position;
            Position = position;
            CreateCollisionMask(Position, 32, 64);
            IsGrounded = true;
        }
        public void AdjustProposition(Vector2 adjustment)
        {
            if(Position.Y != adjustment.Y)
            {
                ProposedPosition = adjustment;
                OnPlayerMoved();
            }
        }

        private void OnLeftPress(object source, MoveInputEventArgs args)
        {
            if(Movement != MovementType.Climb)
            {
                float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
                float velocityRate = MoveSpeed * deltaTime;
                float adjustedMoveSpeed = (int)Math.Round(args.InputValue * velocityRate);
                Velocity = Velocity + new Vector2(adjustedMoveSpeed, 0);

                if (AnimationManager.Animation != AnimationLibrary["idle"])
                {
                    AnimationManager.Play(AnimationLibrary["idle"]);
                }
            }
        }
        private void OnRightPress(object source, MoveInputEventArgs args)
        {
            if(Movement != MovementType.Climb)
            {
                float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
                float velocityRate = MoveSpeed * deltaTime;
                float adjustedMoveSpeed = (int)Math.Round(args.InputValue * velocityRate);
                Velocity = Velocity + new Vector2(adjustedMoveSpeed, 0);

                if (AnimationManager.Animation != AnimationLibrary["idle"])
                {
                    AnimationManager.Play(AnimationLibrary["idle"]);
                }
            }
        }
        private void OnUpPress(object source, MoveInputEventArgs args)
        {
            if(Movement == MovementType.Climb)
            {
                float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
                float velocityRate = MoveSpeed * deltaTime;
                float adjustedMoveSpeed = (int)Math.Round(args.InputValue * velocityRate);
                Velocity = Velocity + new Vector2(0, adjustedMoveSpeed);

                if (AnimationManager.Animation != AnimationLibrary["climbLadder"])
                {
                    AnimationManager.Play(AnimationLibrary["climbLadder"]);
                }
            }
        }
        private void OnDownPress(object source, MoveInputEventArgs args)
        {
            if(Movement == MovementType.Climb)
            {
                float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;
                float velocityRate = MoveSpeed * deltaTime;
                float adjustedMoveSpeed = (int)Math.Round(args.InputValue * velocityRate);
                Velocity = Velocity + new Vector2(0, adjustedMoveSpeed);

                if (AnimationManager.Animation != AnimationLibrary["climbLadder"])
                {
                    AnimationManager.Play(AnimationLibrary["climbLadder"]);
                }
            }
        }
        private void OnInteractPress(object source, EventArgs args)
        {
            PlayerInteracted?.Invoke(this, EventArgs.Empty);
        }
        private void OnJumpPress(object source, EventArgs args)
        {
            if(Movement != MovementType.Climb)
            {
                float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;

                if (IsGrounded == true)
                {
                    float jumpRate = WalkSpeed * deltaTime;
                    FallVector = new Vector2(0, -(int)Math.Round(jumpRate * 8f));
                    IsGrounded = false;
                    AnimationManager.Play(AnimationLibrary["jump"]);
                }
            }
        }
        private void OnRunPress(object source, EventArgs args)
        {
            if(Movement != MovementType.Climb)
            {
                if (MoveSpeed < RunSpeed)
                {
                    MoveSpeed = RunSpeed;
                }
            }
        }
        private void OnRunRelease(object source, EventArgs args)
        {
            if(Movement != MovementType.Climb)
            {
                if (MoveSpeed == RunSpeed)
                {
                    MoveSpeed = WalkSpeed;
                }
            }
        }

        protected virtual void OnPlayerMoved()
        {
            PlayerMoved?.Invoke(this, EventArgs.Empty);
        }
        protected virtual void OnPlayerClimbed()
        {
            PlayerClimbed?.Invoke(this, new PlayerClimbedEventArgs() { Ladder = ActiveLadder } );
        }
        protected virtual void OnPlayerInteracted()
        {
            PlayerInteracted?.Invoke(this, EventArgs.Empty);
        }
    }
}
