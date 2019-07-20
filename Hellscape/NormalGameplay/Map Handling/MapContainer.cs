using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;

namespace Hellscape
{
    public class MapContainer
    {
        public event EventHandler MapLoaded;

        public string MapID { get; protected set; }
        public TiledMap ActiveMap { get; protected set; }
        private TiledMap MapToLoad;
        private ContentManager Content;
        public List<EntityCollisionSolid> CollisionSolids { get; protected set; }
        public List<TransitionHandler> TransitionHandlers { get; protected set; }
        public List<TileEntitySceneObject> TileSceneObjects { get; protected set; }

        public MapContainer()
        {
            Content = Global.Content;

            MapID = "";
            ActiveMap = null;
            MapToLoad = null;

            CollisionSolids = new List<EntityCollisionSolid>();
            TransitionHandlers = new List<TransitionHandler>();
        }

        /// <summary>
        /// Loads TiledMap into memory, releasing old CollisionMasks, if present
        /// </summary>
        /// <param name="mapID">ID of Map to Load</param>
        /// <returns></returns>
        public void LoadMap(string mapID)
        {
            SceneObjectLoader loader = new SceneObjectLoader();

            if (ActiveMap != null)
            {
                loader.SaveSceneObjects(MapID, TileSceneObjects);
            }

            if(CollisionSolids.Count > 0)
            {
                for(int i = CollisionSolids.Count - 1; i >= 0; i--)
                {
                    CollisionSolids.RemoveAt(i);
                }
            }
            if(TransitionHandlers.Count > 0)
            {
                for(int i = TransitionHandlers.Count - 1; i >= 0; i--)
                {
                    TransitionHandlers.RemoveAt(i);
                }
            }

            MapID = mapID;
            string assetPath = Global.GetMapAssetPathByID(MapID);
            MapToLoad = Content.Load<TiledMap>(assetPath);
            TiledMapObjectLayer collisionLayer = MapToLoad.GetLayer<TiledMapObjectLayer>("CollisionMasks");
            TiledMapObjectLayer transitionLayer = MapToLoad.GetLayer<TiledMapObjectLayer>("Transitions");
            foreach (TiledMapObject _sceneObject in collisionLayer.Objects)
            {
                switch (_sceneObject.Name)
                {
                    case "CollisionSolid":
                        {
                            EntityCollisionSolid _newCollisionObject = new EntityCollisionSolid(_sceneObject.Position, (int)_sceneObject.Size.Width, (int)_sceneObject.Size.Height);
                            CollisionSolids.Add(_newCollisionObject);
                            break;
                        }
                }
            }
            foreach(TiledMapObject _transition in transitionLayer.Objects)
            {
                switch (_transition.Name)
                {
                    case "TransitionDoor":
                        {
                            TransitionHandler transition = new TransitionHandler(_transition.Properties["MapID"], _transition.Position.X, _transition.Position.Y, float.Parse(_transition.Properties["PlayerX"]), float.Parse(_transition.Properties["PlayerY"]), _transition.Size.Width, _transition.Size.Height);
                            TransitionHandlers.Add(transition);
                            break;
                        }
                    case "TransitionDoorLocked":
                        {
                            TransitionHandler transition = new TransitionHandler(_transition.Properties["MapID"], _transition.Position.X, _transition.Position.Y, float.Parse(_transition.Properties["PlayerX"]), float.Parse(_transition.Properties["PlayerY"]), _transition.Size.Width, _transition.Size.Height);
                            if(_transition.Properties["LockType"] == "Key")
                            {
                                transition.LockWithKey(Global.GetSceneObjectBYID(_transition.Properties["KeyShortname"]), int.Parse(_transition.Properties["KeyQuantity"]));
                            }
                            if (_transition.Properties["LockType"] == "Trigger")
                            {
                                transition.LockWithTrigger(_transition.Properties["TriggerKey"]);
                            }
                            TransitionHandlers.Add(transition);
                            break;
                        }
                }
            }

            ActiveMap = MapToLoad;
            TileSceneObjects = new List<TileEntitySceneObject>(loader.LoadSceneObjects(mapID));

            OnMapLoaded();
        }
        public void Update()
        {
            if(TileSceneObjects.Count > 0)
            {
                foreach(TileEntitySceneObject o in TileSceneObjects)
                {
                    o.Update();
                }
            }
        }
        public void Draw()
        {
            if (TileSceneObjects.Count > 0)
            {
                foreach (TileEntitySceneObject o in TileSceneObjects)
                {
                    o.Draw();
                }
            }
        }

        public void RemoveTileSceneObject(TileEntitySceneObject obj)
        {
            TileSceneObjects.Remove(obj);
        }

        protected virtual void OnMapLoaded()
        {
            MapLoaded?.Invoke(this, EventArgs.Empty);
        }
    }
}
