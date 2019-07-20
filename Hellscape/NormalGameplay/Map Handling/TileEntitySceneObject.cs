using Microsoft.Xna.Framework;

namespace Hellscape
{
    public class TileEntitySceneObject
    {
        public SceneObject Object { get; protected set; }
        private AnimationManager AnimationManager { get; set; }
        public Vector2 Position { get; protected set; }
        public int Quantity { get; protected set; }
        public Rectangle CollisionMask { get; protected set; }

        public TileEntitySceneObject(MapLoaderSceneObject loadedObject)
        {
            AnimationManager = new AnimationManager();
            Object = Global.GetSceneObjectBYID(loadedObject.ShortName).Clone();
            Quantity = loadedObject.Quantity;
            Position = new Vector2(loadedObject.MapX, loadedObject.MapY);
            AnimationManager.Play(Object.Animation);
            CreateCollisionMask();
        }
        public void Update()
        {
            AnimationManager.Update(Global.GameTime);
        }
        public void Draw()
        {
            AnimationManager.Draw(Position);
        }
        private void CreateCollisionMask()
        {
            CollisionMask = new Rectangle((int)Position.X, (int)Position.Y, Object.Animation.FrameWidth, Object.Animation.FrameHeight);
        }
    }
}
