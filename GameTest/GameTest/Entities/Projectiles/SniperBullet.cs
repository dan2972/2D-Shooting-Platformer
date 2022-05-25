using GameTest.Content;
using GameTest.Entities.Characters;
using GameTest.Entities.Particles;
using GameTest.Entities.Terrain;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Projectiles
{
    public class SniperBullet : Projectile
    {
        public float Knockback { get; set; }

        private MouseState MState;

        private EntityList EList;
        private Player P;
        private Camera Cam;
        private TerrainMap Map;

        private Line BulletLine;
        private Color LineColor;
        private float LineOpacity = 1.0f;

        private float EndX, EndY;
        private float Speed;
        private float Rotation;
        private float SniperDistance = 1000;

        public SniperBullet(Player P, Camera Cam, EntityList EList, TerrainMap Map) : base(EList, Map)
        {
            this.P = P;
            this.EList = EList;
            this.Cam = Cam;
            this.Map = Map;

            Speed = 1200;
            Knockback = 150f;

            X = P.X;
            Y = P.Y;

            MState = Mouse.GetState();
            float OffsetX = (MState.X - GlobalConstants.SCREEN_WIDTH / 2) / GlobalConstants.GAME_SCALE + Cam.GetX();
            float OffsetY = (MState.Y - GlobalConstants.SCREEN_HEIGHT / 2) / GlobalConstants.GAME_SCALE + Cam.GetY();

            float DX = OffsetX - X;
            float DY = OffsetY - Y;

            Rotation = (float)Math.Atan(DY / DX);

            float Distance = Vector2.Distance(new Vector2(X, Y), new Vector2(OffsetX, OffsetY));

            VX = DX / Distance * Speed;
            VY = DY / Distance * Speed;
            EndX = X + DX / Distance * SniperDistance;
            EndY = Y + DY / Distance * SniperDistance;

            LineColor = new Color(255, 80, 0);
            BulletLine = new Line(new Vector2(X, Y), new Vector2(EndX, EndY));
            BulletLine.ChangeColor(LineColor);

            CheckCollision();
            ParticleEffects(DX, DY, Distance);
        }

        private void ParticleEffects(float DX, float DY, float Distance)
        {
            float d = Vector2.Distance(new Vector2(X, Y), new Vector2(EndX, EndY));
            int particleCount = (int)d / 10;
            for (int i = 0; i < particleCount; i++)
            {
                Random r = new Random();
                float rd = (float)(d * r.NextDouble());
                float px = X + DX / Distance * rd;
                float py = Y + DY / Distance * rd;
                float pvy = (float)(r.NextDouble() * 10 - 5);

                BasicParticle p = new BasicParticle(px, py, 0, pvy, EList);
                p.FadeSpeed = (float)r.NextDouble() * 1.5f + 0.4f;
                p.ParticleColor = new Color(200, 200, 200);
                p.LifeTime = 3f;
                p.SizeX = (float)r.NextDouble() * 20 + 10;
                p.SizeY = 1;
                p.Rotation = Rotation;
                EList.AddObject(p, ObjectType.ID.Particle);
            }
        }

        private void CheckCollision()
        {
            //create check boundary for loop
            int checkStartX = (int)(X / GlobalConstants.BLOCK_SIZE);
            int checkEndX = (int)(EndX / GlobalConstants.BLOCK_SIZE);
            int checkStartY = (int)(Y / GlobalConstants.BLOCK_SIZE);
            int checkEndY = (int)(EndY / GlobalConstants.BLOCK_SIZE);

            if (checkStartX > checkEndX)
            {
                int temp = checkStartX;
                checkStartX = checkEndX;
                checkEndX = temp;
            }
            if(checkStartY > checkEndY)
            {
                int temp = checkStartY;
                checkStartY = checkEndY;
                checkEndY = temp;
            }

            checkStartX -= 2;
            checkEndX += 3;
            checkStartY -= 2;
            checkEndY += 3;

            checkStartX = checkStartX < 0 ? 0 : checkStartX;
            checkStartY = checkStartY < 0 ? 0 : checkStartY;
            checkEndX = checkEndX > Map.GetMap().GetLength(1) ? Map.GetMap().GetLength(1) : checkEndX;
            checkEndY = checkEndY > Map.GetMap().GetLength(0) ? Map.GetMap().GetLength(0) : checkEndY;

            ArrayList CollisionPoints = new ArrayList();

            //find all collision points and add them to array list
            for (int i = checkStartY; i < checkEndY; i++)
            {
                for (int j = checkStartX; j < checkEndX; j++)
                {
                    GameObject obj = Map.GetMap()[i, j];
                    TerrainObject a = (TerrainObject)obj;
                    //a.Highlight();

                    if (obj.GetID() != ObjectType.ID.Air)
                    {
                        Rectangle objRect = new Rectangle((int)(obj.X - obj.SizeX / 2), (int)(obj.Y - obj.SizeY / 2), (int)obj.SizeX, (int)obj.SizeY);

                        if (Calculations.CollisionHelper.LineRectangle(new Vector2(X, Y), new Vector2(EndX, EndY), objRect))
                        {
                            Vector2 colPoint = Calculations.CollisionHelper.LineRectanglePoint(new Vector2(X, Y), new Vector2(EndX, EndY), objRect);
                            CollisionPoints.Add(colPoint);
                        }
                    }
                }
            }

            //if there is a collision point, find the shortest one and modify line to match it
            if(CollisionPoints.Count > 0)
            {
                Vector2 shortestColPoint = new Vector2(EndX, EndY);
                float shortestDistance = Vector2.Distance(new Vector2(X, Y), new Vector2(EndX, EndY));
                for(int i = 0; i < CollisionPoints.Count; i++)
                {
                    Vector2 p = (Vector2)CollisionPoints[i];
                    float d = Vector2.Distance(new Vector2(X, Y), p);
                    if (d < shortestDistance)
                    {
                        shortestColPoint = p;
                        shortestDistance = d;
                    }
                }
                BulletLine.ChangePosition(new Vector2(X, Y), shortestColPoint);
                EndX = shortestColPoint.X;
                EndY = shortestColPoint.Y;

                for (int k = 0; k < 10; k++) //particle effects
                {
                    Random r = new Random();

                    float pr = (float)(r.NextDouble() * 2 * Math.PI);
                    float pvx = (float)(-600 / 2 * Math.Cos(pr));
                    float pvy = (float)(-600 / 2 * Math.Sin(pr));

                    BasicParticle p = new BasicParticle(EndX, EndY, pvx, pvy, EList);
                    p.FadeSpeed = 4;
                    p.LifeTime = 0.5f;
                    EList.AddObject(p, ObjectType.ID.Particle);
                }
            }

            //check collisions with other characters
            for (int i = 0; i < EList.actorList.Count; i++)
            {
                GameObject obj = (GameObject)EList.actorList[i];

                if (Calculations.CollisionHelper.LineRectangle(new Vector2(X, Y), new Vector2(EndX, EndY),
                                                                new Rectangle((int)(obj.X - obj.SizeX / 2), (int)(obj.Y - obj.SizeY / 2), (int)obj.SizeX, (int)obj.SizeY)))
                {
                    if (obj.GetID() != ObjectType.ID.Player)
                    {
                        Actor a = (Actor)obj;

                        float kx = (float)(Knockback * Math.Cos(Rotation));
                        //float ky = (float)(Math.Abs(Knockback * Math.Sin(Rotation)));
                        kx = VX < 0 ? -kx : kx;
                        a.TakeDamage();
                        a.ApplyForce();
                        a.VX += kx;
                        a.VY -= Knockback;

                        for (int k = 0; k < 10; k++) // particle effects
                        {
                            Random r = new Random();

                            float pr = (float)(r.NextDouble() * 2 * Math.PI);
                            float pvx = (float)(-600 / 2 * Math.Cos(pr));
                            float pvy = (float)(-600 / 2 * Math.Sin(pr));

                            BasicParticle p = new BasicParticle(a.X, a.Y, pvx, pvy, EList);
                            p.FadeSpeed = 4;
                            p.LifeTime = 0.5f;
                            EList.AddObject(p, ObjectType.ID.Particle);
                        }
                    }
                }
            }
        }

        public override void Tick(float GTime)
        {
            LineColor = Color.Lerp(new Color(255, 80, 0), new Color(200, 200, 200), (1f - LineOpacity) * 3);
            LineOpacity -= 2f * GTime;
            if (LineOpacity <= 0)
                Destroy();
        }

        public override void Destroy()
        {
            EList.RemoveObject(this, ObjectType.ID.Projectile);
        }

        public override void Draw(SpriteBatch Sb)
        {
            BulletLine.ChangeColor(LineColor * LineOpacity);
            BulletLine.Draw(Sb);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Projectile;
        }
    }
}
