using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Hellscape
{
    /// <summary>
    /// SceneObjects are the most basic form of inventory items. They are stripped down and used as reference in the Global codex.
    /// TileEntitySceneObjects and InventoryItems both reference SceneObjects by referencing the ShortName as a form of ID.
    /// </summary>
    public class SceneObject
    {
        public string ShortName { get; protected set; }
        public string LongName { get; protected set; }
        public int MaxQuantity { get; protected set; }
        public Animation Animation { get; protected set; }

        public SceneObject(string shortName, string longName, int maxQuantity, Animation animation)
        {
            ShortName = shortName;
            LongName = longName;
            MaxQuantity = maxQuantity;
            Animation = animation;
        }

        public SceneObject Clone()
        {
            return new SceneObject(ShortName, LongName, MaxQuantity, Animation);
        }
    }
}
