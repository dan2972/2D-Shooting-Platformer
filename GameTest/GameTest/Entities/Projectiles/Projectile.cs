using GameTest.Entities.Terrain;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities.Projectiles
{
    public class Projectile : GameObject
    {
        public float VX { get; set; }
        public float VY { get; set; }

        private float xRemainder, yRemainder;
        private EntityList EList;
        private TerrainMap Map;

        public Projectile(EntityList EList, TerrainMap Map)
        {
            X = 0;
            Y = 0;
            VX = 0;
            VY = 0;
            this.EList = EList;
            this.Map = Map;
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
                        for (int j = checkStartX; j < checkEndX; j++)
                        {
                            GameObject obj = Map.GetMap()[i, j];
                            if (obj.GetID() != ObjectType.ID.Air)
                            {
                                if (Calculations.CollisionHelper.AABBCentered(new Vector2(X, Y) + new Vector2(sign, 0), new Vector2(SizeX, SizeY),
                                                                new Vector2(obj.X, obj.Y), new Vector2(obj.SizeX, obj.SizeY)))
                                {
                                    hitSolid = true;
                                    Destroy();
                                }
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
                                    Destroy();
                                }
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

        public virtual void Destroy()
        {
            EList.RemoveObject(this, ObjectType.ID.Projectile);
        }

        public override void Draw(SpriteBatch Sb)
        {
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.Projectile;
        }

        public override void Tick(float GTime)
        {

        }

        public override ObjectType.CLASS GetClass()
        {
            return ObjectType.CLASS.Projectile;
        }
    }
}
