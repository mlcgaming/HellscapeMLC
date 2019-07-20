using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class TileSceneObject
    {
        private Vector2 Position;
        private Vector2 Velocity;
        private float DisposalTimer;
        private Animation Animation;
        private AnimationManager AnimationManager;
        public bool IsDisposable { get; private set; }
        
        public TileSceneObject(Animation animation, Vector2 position)
        {
            Animation = animation;
            Position = position;
            Velocity = new Vector2(0, 0);
            DisposalTimer = 0.5f;
            IsDisposable = false;
            AnimationManager = new AnimationManager();
            AnimationManager.Play(Animation);
        }
        public TileSceneObject(Animation animation, Vector2 position, Vector2 velocity)
        {
            Animation = animation;
            Position = position;
            Velocity = velocity;
            DisposalTimer = 0.5f;
            IsDisposable = false;
            AnimationManager = new AnimationManager();
            AnimationManager.Play(Animation);
        }

        public void Update()
        {
            if(IsDisposable == false)
            {
                if(DisposalTimer > 0)
                {
                    float deltaTime = (float)Global.GameTime.ElapsedGameTime.TotalSeconds;

                    Vector2 deltaVelocity;
                    float deltaX, deltaY;

                    deltaX = Velocity.X * deltaTime;
                    deltaY = Velocity.Y * deltaTime;

                    deltaVelocity = new Vector2(deltaX, deltaY);

                    Position = Position + deltaVelocity;

                    DisposalTimer -= deltaTime;
                }
                else
                {
                    IsDisposable = true;
                }
            }
        }
        public void Draw()
        {
            AnimationManager.Draw(Position);
        }
    }
}
