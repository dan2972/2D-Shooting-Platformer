using GameTest.Content;
using GameTest.Entities.Characters;
using GameTest.Entities.Particles;
using GameTest.Entities.Terrain;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Projectiles
{
    public class BasicBullet : Projectile
    {
        public float Knockback { get; set; }

        private MouseState MState;

        private EntityList EList;
        private Player P;
        private Camera Cam;
        private TerrainMap Map;

        private float PlayerX, PlayerY;
        private float Speed;
        private float LifeTime = 3;

        private float Rotation;

        public BasicBullet(Player P, Camera Cam, EntityList EList, TerrainMap Map, float angle = 0) : base(EList, Map)
        {
            this.P = P;
            this.EList = EList;
            this.Cam = Cam;
            this.Map = Map;

            Speed = 1200;

            PlayerX = P.X;
            PlayerY = P.Y;

            X = PlayerX;
            Y = PlayerY;

            MState = Mouse.GetState();
            float OffsetX = (MState.X - GlobalConstants.SCREEN_WIDTH / 2) / GlobalConstants.GAME_SCALE + Cam.GetX();
            float OffsetY = (MState.Y - GlobalConstants.SCREEN_HEIGHT / 2) / GlobalConstants.GAME_SCALE + Cam.GetY();

            float DX = OffsetX - PlayerX;
            float DY = OffsetY - PlayerY;

            Rotation = (float)Math.Atan(DY/DX) + angle;

            float Distance = Vector2.Distance(new Vector2(PlayerX, PlayerY), new Vector2(OffsetX, OffsetY));

            float cosA = (float)Math.Cos(angle);
            float sinA = (float)Math.Sin(angle);
            float cosR = DX / Distance;
            float sinR = DY / Distance;
            float sinRA = sinR * cosA + cosR * sinA;
            float cosRA = cosR * cosA - sinR * sinA;

            VX = cosRA * Speed;
            VY = sinRA * Speed;

            SizeX = 10;
            SizeY = 10;

            Knockback = 150f;
        }

        public override void Tick(float GTime)
        {
            MoveX(VX * GTime);
            MoveY(VY * GTime);

            Rotation = (float)Math.Atan(VY / VX);

            VY += GlobalConstants.GRAVITY * GTime;

            LifeTime -= GTime;
            if(LifeTime <= 0)
            {
                EList.RemoveObject(this, ObjectType.ID.Projectile);
            }

            for(int i = 0; i < EList.actorList.Count; i++)
            {
                GameObject obj = (GameObject)EList.actorList[i];

                //float halfS = SizeX / 2;
                if(Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y), new Vector2(SizeX, SizeY), 
                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                {
                    if(obj.GetID() != ObjectType.ID.Player)
                    {
                        Actor a = (Actor)obj;

                        float kx = (float)(Knockback * Math.Cos(Rotation));
                        //float ky = (float)(Math.Abs(Knockback * Math.Sin(Rotation)));
                        kx = VX < 0 ? -kx : kx;
                        a.TakeDamage();
                        a.ApplyForce();
                        a.VX += kx;
                        a.VY -= Knockback;

                        Destroy();
                    }
                }
            }
        }

        public override void Destroy()
        {
            for(int i = 0; i < 10; i++)
            {
                Random r = new Random();

                float pr = (float)(r.NextDouble() * 2 * Math.PI);
                float pvx = (float)(-600 / 2 * Math.Cos(pr));
                float pvy = (float)(-600 / 2 * Math.Sin(pr));

                /*
                float pvx = -VX / 2;
                float pvy = -VY / 2;

                float originalRotation = (float)Math.Atan(VY/VX);
                float newRotation = (float)(originalRotation + (r.NextDouble() * 2f - 1f));

                pvx = pvx * (float)Math.Cos(newRotation);
                pvy = pvy * (float)Math.Sin(newRotation);*/

                BasicParticle p = new BasicParticle(X, Y, pvx, pvy, EList);
                p.FadeSpeed = 4;
                p.LifeTime = 0.5f;
                EList.AddObject(p, ObjectType.ID.Particle);
            }
            EList.RemoveObject(this, ObjectType.ID.Projectile);
        }

        public override void Draw(SpriteBatch Sb)
        {
            Texture2D Texture = TextureManager.DefaultTexture;

            var origin = new Vector2(Texture.Width / 2f, Texture.Height / 2f);
            var scale = new Vector2(SizeX, SizeY);
            Sb.Draw(Texture, new Vector2(X, Y), null, Color.Yellow, Rotation,
                origin, scale, SpriteEffects.None, 0);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Projectile;
        }
    }
}
