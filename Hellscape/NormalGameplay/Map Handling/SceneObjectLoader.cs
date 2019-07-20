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
        public void SaveSceneObjects(string mapID, List<TileEntitySceneObject> tileSceneObjects)
        {
            if (tileSceneObjects.Count > 0)
            {
                List<MapLoaderSceneObject> mapObjects = new List<MapLoaderSceneObject>();

                for (int i = tileSceneObjects.Count - 1; i >= 0; i--)
                {
                    TileEntitySceneObject obj = tileSceneObjects.ElementAt(i);

                    mapObjects.Add(new MapLoaderSceneObject(obj.Object.ShortName, obj.Position.X, obj.Position.Y, obj.Quantity));

                    tileSceneObjects.RemoveAt(i);
                }

                string jsonSaveString = JsonConvert.SerializeObject(mapObjects, Formatting.Indented);
                File.WriteAllText(Global.Content.RootDirectory + "..\\..\\..\\..\\..\\..\\..\\..\\Hellscape\\Hellscape\\Content\\Data\\maps\\SceneObjects\\" + mapID + ".json", jsonSaveString);
            }
        }
    }
}
