using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities
{
    public class ObjectType
    {
        public enum ID : int
        {
            Player = -1,
            Actor = 0,
            Projectile = 100,
            GrapplingHook = 101,
            Particle = 2,
            Air = 300,
            Wall = 301, 
            Ice = 302,
            Line = 9999
        }

        public enum CLASS : int
        {
            Mob = 0,
            TerrainObject = 1,
            Projectile = 2,
            Particle = 3
        }
    }
}
