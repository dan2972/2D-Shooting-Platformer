using GameTest.Content;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Particles
{
    public class BasicParticle : GameObject
    {

        public float LifeTime { get; set; }
        public float FadeSpeed { get; set; }
        public Color ParticleColor { get; set; }

        public float ParticleSize { get; set; }
        public float Rotation { get; set; }

        public bool GravityOn { get; set; }

        private float VX, VY;
        private float Opacity = 1.0f;

        private EntityList EList;

        public BasicParticle(float X, float Y, float VX, float VY, EntityList EList) : base(X, Y)
        {
            this.VX = VX;
            this.VY = VY;
            this.EList = EList;

            Rotation = (float)Math.Atan(VY/VX);

            ParticleSize = 8f;
            SizeX = ParticleSize;
            SizeY = ParticleSize;

            LifeTime = 3;
            FadeSpeed = 1.0f;

            ParticleColor = Color.White;

            GravityOn = false;
        }


        public override void Tick(float GTime)
        {
            X += VX * GTime;
            Y += VY * GTime;

            if(GravityOn)
                VY += GlobalConstants.GRAVITY * GTime;

            Opacity -= GTime * FadeSpeed;
            Opacity = Opacity < 0 ? 0 : Opacity;

            LifeTime -= GTime;
            if(LifeTime <= 0)
            {
                EList.RemoveObject(this, ObjectType.ID.Particle);
            }

            if(SizeX != 0 && SizeY != 0 && SizeX == SizeY)
            {
                SizeX = ParticleSize;
                SizeY = ParticleSize;
            }
        }

        public override void Draw(SpriteBatch Sb)
        {
            Texture2D Texture = TextureManager.DefaultTexture;

            var origin = new Vector2(Texture.Width / 2, Texture.Height / 2);
            var scale = new Vector2(SizeX, SizeY);
            Sb.Draw(Texture, new Vector2(X, Y), null, ParticleColor * Opacity, Rotation,
                origin, scale, SpriteEffects.None, 0);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Particle;
        }

        public override ObjectType.CLASS GetClass()
        {
            return ObjectType.CLASS.Particle;
        }
    }
}
