using GameTest.Content;
using GameTest.Entities.Terrain;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Characters
{
    public class BasicEnemy : Actor
    {
        private EntityList EList;
        private TerrainMap Map;

        private bool MovingRight, MovingLeft, Jumping;

        private int RandomActionDecision;
        private float RandomActionTimer, RandomActionCooldown;

        public BasicEnemy(float X, float Y, EntityList EList, TerrainMap Map, float VX = 0, float VY = 0) : base(X, Y, EList, Map, VX, VY)
        {
            this.EList = EList;
            SizeX = 32;
            SizeY = 32;

            Speed = 300;
            Friction = 6f;
            FrictionThreshold = 50;
            Acceleration = 1200;
            JumpStrength = 500;

            DefaultCharacterColor = new Color(255, 50, 50);
        }

        public override void Tick(float GTime)
        {
            MoveX(VX * GTime);
            MoveY(VY * GTime);
            CheckCollisions();
            HandleMovement(GTime);

            base.Tick(GTime);
        }

        private void HandleMovement(float GTime)
        {
            if (MovingLeft)
            {
                //VX = -Speed;
                VX -= Acceleration * GTime;
                if (VX < -Speed)
                    VX = -Speed;
            }
            else if (MovingRight)
            {
                //VX = Speed;
                VX += Acceleration * GTime;
                if (VX > Speed)
                    VX = Speed;
            }
            else
            {
                //Console.WriteLine(VX + ", " + (1.0f - Friction) * GTime);
                HandleFriction(GTime);
            }

            if (!Grounded)
            {
                VY += GlobalConstants.GRAVITY * GTime;
            }
            else
            {
                VY = 0;
            }

            RandomActionGenerator(GTime);
        }

        private void RandomActionGenerator(float GTime)
        {
            Random r = new Random();

            RandomActionTimer -= GTime;
            if (RandomActionTimer <= 0)
            {
                RandomActionCooldown = (float)r.NextDouble() * 1;
                RandomActionTimer = RandomActionCooldown;
                RandomActionDecision = r.Next(0, 4);
            }

            switch (RandomActionDecision)
            {
                case 0:
                    MovingRight = false;
                    MovingLeft = false;
                    break;
                case 1:
                    MovingRight = true;
                    MovingLeft = false;
                    break;
                case 2:
                    MovingRight = false;
                    MovingLeft = true;
                    break;
                case 3:
                    if (Grounded)
                    {
                        VY -= JumpStrength;
                        Friction = GlobalConstants.DEFAULT_FRICTION;
                        RenderSizeX = SizeX - 10;
                        RenderSizeY = SizeY + 10;
                    }
                    break;

                default:
                    break;
            }
        }

        public override void Draw(SpriteBatch Sb)
        {
            CharacterTexture = TextureManager.MadFaceTexture;
            base.Draw(Sb);
        }

    }
}
