using GameTest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Terrain
{
    public class Ice : TerrainObject
    {
        public Ice(float X, float Y) : base(X, Y)
        {
            SizeX = 32;
            SizeY = 32;
            Size = 32;
            Friction = 0.5f;
        }

        public override void Draw(SpriteBatch Sb)
        {
            Texture2D Texture = TextureManager.DefaultTexture;

            Sb.Draw(Texture, destinationRectangle: new Rectangle((int)(X - SizeX / 2), (int)(Y - SizeY / 2), (int)SizeX, (int)SizeY), color: new Color(180,255,255));
            base.Draw(Sb);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Ice;
        }

        public override void Tick(float GTime)
        {

        }
    }
}
