using GameTest.Entities.Characters;
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
    public class GrapplingHook : GameObject
    {
        private MouseState MState;
        private KeyboardState KState;
        private KeyboardState prevKState;

        private Player P;
        private Line Rope;
        private EntityList EList;
        private TerrainMap Map;

        private bool InShootingState = true;
        private bool FoundGrapplePoint = false;

        private float VX, VY;
        private float Speed = 1200f;

        private float PlayerX, PlayerY;

        private float MaxLength = 450f;

        public GrapplingHook(Player P, Camera Cam, EntityList EList, TerrainMap Map)
        {
            this.P = P;
            this.EList = EList;
            this.Map = Map;

            PlayerX = P.X;
            PlayerY = P.Y;

            X = PlayerX;
            Y = PlayerY;

            MState = Mouse.GetState();
            float OffsetX = (MState.X - GlobalConstants.SCREEN_WIDTH / 2) / GlobalConstants.GAME_SCALE + Cam.GetX();
            float OffsetY = (MState.Y - GlobalConstants.SCREEN_HEIGHT / 2) / GlobalConstants.GAME_SCALE + Cam.GetY();

            float DX = OffsetX - PlayerX;
            float DY = OffsetY - PlayerY;

            float Distance = Vector2.Distance(new Vector2(PlayerX, PlayerY), new Vector2(OffsetX, OffsetY));

            VX = DX / Distance * Speed;
            VY = DY / Distance * Speed;

            Rope = new Line(new Vector2(PlayerX, PlayerY), new Vector2(X, Y), 5);
            Rope.ChangeColor(Color.Gray);
        }

        public override void Draw(SpriteBatch Sb)
        {
            Rope.Draw(Sb);
        }

        public override void Tick(float GTime)
        {
            MState = Mouse.GetState();

            PlayerX = P.X;
            PlayerY = P.Y;

            if (InShootingState)
            {
                X += VX * GTime;
                Y += VY * GTime;

                if (Vector2.Distance(new Vector2(PlayerX, PlayerY), new Vector2(X, Y)) > MaxLength)
                    InShootingState = false;

                int checkStartX = (int)(X / GlobalConstants.BLOCK_SIZE) - 2;
                int checkEndX = (int)(X / GlobalConstants.BLOCK_SIZE) + 3;
                int checkStartY = (int)(Y / GlobalConstants.BLOCK_SIZE) - 2;
                int checkEndY = (int)(Y / GlobalConstants.BLOCK_SIZE) + 3;

                checkStartX = checkStartX < 0 ? 0 : checkStartX;
                checkStartY = checkStartY < 0 ? 0 : checkStartY;
                checkEndX = checkEndX > Map.GetMap().GetLength(1) ? Map.GetMap().GetLength(1) : checkEndX;
                checkEndY = checkEndY > Map.GetMap().GetLength(0) ? Map.GetMap().GetLength(0) : checkEndY;

                for (int i = checkStartY; i < checkEndY; i++)
                {
                    for (int j = checkStartX; j < checkEndX; j++)
                    {
                        GameObject obj = Map.GetMap()[i, j];

                        TerrainObject a = (TerrainObject)obj;
                        //a.Highlight();

                        if (obj.GetID() != ObjectType.ID.Air)
                        {
                            //Vector2 p = Calculations.CollisionHelper.LineRectanglePoint(new Vector2(PlayerX, PlayerY), new Vector2(X, Y),
                                                                //new Rectangle((int)obj.X, (int)obj.Y, (int)obj.SizeX, (int)obj.SizeY));
                            if (Calculations.CollisionHelper.LineRectangle(new Vector2(PlayerX, PlayerY), new Vector2(X, Y),
                                                                new Rectangle((int)(obj.X - obj.SizeX / 2), (int)(obj.Y - obj.SizeY / 2), (int)obj.SizeX, (int)obj.SizeY)))
                            {
                                X = obj.X;
                                Y = obj.Y;
                                InShootingState = false;
                                FoundGrapplePoint = true;
                                P.Grapple(X, Y, Vector2.Distance(new Vector2(PlayerX, PlayerY), new Vector2(X, Y)));
                            }
                        }
                    }
                }
            }

            if (!InShootingState)
            {
                KState = Keyboard.GetState();

                if (!FoundGrapplePoint)
                {
                    float Distance = Vector2.Distance(new Vector2(PlayerX, PlayerY), new Vector2(X, Y));
                    VX = (X - PlayerX) / Distance * Speed + (X - PlayerX);
                    VY = (Y - PlayerY) / Distance * Speed + (Y - PlayerY);
                    X -= VX * GTime;
                    Y -= VY * GTime;
                    if (Distance < 10)
                    {
                        P.UnGrapple(false);
                        EList.RemoveObject(this, ObjectType.ID.Projectile);
                    }
                }

                if (KState.IsKeyDown(Keys.E) && prevKState.IsKeyUp(Keys.E))
                {
                    P.UnGrapple(true);
                    EList.RemoveObject(this, ObjectType.ID.Projectile);
                }

            }

            if (!P.getGrappleShooting())
            {
                P.UnGrapple(false);
                EList.RemoveObject(this, ObjectType.ID.Projectile);
            }

            Rope.ChangePosition(new Vector2(PlayerX, PlayerY), new Vector2(X, Y));
            prevKState = Keyboard.GetState();
        }

        public override ObjectType.ID GetID()
        {
            return ObjectType.ID.GrapplingHook;
        }

        public override ObjectType.CLASS GetClass()
        {
            return ObjectType.CLASS.Projectile;
        }
    }
}
