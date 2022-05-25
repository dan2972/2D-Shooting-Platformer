using GameTest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Terrain
{
    public class TerrainObject : GameObject
    {
        protected float Friction = 6;
        protected Color ObjColor;

        protected bool Highlighted = false;

        public TerrainObject(float X, float Y) : base(X, Y)
        {
            ObjColor = Color.White;
        }

        public void Highlight()
        {
            Highlighted = true;
        }

        public void RemoveHighlight()
        {
            Highlighted = false;
        }

        public override void Draw(SpriteBatch Sb)
        {
            if (Highlighted)
            {
                Texture2D Texture = TextureManager.DefaultTexture;

                Sb.Draw(Texture, destinationRectangle: new Rectangle((int)(X - SizeX / 2), (int)(Y - SizeY / 2), (int)SizeX, (int)SizeY), color: Color.Red * 0.4f);
            }
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Wall;
        }

        public override void Tick(float GTime)
        {
            
        }

        public float getFriction()
        {
            return Friction;
        }

        public override ObjectType.CLASS GetClass()
        {
            return ObjectType.CLASS.TerrainObject;
        }
    }
}
