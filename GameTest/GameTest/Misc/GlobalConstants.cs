using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Misc
{
    public class GlobalConstants
    {

        public static int SCREEN_WIDTH = 1280;
        public static int SCREEN_HEIGHT = 720;

        public const int GAME_WIDTH = 1280;
        public const int GAME_HEIGHT = 720;

        public static float GAME_SCALE = (float)SCREEN_WIDTH / (float)GAME_WIDTH;

        public static float BLOCK_SIZE = 32;

        public static float GRAVITY = 800;

        public static float DEFAULT_FRICTION = 6;

    }
}
