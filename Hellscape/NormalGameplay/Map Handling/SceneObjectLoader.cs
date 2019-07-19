using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Hellscape
{
    public class SceneObjectLoader
    {
        public List<TileEntitySceneObject> SceneObjects { get; protected set; }

        public SceneObjectLoader()
        {
            SceneObjects = new List<TileEntitySceneObject>();
        }

        public List<TileEntitySceneObject> LoadSceneObjects(string mapID)
        {
            string mapJson = File.ReadAllText(Global.Content.RootDirectory + "..\\..\\..\\..\\..\\..\\..\\..\\Hellscape\\Hellscape\\Content\\Data\\maps\\SceneObjects\\" + mapID + ".json");
            List<MapLoaderSceneObject> mapObjects = JsonConvert.DeserializeObject<List<MapLoaderSceneObject>>(mapJson);

            foreach (MapLoaderSceneObject o in mapObjects)
            {
                TileEntitySceneObject newTileObject = new TileEntitySceneObject(o);
                SceneObjects.Add(newTileObject);
            }

            return SceneObjects;
        }
    }
}
