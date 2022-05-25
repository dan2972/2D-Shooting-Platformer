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
    public class Actor : GameObject
    {
        public float VX { get; set; }
        public float VY { get; set; }

        public bool Grounded { get; set; }
        public bool Collided { get; set; }

        protected Color DefaultCharacterColor = Color.SkyBlue;
        protected Texture2D CharacterTexture;

        protected float Speed;
        protected float Friction;
        protected float FrictionThreshold = 50;
        protected float Acceleration;
        protected float JumpStrength;
        protected float ForceApplyThreshold = 50;

        protected float RenderSizeX;
        protected float RenderSizeY;

        protected bool ForceApplied = false;
        protected bool TakingDamage = false;

        private float xRemainder, yRemainder;
        private EntityList EList;
        private TerrainMap Map;

        private Color CharacterColor;

        private float DamageFlashTimer = 0.1f;
        private float DamageFlashTime = 0.1f;

        public Actor(float X, float Y, EntityList EList, TerrainMap Map, float VX = 0, float VY = 0) : base(X, Y)
        {
            this.VX = VX;
            this.VY = VY;
            this.EList = EList;
            this.Map = Map;
            Grounded = false;
            Collided = false;
        }

        public void MoveX(float amount)
        {
            xRemainder += amount;
            int move = (int)Math.Round(xRemainder);

            if (move != 0)
            {
                xRemainder -= move;
                int sign = Math.Sign(move);
                while (move != 0)
                {
                    bool hitSolid = false;
                    //////////////////////////////// Check for Collision

                    int checkStartX = (int)(X / GlobalConstants.BLOCK_SIZE) - 4;
                    int checkEndX = (int)(X / GlobalConstants.BLOCK_SIZE) + 4;
                    int checkStartY = (int)(Y / GlobalConstants.BLOCK_SIZE) - 4;
                    int checkEndY = (int)(Y / GlobalConstants.BLOCK_SIZE) + 4;

                    checkStartX = checkStartX < 0 ? 0 : checkStartX;
                    checkStartY = checkStartY < 0 ? 0 : checkStartY;
                    checkEndX = checkEndX > Map.GetMap().GetLength(1) ? Map.GetMap().GetLength(1) : checkEndX;
                    checkEndY = checkEndY > Map.GetMap().GetLength(0) ? Map.GetMap().GetLength(0) : checkEndY;

                    for (int i = checkStartY; i < checkEndY; i++)
                    {
                        for(int j = checkStartX; j < checkEndX; j++)
                        {
                            GameObject obj = Map.GetMap()[i, j];
                            if (obj.GetID() != ObjectType.ID.Air)
                            {
                                if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y) + new Vector2(sign, 0), new Vector2(SizeX, SizeY),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                                {
                                    hitSolid = true;
                                    break;
                                }
                            }
                        }
                    }

                    for(int i = 0; i < EList.actorList.Count; i++)
                    {
                        GameObject obj = (GameObject)EList.actorList[i];
                        if (obj != this)
                        {
                            if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y) + new Vector2(sign, 0), new Vector2(SizeX, SizeY),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                            {
                                hitSolid = true;
                                break;
                            }
                        }
                    }

                    ///////////////////////////////
                    if (!hitSolid)
                    {
                        X += sign;
                        move -= sign;
                    }
                    else
                    {
                        move = 0;
                        break;
                    }
                }
            }
        }

        public void MoveY(float amount)
        {
            yRemainder += amount;
            int move = (int)Math.Round(yRemainder);

            if (move != 0)
            {
                yRemainder -= move;
                int sign = Math.Sign(move);
                while (move != 0)
                {
                    bool hitSolid = false;

                    //////////////////////////////// Check for Collision

                    int checkStartX = (int)(X / GlobalConstants.BLOCK_SIZE) - 4;
                    int checkEndX = (int)(X / GlobalConstants.BLOCK_SIZE) + 4;
                    int checkStartY = (int)(Y / GlobalConstants.BLOCK_SIZE) - 4;
                    int checkEndY = (int)(Y / GlobalConstants.BLOCK_SIZE) + 4;

                    checkStartX = checkStartX < 0 ? 0 : checkStartX;
                    checkStartY = checkStartY < 0 ? 0 : checkStartY;
                    checkEndX = checkEndX > Map.GetMap().GetLength(1) ? Map.GetMap().GetLength(1) : checkEndX;
                    checkEndY = checkEndY > Map.GetMap().GetLength(0) ? Map.GetMap().GetLength(0) : checkEndY;

                    for (int i = checkStartY; i < checkEndY; i++)
                    {
                        for (int j = checkStartX; j < checkEndX; j++)
                        {
                            GameObject obj = Map.GetMap()[i, j];

                            if (obj.GetID() != ObjectType.ID.Air)
                            {
                                if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y) + new Vector2(0, sign), new Vector2(SizeX, SizeY),
                                                                new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                                {
                                    hitSolid = true;
                                    break;
                                }
                            }
                        }
                    }

                    for (int i = 0; i < EList.actorList.Count; i++)
                    {
                        GameObject obj = (GameObject)EList.actorList[i];
                        if (obj != this)
                        {
                            if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y) + new Vector2(0, sign), new Vector2(SizeX, SizeY),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                            {
                                hitSolid = true;
                                break;
                            }
                        }
                    }

                    ///////////////////////////////

                    if (!hitSolid)
                    {
                        Y += sign;
                        move -= sign;
                    }
                    else
                    {
                        move = 0;
                        break;
                    }
                }
            }
        }

        public void CheckCollisions()
        {
            //Console.WriteLine(Friction);
            bool collidedWithGround = false;
            bool collided = false;

            int checkStartX = (int)(X / GlobalConstants.BLOCK_SIZE) - 4;
            int checkEndX = (int)(X / GlobalConstants.BLOCK_SIZE) + 4;
            int checkStartY = (int)(Y / GlobalConstants.BLOCK_SIZE) - 4;
            int checkEndY = (int)(Y / GlobalConstants.BLOCK_SIZE) + 4;

            checkStartX = checkStartX < 0 ? 0 : checkStartX;
            checkStartY = checkStartY < 0 ? 0 : checkStartY;
            checkEndX = checkEndX > Map.GetMap().GetLength(1) ? Map.GetMap().GetLength(1) : checkEndX;
            checkEndY = checkEndY > Map.GetMap().GetLength(0) ? Map.GetMap().GetLength(0) : checkEndY;

            //Console.WriteLine("X: " + checkStartX + ", " + checkEndX + " Y: " + checkStartY + ", " + checkEndY);

            for (int i = checkStartY; i < checkEndY; i++)
            {
                for (int j = checkStartX; j < checkEndX; j++)
                {
                    GameObject obj = Map.GetMap()[i, j];

                    //TerrainObject a = (TerrainObject)obj;
                    //a.Highlight();

                    if (obj.GetID() != ObjectType.ID.Air)
                    {
                        if (Calculations.CollisionHelper.AABBCentered(new Vector2(X + SizeX / 2, Y), new Vector2(1, SizeY),
                                                                        new Vector2(obj.X , obj.Y), new Vector2(obj.SizeX, obj.SizeY)) ||
                            Calculations.CollisionHelper.AABBCentered(new Vector2(X - SizeX / 2 - 1, Y), new Vector2(1, SizeY),
                                                                        new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                        {
                            collided = true;
                        }

                        if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y + SizeY / 2), new Vector2(SizeX, 1),
                                                                        new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                        {
                            collided = true;
                            collidedWithGround = true;
                            if (obj.GetClass() == ObjectType.CLASS.TerrainObject)
                            {
                                TerrainObject t = (TerrainObject)obj;
                                Friction = t.getFriction();
                            }
                            Y = (float)Math.Round(Y);
                            if (VY > 0)
                            {
                                RenderSizeX = SizeX + 10;
                                RenderSizeY = SizeY - 10;
                            }
                        }
                        if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y - SizeY / 2 - 1), new Vector2(SizeX, 1),
                                                                        new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                        {
                            collided = true;
                            if (VY < 0)
                            {
                                VY = 0;
                                RenderSizeX = SizeX + 10;
                                RenderSizeY = SizeY - 10;
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < EList.actorList.Count; i++)
            {
                GameObject obj = (GameObject)EList.actorList[i];

                if (obj != this)
                {
                    if (Calculations.CollisionHelper.AABBCentered(new Vector2(X + SizeX / 2, Y), new Vector2(1, SizeY),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)) ||
                        Calculations.CollisionHelper.AABBCentered(new Vector2(X - SizeX / 2 - 1, Y), new Vector2(1, SizeY),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                    {
                        collided = true;
                    }

                    if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y + SizeY / 2), new Vector2(SizeX, 1),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                    {
                        collided = true;
                        collidedWithGround = true;
                        if (obj.GetClass() == ObjectType.CLASS.TerrainObject)
                        {
                            TerrainObject t = (TerrainObject)obj;
                            Friction = t.getFriction();
                        }
                        Y = (float)Math.Round(Y);
                        if (VY > 0)
                        {
                            RenderSizeX = SizeX + 10;
                            RenderSizeY = SizeY - 10;
                        }
                    }
                    if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y - SizeY / 2 - 1), new Vector2(SizeX, 1),
                                                                    new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                    {
                        collided = true;
                        if (VY < 0)
                        {
                            VY = 0;
                            RenderSizeX = SizeX + 10;
                            RenderSizeY = SizeY - 10;
                        }
                    }
                }
            }

            if (collided)
                Collided = true;
            else
                Collided = false;
            if (collidedWithGround)
            {
                Grounded = true;
            }
            else
            {
                Grounded = false;
                //Friction = GlobalConstants.DEFAULT_FRICTION;
            }
        }

        protected void HandleFriction(float GTime)
        {
            if (!ForceApplied)
            {
                VX *= 1.0f - (Friction * GTime);
                if (Math.Abs(VX) <= FrictionThreshold)
                    VX = 0;
            }
            else
            {
                VX *= 1.0f - (2f * GTime);
            }
        }

        public void ApplyForce()
        {
            ForceApplied = true;
        }

        public void TakeDamage()
        {
            TakingDamage = true;
            CharacterColor = Color.White;
            DamageFlashTimer = DamageFlashTime;
            RenderSizeX = SizeX - 15;
            RenderSizeY = SizeY + 15;
        }

        public override void Draw(SpriteBatch Sb)
        {
            if (CharacterTexture == null)
                CharacterTexture = TextureManager.DefaultTexture;
            Sb.Draw(CharacterTexture, destinationRectangle: new Rectangle((int)(X - RenderSizeX / 2), (int)(Y - RenderSizeY / 2), (int)RenderSizeX, (int)RenderSizeY), color: CharacterColor);
        }

        public override void Tick(float GTime)
        {
            if (ForceApplied)
            {
                if (Math.Abs(VX) <= ForceApplyThreshold)
                {
                    ForceApplied = false;
                }
            }

            if(Math.Abs(RenderSizeX - SizeX) > 1)
            {
                RenderSizeX += (float)Math.Sign(SizeX - RenderSizeX) * GTime * 70;
            }
            else
            {
                RenderSizeX = SizeX;
            }
            if (Math.Abs(RenderSizeY - SizeY) > 1)
            {
                RenderSizeY += (float)Math.Sign(SizeY - RenderSizeY) * GTime * 70;
            }
            else
            {
                RenderSizeY = SizeY;
            }

            if (TakingDamage)
            {
                DamageFlashTimer -= GTime;
                if(DamageFlashTimer <= 0)
                {
                    TakingDamage = false;
                }
            }
            else
            {
                CharacterColor = DefaultCharacterColor;
            }
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Actor;
        }

        public override ObjectType.CLASS GetClass()
        {
            return ObjectType.CLASS.Mob;
        }
    }
}
