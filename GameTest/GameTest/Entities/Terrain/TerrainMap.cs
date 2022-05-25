using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;

namespace GameTest.Entities.Terrain
{
    public class TerrainMap
    {

        private GameObject[,] Map;

        private EntityList EList;
        private Bitmap Level1;

        public TerrainMap(EntityList EList, int x = 256, int y = 256)
        {
            this.EList = EList;
            string path = Path.Combine(Environment.CurrentDirectory, @"Levels\", "level1.png");
            Console.WriteLine(path);
            Level1 = new Bitmap(path);
            Map = new GameObject[y, x];

            for(int i = 0; i < Map.GetLength(0); i++)
            {
                for (int j = 0; j < Map.GetLength(1); j++)
                {
                    Map[i, j] = new Air(j * 32, i * 32);
                }
            }
        }

        public void LoadMap()
        {
            Console.WriteLine("Loading Level...");
            for (int y = 0; y < Level1.Height; y++)
                for (int x = 0; x < Level1.Width; x++)
                {
                    Color pixelColor = Level1.GetPixel(x, y);
                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;

                    if (r == 255 && g == 255 && b == 255)
                    {
                        Wall w = new Wall(x * 32, y * 32);
                        Map[y, x] = w;
                    }
                    if (r == 0 && g == 255 && b == 255)
                    {
                        Ice a = new Ice(x * 32, y * 32);
                        Map[y, x] = a;
                    }

                }
            Console.WriteLine("Finished Loading Level");
        }

        public void AddObject(GameObject obj, int x, int y)
        {
            Map[y, x] = obj;
        }

        public void RemoveObject(int x, int y)
        {
            Map[y, x] = new Air(x * 32, y * 32);
        }

        public GameObject[,] GetMap()
        {
            return this.Map;
        }


    }
}
