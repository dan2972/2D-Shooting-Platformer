using GameTest.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities
{
    public class Line : GameObject
    {

        public Vector2 Position1 { get; set; }
        public Vector2 Position2 { get; set; }
        public Color FillColor { get; set; }
        
        private float Length, Rotation, Thickness;

        public Line(Vector2 Position1, Vector2 Position2, float Thickness = 1)
        {
            this.Position1 = Position1;
            this.Position2 = Position2;
            this.Thickness = Thickness;

            
            if(Position1.X > Position2.X)
            {
                this.Position2 = Position1;
                this.Position1 = Position2;
            }

            FillColor = Color.White;

            Calculate();
        }

        public void ChangePosition(Vector2 Position1, Vector2 Position2)
        {
            this.Position1 = Position1;
            this.Position2 = Position2;


            if (Position1.X > Position2.X)
            {
                this.Position2 = Position1;
                this.Position1 = Position2;
            }

            Calculate();
        }

        public void Shift(float x, float y)
        {
            ChangePosition(new Vector2(Position1.X + x, Position1.Y + y), new Vector2(Position2.X + x, Position2.Y + y));
        }

        public void ChangeThickness(float Thickness)
        {
            this.Thickness = Thickness;
        }

        public void ChangeColor(Color color)
        {
            FillColor = color;
        }

        public void Calculate()
        {
            Length = Vector2.Distance(Position1, Position2);

            float yDiff = Position2.Y - Position1.Y;
            float xDiff = Position2.X - Position1.X;


            Rotation = (float)Math.Atan(yDiff/xDiff);
            //Console.WriteLine(Position1 + " Rotation: " + Rotation + " Length: " + Length);
        }

        public override void Draw(SpriteBatch Sb)
        {
            Texture2D Texture = TextureManager.DefaultTexture;
            /*
            Sb.Draw(Texture, new Rectangle((int)Position1.X, (int)(Position1.Y - (Thickness / 2)), (int)Length, (int)Thickness), 
                null, Color.Red, Rotation, new Vector2(0, Texture.Height/2), SpriteEffects.None, 0);*/
            var origin = new Vector2(0f, Texture.Height / 2f);
            var scale = new Vector2(Length, Thickness);
            Sb.Draw(Texture, new Vector2(Position1.X, Position1.Y - (Thickness / 2)), null, FillColor, Rotation, 
                origin, scale, SpriteEffects.None, 0);
        }

        public override void Tick(float GTime)
        {
            Calculate();
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Line;
        }

        public override ObjectType.CLASS GetClass()
        {
            throw new NotImplementedException();
        }
    }
}
