namespace Hellscape
{
    public class MapLoaderSceneObject
    {
        public string ShortName { get; protected set; }
        public float MapX { get; protected set; }
        public float MapY { get; protected set; }
        public int Quantity { get; protected set; }

        public MapLoaderSceneObject(string shortName, float mapX, float mapY, int quantity)
        {
            ShortName = shortName;
            MapX = mapX;
            MapY = mapY;
            Quantity = quantity;
        }
    }
}
