using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GameTest.Content
{
    public class TextureManager
    {
        public static Texture2D DefaultTexture { get; set; }
        public static Texture2D FaceTexture { get; set; }
        public static Texture2D MadFaceTexture { get; set; }

        public TextureManager()
        {

        }

        public static void LoadTextures(ContentManager Content)
        {
            DefaultTexture = Content.Load<Texture2D>("default_texture");
            FaceTexture = Content.Load<Texture2D>("face");
            MadFaceTexture = Content.Load<Texture2D>("face_mad");
        }

    }
}
