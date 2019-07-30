using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended.Tiled;
using MonoGame.Extended;

namespace Hellscape
{
    public class MapContainer
    {
        public event EventHandler MapLoaded;

        public string MapID { get; protected set; }
        public TiledMap ActiveMap { get; protected set; }
        private TiledMap MapToLoad;
        private ContentManager Content;
        public Rectangle BoundingBox { get; private set; } 
        public List<EntityCollisionSolid> CollisionSolids { get; protected set; }
        public List<TransitionHandler> TransitionHandlers { get; protected set; }
        public List<TileEntitySceneObject> TileSceneObjects { get; protected set; }
        public List<EntityLadder> InteractiveEntities { get; private set; }

        public MapContainer()
        {
            Content = Global.Content;

            MapID = "";
            ActiveMap = null;
            MapToLoad = null;

            CollisionSolids = new List<EntityCollisionSolid>();
            TransitionHandlers = new List<TransitionHandler>();
            InteractiveEntities = new List<EntityLadder>();
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
            TiledMapObjectLayer interactive = MapToLoad.GetLayer<TiledMapObjectLayer>("Interactive");

            if(collisionLayer.Objects.Length > 0 && collisionLayer != null)
            {
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
                        case "CollisionSolidAngle":
                            {
                                EntityCollisionSolid _newCollision = new EntityCollisionSolid(_sceneObject.Position, (int)_sceneObject.Size.Width, (int)_sceneObject.Size.Height, float.Parse(_sceneObject.Properties["Tilt"]));
                                CollisionSolids.Add(_newCollision);
                                break;
                            }
                    }
                }
            }
            if(transitionLayer.Objects.Length > 0 && transitionLayer != null)
            {
                foreach (TiledMapObject _transition in transitionLayer.Objects)
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
                                if (_transition.Properties["LockType"] == "Key")
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
                        case "TransitionSilent":
                            {
                                TransitionHandler transition = new TransitionHandler(_transition.Properties["MapID"], _transition.Position.X, _transition.Position.Y, float.Parse(_transition.Properties["PlayerX"]), float.Parse(_transition.Properties["PlayerY"]), _transition.Size.Width, _transition.Size.Height);
                                transition.SetAsSilent();
                                TransitionHandlers.Add(transition);
                                break;
                            }
                    }
                }
            }
            if(interactive != null && interactive.Objects.Length > 0)
            {
                foreach (TiledMapObject _interactive in interactive.Objects)
                {
                    switch (_interactive.Name)
                    {
                        case "InteractiveLadder":
                            {
                                EntityLadder ladder = new EntityLadder(_interactive.Position, (int)_interactive.Size.Width, (int)_interactive.Size.Height);
                                InteractiveEntities.Add(ladder);
                                break;
                            }
                    }
                }
            }

            ActiveMap = MapToLoad;
            TileSceneObjects = new List<TileEntitySceneObject>(loader.LoadSceneObjects(mapID));

            BoundingBox = new Rectangle(0, 0, ActiveMap.WidthInPixels, ActiveMap.HeightInPixels);

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

            //foreach (EntityCollisionSolid solid in CollisionSolids)
            //{
            //    if (solid.IsAngled == true)
            //    {
            //        Global.SpriteBatch.DrawLine(new Vector2(0, solid.YIntercept), new Vector2(ActiveMap.WidthInPixels, Common.GetYFromSlope(ActiveMap.WidthInPixels, solid.Tilt, solid.YIntercept)), Color.Red, 2f);
            //        //Global.SpriteBatch.DrawLine(new Vector2(0, solid.YIntercept), ActiveMap.WidthInPixels, solid.Tilt, Color.Red, 1f);
            //    }
            //}
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
