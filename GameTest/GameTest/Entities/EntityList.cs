using GameTest.Entities.Terrain;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Entities
{
    public class EntityList
    {
        private TerrainMap Map;

        public ArrayList actorList { get; set; }
        public ArrayList particleList { get; set; }
        public ArrayList projectileList { get; set; }

        public EntityList(TerrainMap Map)
        {
            this.Map = Map;
            actorList = new ArrayList();
            particleList = new ArrayList();
            projectileList = new ArrayList();
        }

        public void Tick(float GTime)
        {
            for (int i = 0; i < Map.GetMap().GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetMap().GetLength(1); j++)
                {
                    Map.GetMap()[j, i].Tick(GTime);
                    TerrainObject t = (TerrainObject)Map.GetMap()[j, i];
                    t.RemoveHighlight();
                }
            }
            for (int i = 0; i < actorList.Count; i++)
            {
                GameObject obj = (GameObject)actorList[i];
                obj.Tick(GTime);
            }
            for (int i = 0; i < particleList.Count; i++)
            {
                GameObject obj = (GameObject)particleList[i];
                obj.Tick(GTime);
            }
            for (int i = 0; i < projectileList.Count; i++)
            {
                GameObject obj = (GameObject)projectileList[i];
                obj.Tick(GTime);
            }
        }

        public void Draw(SpriteBatch Sb)
        {
            for (int i = 0; i < Map.GetMap().GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetMap().GetLength(1); j++)
                    Map.GetMap()[j, i].Draw(Sb);
            }
            for (int i = 0; i < actorList.Count; i++)
            {
                GameObject obj = (GameObject)actorList[i];
                obj.Draw(Sb);
            }
            for (int i = 0; i < particleList.Count; i++)
            {
                GameObject obj = (GameObject)particleList[i];
                obj.Draw(Sb);
            }
            for (int i = 0; i < projectileList.Count; i++)
            {
                GameObject obj = (GameObject)projectileList[i];
                obj.Draw(Sb);
            }
        }

        public void AddObject(GameObject obj, ObjectType.ID id)
        {
            switch (id)
            {
                case ObjectType.ID.Actor:
                    actorList.Add(obj);
                    break;
                case ObjectType.ID.Particle:
                    particleList.Add(obj);
                    break;
                case ObjectType.ID.Projectile:
                    projectileList.Add(obj);
                    break;
                default:
                    particleList.Add(obj);
                    break;
            }
        }

        public void RemoveObject(GameObject obj, ObjectType.ID id)
        {
            switch (id)
            {
                case ObjectType.ID.Actor:
                    actorList.Remove(obj);
                    break;
                case ObjectType.ID.Particle:
                    particleList.Remove(obj);
                    break;
                case ObjectType.ID.Projectile:
                    projectileList.Remove(obj);
                    break;
                default:
                    particleList.Remove(obj);
                    break;
            }
        }

    }
}
