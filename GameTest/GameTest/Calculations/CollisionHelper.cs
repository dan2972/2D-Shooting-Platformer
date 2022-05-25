using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Calculations
{
    public class CollisionHelper
    {

        public static bool AABB(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
        {
            return (pos1.X + size1.X > pos2.X && pos1.X < pos2.X + size2.X
                && pos1.Y + size1.Y > pos2.Y && pos1.Y < pos2.Y + size2.Y);
        }

        public static bool AABBCentered(Vector2 pos1, Vector2 size1, Vector2 pos2, Vector2 size2)
        {
            return (pos1.X + size1.X / 2 > pos2.X - size2.X / 2 && pos1.X - size1.X / 2 < pos2.X + size2.X / 2
                && pos1.Y + size1.Y / 2 > pos2.Y - size2.Y / 2 && pos1.Y  - size1.Y / 2 < pos2.Y + size2.Y / 2);
        }

        public static bool LineLineCollision(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            float denominator = ((b.X - a.X) * (d.Y - c.Y)) - ((b.Y - a.Y) * (d.X - c.X));
            float numerator1 = ((a.Y - c.Y) * (d.X - c.X)) - ((a.X - c.X) * (d.Y - c.Y));
            float numerator2 = ((a.Y - c.Y) * (b.X - a.X)) - ((a.X - c.X) * (b.Y - a.Y));

            if (denominator == 0) return numerator1 == 0 && numerator2 == 0;

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r >= 0 && r <= 1) && (s >= 0 && s <= 1);
        }

        public static Vector2 LineLineCollisionPoint(Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {

            // calculate the distance to intersection point
            float denom = ((d.Y - c.Y) * (b.X - a.X) - (d.X - c.X) * (b.Y - a.Y));
            if (denom == 0)
                return new Vector2(float.MaxValue, float.MaxValue);
            float uA = ((d.X - c.X) * (a.Y - c.Y) - (d.Y - c.Y) * (a.X - c.X)) / denom;
            float uB = ((b.X - a.X) * (a.Y - c.Y) - (b.Y - a.Y) * (a.X - c.X)) / denom;

            // if uA and uB are between 0-1, lines are colliding
            if (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1)
            {
                return new Vector2((a.X + (uA * (b.X - a.X))), (a.Y + (uA * (b.Y - a.Y))));
            }
            return new Vector2(float.MaxValue, float.MaxValue);
        }

        public static bool LineRectangle(Vector2 a, Vector2 b, Rectangle r)
        {
            bool top = LineLineCollision(a, b, new Vector2(r.X, r.Y), new Vector2(r.X + r.Width, r.Y));
            if (top)
                return true;
            bool bottom = LineLineCollision(a, b, new Vector2(r.X, r.Y + r.Height), new Vector2(r.X + r.Width, r.Y + r.Height));
            if (bottom)
                return true;
            bool left = LineLineCollision(a, b, new Vector2(r.X, r.Y), new Vector2(r.X, r.Y + r.Height));
            if (left)
                return true;
            bool right = LineLineCollision(a, b, new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y + r.Height));
            if (right)
                return true;

            return false;
        }

        public static Vector2 LineRectanglePoint(Vector2 a, Vector2 b, Rectangle r)
        {
            ArrayList collisionList = new ArrayList();

            Vector2 top = LineLineCollisionPoint(a, b, new Vector2(r.X, r.Y), new Vector2(r.X + r.Width, r.Y));
            Vector2 bottom = LineLineCollisionPoint(a, b, new Vector2(r.X, r.Y + r.Height), new Vector2(r.X + r.Width, r.Y + r.Height));
            Vector2 left = LineLineCollisionPoint(a, b, new Vector2(r.X, r.Y), new Vector2(r.X, r.Y + r.Height));
            Vector2 right = LineLineCollisionPoint(a, b, new Vector2(r.X + r.Width, r.Y), new Vector2(r.X + r.Width, r.Y + r.Height));

            if (top.X != int.MaxValue)
                collisionList.Add(top);
            if (bottom.X != int.MaxValue)
                collisionList.Add(bottom);
            if (left.X != int.MaxValue)
                collisionList.Add(left);
            if (right.X != int.MaxValue)
                collisionList.Add(right);

            Vector2 collisionPoint = new Vector2(float.MaxValue, float.MaxValue);

            for(int i = 0; i < collisionList.Count; i++)
            {
                if (Vector2.Distance(a, (Vector2)collisionList[i]) < Vector2.Distance(a, collisionPoint))
                    collisionPoint = (Vector2)collisionList[i];
            }

            return collisionPoint;

        }

        public static Vector2 ToVector2(Point point)
        {
            return new Vector2(point.X, point.Y);
        }

    }
}
