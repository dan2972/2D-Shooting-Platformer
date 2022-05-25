using GameTest.Content;
using GameTest.Entities.Particles;
using GameTest.Entities.Projectiles;
using GameTest.Entities.Terrain;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Characters
{
    public class Player : Actor
    {
        private KeyboardState keyboardState;
        private KeyboardState prevKeyboardState;
        private MouseState mouseState;
        private MouseState prevMouseState;

        private bool FlightMode = false;
        private bool GrappleMode = false;
        private bool GrappleShooting = false;

        private Camera Cam;
        private EntityList EList;
        private TerrainMap Map;

        private float PrimaryCooldownTime = 0.2f;
        private float PrimaryCooldownTimer = 0.2f;
        private float SecondaryCooldownTime = 0.5f;
        private float SecondaryCooldownTimer = 0.5f;

        private float MaxFlashDistance = 200;

        private float GrappleX, GrappleY, GrappleRadius;
        private float GrappleAngle, GrappleAngleV, GrappleAngleA;
        private float GrappleReleaseMultiplier = 50;
        private float LastX, LastY, DX, DY;

        public Player(float X, float Y, EntityList EList, Camera Cam, TerrainMap Map, float VX = 0, float VY = 0) : base(X, Y, EList, Map, VX, VY)
        {
            this.Cam = Cam;
            this.EList = EList;
            this.Map = Map;
            SizeX = 32;
            SizeY = 32;

            Speed = 300;
            Friction = 6f;
            FrictionThreshold = 50;
            ForceApplyThreshold = 50;
            Acceleration = 1200;
            JumpStrength = 500;
        }

        public override void Tick(float GTime)
        {
            //Console.WriteLine("X: " + VX + " Y: " + VY);
            //Console.WriteLine(Collided);

            if (GrappleMode)
            {
                HandleGrapplePhysics(GTime);
            }
            else
            {
                MoveX(VX * GTime);
                MoveY(VY * GTime);
            }

            CheckCollisions();

            HandleKeyboardInput(GTime);
            HandleMouseInput(GTime);
            HandleTimers(GTime);

            base.Tick(GTime);

            DX = LastX - X;
            DY = LastY - Y;
            LastX = X;
            LastY = Y;
        }

        private void HandleGrapplePhysics(float GTime)
        {
            // Pendulum physics calculations for grappling hook

            float dgx = GrappleX - X;
            float dgy = GrappleY - Y;

            if (dgx == 0) //check for nan cases
            {
                if (dgy < 0)
                    GrappleAngle = (float)Math.PI / 2;
                if (dgy > 0)
                    GrappleAngle = (float)(-Math.PI / 2);
            }
            else
            {
                if ((GrappleX - X) > 0)
                    GrappleAngle = (float)(Math.Atan((dgy) / (dgx)) + Math.PI);
                else
                    GrappleAngle = (float)(Math.Atan((dgy) / (dgx)));
            }

            GrappleAngleA = (float)(-0.01f * GrappleAngleV - GlobalConstants.GRAVITY * 2 * Math.Sin(GrappleAngle - (Math.PI / 2)) / GrappleRadius);
            GrappleAngleV += GrappleAngleA * GTime;
            GrappleAngle += GrappleAngleV * GTime;

            float targx = (float)(GrappleRadius * Math.Cos(GrappleAngle) + GrappleX);
            float targy = (float)(GrappleRadius * Math.Sin(GrappleAngle) + GrappleY);

            MoveX(targx - X);
            MoveY(targy - Y);
            //Console.WriteLine(targy - Y + " TargY: " + targy + " Y: " + Y + " Math: " + GrappleRadius * Math.Sin(GrappleAngle) + " Angle: " + GrappleAngle);
            //Console.WriteLine("Angle Calculation: " + (GrappleY - Y) / (GrappleX - X) + " : " + (GrappleY - Y) + " : " + (GrappleX - X));

            if (Collided)
            {
                UnGrapple(true);
            }
        }

        public void Grapple(float GrappleX, float GrappleY, float GrappleRadius)
        {
            GrappleMode = true;
            this.GrappleX = GrappleX;
            this.GrappleY = GrappleY;
            this.GrappleRadius = GrappleRadius;
        }

        public void UnGrapple(bool applyForce)
        {
            GrappleMode = false;
            GrappleShooting = false;
            if (applyForce)
            {
                ApplyForce();
                VX = -DX * GrappleReleaseMultiplier;
                VY = -DY * GrappleReleaseMultiplier;
            }
            GrappleAngle = 0;
            GrappleAngleV = 0;
            GrappleAngleA = 0;
        }

        private void HandleMouseInput(float GTime)
        {
            mouseState = Mouse.GetState();
            
            if (mouseState.LeftButton == ButtonState.Pressed && PrimaryCooldownTimer <= 0)
            {
                RenderSizeX = SizeX - 8;
                RenderSizeY = SizeY + 8;
                Cam.Shake(3, 15);
                BasicBullet b = new BasicBullet(this, Cam, EList, Map);
                EList.AddObject(b, ObjectType.ID.Projectile);
                PrimaryCooldownTimer = PrimaryCooldownTime;
            }

            if(mouseState.RightButton == ButtonState.Pressed && SecondaryCooldownTimer <= 0)
            {
                RenderSizeX = SizeX - 8;
                RenderSizeY = SizeY + 8;
                Cam.Shake(5, 12);
                SniperBullet b = new SniperBullet(this, Cam, EList, Map);
                EList.AddObject(b, ObjectType.ID.Projectile);
                SecondaryCooldownTimer = SecondaryCooldownTime;
            }

            prevMouseState = Mouse.GetState();
        }

        private void HandleTimers(float GTime)
        {
            PrimaryCooldownTimer -= GTime;
            PrimaryCooldownTimer = PrimaryCooldownTimer <= 0 ? 0 : PrimaryCooldownTimer;
            SecondaryCooldownTimer -= GTime;
            SecondaryCooldownTimer = SecondaryCooldownTimer <= 0 ? 0 : SecondaryCooldownTimer;
        }

        private void HandleKeyboardInput(float GTime)
        {
            keyboardState = Keyboard.GetState();

            if (!GrappleMode)
            {

                if (keyboardState.IsKeyDown(Keys.A) || keyboardState.IsKeyDown(Keys.Left))
                {
                    VX -= Acceleration * GTime;
                    if (VX < -Speed)
                        VX = -Speed;
                }
                else if (keyboardState.IsKeyDown(Keys.D) || keyboardState.IsKeyDown(Keys.Right))
                {
                    VX += Acceleration * GTime;
                    if (VX > Speed)
                        VX = Speed;
                }
                else
                {
                    //Console.WriteLine("ForceApplied: " + ForceApplied);
                    HandleFriction(GTime);
                }

                if (keyboardState.IsKeyDown(Keys.E) && prevKeyboardState.IsKeyUp(Keys.E))
                {
                    if (!GrappleShooting)
                    {
                        GrappleShooting = true;
                        GrapplingHook g = new GrapplingHook(this, Cam, EList, Map);
                        EList.AddObject(g, ObjectType.ID.Projectile);
                    }
                }
            }

            if (FlightMode)
            {
                if (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))
                {
                    VY = -Speed;
                }
                else if (keyboardState.IsKeyDown(Keys.S) || keyboardState.IsKeyDown(Keys.Down))
                {
                    VY = Speed;
                }
                else
                {
                    VY = 0;
                }
            }
            else
            {

                if (!Grounded && !GrappleMode)
                {
                    VY += GlobalConstants.GRAVITY * GTime;
                }
                else
                {
                    if (VY > 750) // if the player hits the ground too hard, create particles
                    {
                        Cam.Shake(5, 15);
                        for (int i = 0; i < 10; i++)
                        {
                            Random r = new Random();

                            BasicParticle p = new BasicParticle(r.Next((int)(X - SizeX / 2), (int)(X + SizeX / 2)),
                                                                Y + SizeY / 2, 0, 10, EList);
                            p.FadeSpeed = (float)r.NextDouble() * 2 + 1.5f;
                            p.LifeTime = 0.8f;
                            p.ParticleSize = r.Next(8, 24);
                            p.Rotation = (float)(r.NextDouble() * 2 * Math.PI);
                            EList.AddObject(p, ObjectType.ID.Particle);
                        }
                    }
                    VY = 0;
                }

                if (Grounded && (keyboardState.IsKeyDown(Keys.W) || keyboardState.IsKeyDown(Keys.Up))) //jumping physics
                {
                    VY -= JumpStrength;
                    Friction = GlobalConstants.DEFAULT_FRICTION;
                    RenderSizeX = SizeX - 10;
                    RenderSizeY = SizeY + 10;
                }
            }

            if(keyboardState.IsKeyDown(Keys.Q) && prevKeyboardState.IsKeyUp(Keys.Q))
            {
                RenderSizeX = SizeX - 8;
                RenderSizeY = SizeY + 8;
                Cam.Shake(3, 15);
                for (int i = 0; i < 5; i++)
                {
                    BasicBullet b = new BasicBullet(this, Cam, EList, Map, (i - 2) * (float)(Math.PI / 32f));
                    EList.AddObject(b, ObjectType.ID.Projectile);
                }
                PrimaryCooldownTimer = PrimaryCooldownTime;
            }
            
            //flash mechanics
            if (keyboardState.IsKeyDown(Keys.F) && prevKeyboardState.IsKeyUp(Keys.F))
            {
                MouseState ms = Mouse.GetState();


                float OffsetX = (ms.X - GlobalConstants.SCREEN_WIDTH / 2) / GlobalConstants.GAME_SCALE + Cam.GetX();
                float OffsetY = (ms.Y - GlobalConstants.SCREEN_HEIGHT / 2) / GlobalConstants.GAME_SCALE + Cam.GetY();

                int mousePosX = (int)Math.Round(OffsetX / GlobalConstants.BLOCK_SIZE);
                int mousePosY = (int)Math.Round(OffsetY / GlobalConstants.BLOCK_SIZE);

                float distance = (float)Math.Sqrt((X - OffsetX) * (X - OffsetX) + (Y - OffsetY) * (Y - OffsetY));
                if (distance > MaxFlashDistance)
                {
                    mousePosX = (int)Math.Round((X / GlobalConstants.BLOCK_SIZE) + ((OffsetX - X) / distance * (MaxFlashDistance / GlobalConstants.BLOCK_SIZE)));
                    mousePosY = (int)Math.Round((Y / GlobalConstants.BLOCK_SIZE) + ((OffsetY - Y) / distance * (MaxFlashDistance / GlobalConstants.BLOCK_SIZE)));
                }

                mousePosX = mousePosX < 0 ? 0 : mousePosX;
                mousePosY = mousePosY < 0 ? 0 : mousePosY;
                mousePosX = mousePosX > Map.GetMap().GetLength(1) ? Map.GetMap().GetLength(1) : mousePosX;
                mousePosY = mousePosY > Map.GetMap().GetLength(0) ? Map.GetMap().GetLength(0) : mousePosY;

                if (Map.GetMap()[mousePosY, mousePosX].GetID() == ObjectType.ID.Air)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Random r = new Random();

                        float pr = (float)(r.NextDouble() * 2 * Math.PI);
                        float pvx = (float)(-400 / 2 * Math.Cos(pr));
                        float pvy = (float)(-400 / 2 * Math.Sin(pr));

                        BasicParticle p = new BasicParticle(X, Y, pvx, pvy, EList);
                        p.FadeSpeed = 4;
                        p.LifeTime = 0.5f;
                        p.ParticleColor = Color.Yellow;
                        EList.AddObject(p, ObjectType.ID.Particle);
                    }
                    X = Map.GetMap()[mousePosY, mousePosX].X;
                    Y = Map.GetMap()[mousePosY, mousePosX].Y;
                    VX = 0;
                    VY = 0;
                    for (int i = 0; i < 10; i++)
                    {
                        Random r = new Random();

                        float pr = (float)(r.NextDouble() * 2 * Math.PI);
                        float pvx = (float)(-400 / 2 * Math.Cos(pr));
                        float pvy = (float)(-400 / 2 * Math.Sin(pr));

                        BasicParticle p = new BasicParticle(X, Y, pvx, pvy, EList);
                        p.FadeSpeed = 4;
                        p.LifeTime = 0.5f;
                        p.ParticleColor = Color.Yellow;
                        EList.AddObject(p, ObjectType.ID.Particle);
                    }
                }

            }

            prevKeyboardState = Keyboard.GetState();
        }

        public override void Draw(SpriteBatch Sb)
        {
            CharacterTexture = TextureManager.FaceTexture;
            base.Draw(Sb);
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Player;
        }

        public bool getGrappleShooting()
        {
            return GrappleShooting;
        }

    }
}
