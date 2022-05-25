using GameTest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Terrain
{
    public class Air : TerrainObject
    {
        public Air(float X, float Y) : base(X, Y)
        {
            SizeX = 32;
            SizeY = 32;
            Size = 32;
            Friction = 0.5f;
        }

        public override void Draw(SpriteBatch Sb)
        {
            base.Draw(Sb);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Air;
        }

        public override void Tick(float GTime)
        {

        }
    }
}
