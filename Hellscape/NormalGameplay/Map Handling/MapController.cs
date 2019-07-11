﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using MonoGame.Extended;
using MonoGame.Extended.Tiled;
using MonoGame.Extended.Tiled.Graphics;

namespace Hellscape
{
    public class MapContainer
    {
        private string MapID { get; set; }
        private TiledMap ActiveMap { get; set; }
        private TiledMap MapToLoad { get; set; }
        private ContentManager Content { get; set; }
        public List<EntityCollisionSolid> CollisionSolids { get; protected set; }

        public MapContainer()
        {
            Content = Global.Content;

            MapID = "";
            ActiveMap = null;
            MapToLoad = null;

            CollisionSolids = new List<EntityCollisionSolid>();
        }
        public MapContainer(ContentManager content)
        {
            Content = content;

            MapID = "";
            ActiveMap = null;

            CollisionSolids = new List<EntityCollisionSolid>();
        }

        /// <summary>
        /// Loads TiledMap into memory, releasing old CollisionMasks, if present
        /// </summary>
        /// <param name="mapID">ID of Map to Load</param>
        /// <returns></returns>
        public TiledMap LoadMap(string mapID)
        {
            if(CollisionSolids.Count > 0)
            {
                for(int i = CollisionSolids.Count - 1; i >= 0; i--)
                {
                    CollisionSolids.RemoveAt(i);
                }
            }

            MapID = mapID;
            string assetPath = Global.GetMapAssetPathByID(MapID);
            MapToLoad = Content.Load<TiledMap>(assetPath);
            TiledMapObjectLayer collisionLayer = MapToLoad.GetLayer<TiledMapObjectLayer>("SceneObjects");
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
            ActiveMap = MapToLoad;
            return ActiveMap;
        }
    }
}
