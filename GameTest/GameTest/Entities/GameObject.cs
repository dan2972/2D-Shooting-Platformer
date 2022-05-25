using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities
{
    public abstract class GameObject
    {

        public float X { get; set; }
        public float Y { get; set; }
        public float SizeX { get; set; }
        public float SizeY { get; set; }
        public float Size { get; set; }
        public float Radius { get; set; }

        public GameObject(float X, float Y)
        {
            this.X = X;
            this.Y = Y;
        }

        public GameObject()
        {
            X = 0;
            Y = 0;
        }

        public abstract void Tick(float GTime);

        public abstract void Draw(SpriteBatch Sb);

        public abstract ObjectType.ID GetID();

        public abstract ObjectType.CLASS GetClass();

    }
}
