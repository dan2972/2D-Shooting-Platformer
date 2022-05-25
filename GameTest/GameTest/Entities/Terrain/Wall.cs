using GameTest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Terrain
{
    public class Wall : TerrainObject
    {
        public Wall(float X, float Y) : base(X, Y)
        {
            SizeX = 32;
            SizeY = 32;
            Size = 32;
        }

        public override void Draw(SpriteBatch Sb)
        {
            Texture2D Texture = TextureManager.DefaultTexture;

            Sb.Draw(Texture, destinationRectangle: new Rectangle((int)(X - SizeX / 2), (int)(Y - SizeY / 2), (int)SizeX, (int)SizeY), color: Color.White);
            base.Draw(Sb);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Wall;
        }

        public override void Tick(float GTime)
        {
            
        }
    }
}
