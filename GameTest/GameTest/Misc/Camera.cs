using GameTest;
using GameTest.Entities;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public class Camera
{
    public float Zoom { get; set; }
    public float FollowSpeed { get; set; }
    public Rectangle VisibleArea { get; protected set; }
    public Matrix Transform { get; protected set; }

    private EntityList EList;

    private float X, Y;
    private float ShakeIntensity, ShakeReductionRate;

    private float ScrollValue, LastScrollValue;

    public Camera(EntityList EList)
    {
        this.EList = EList;

        Zoom = GlobalConstants.GAME_SCALE;
        X = 0;
        Y = 0;
        FollowSpeed = 10;
    }

    private void UpdatePosition(Vector2 target, float GTime)
    {
        Zoom = GlobalConstants.GAME_SCALE;
        float shakeX = 0, shakeY = 0;
        if(ShakeIntensity > 0)
        {
            Random r = new Random();
            shakeX = (float)(r.NextDouble() * 2 - 1) * ShakeIntensity;
            Random r2 = new Random();
            shakeY = (float)(r2.NextDouble() * 2 - 1) * ShakeIntensity;

            ShakeIntensity -= ShakeReductionRate * GTime;
        }

        X += (target.X - X) * GTime * FollowSpeed + shakeX;
        Y += (target.Y - Y) * GTime * FollowSpeed + shakeY;

        var position = Matrix.CreateTranslation(-X, -Y, 0);
        var offset = Matrix.CreateTranslation(
            GlobalConstants.SCREEN_WIDTH / 2,
            GlobalConstants.SCREEN_HEIGHT / 2,
            0);

        var scale = Matrix.CreateScale(Zoom);

        Transform = position * scale * offset;
    }

    public void UpdateCamera(float GTime)
    {
        for (int i = 0; i < EList.actorList.Count; i++)
        {
            GameObject obj = (GameObject)EList.actorList[i];

            if(obj.GetID() == ObjectType.ID.Player)
            {
                UpdatePosition(new Vector2(obj.X + obj.SizeX / 2, obj.Y + obj.SizeY / 2), GTime);
            }
        }

        KeyboardZoomControl();
    }

    private void KeyboardZoomControl()
    {
        KeyboardState ks = Keyboard.GetState();
        if (ks.IsKeyDown(Keys.LeftControl))
        {
            MouseState ms = Mouse.GetState();

            ScrollValue = ms.ScrollWheelValue;
            float ds = ScrollValue - LastScrollValue;
            if (ds > 0)
                GlobalConstants.GAME_SCALE += 0.1f;
            if (ds < 0)
                GlobalConstants.GAME_SCALE -= 0.1f;

            GlobalConstants.GAME_SCALE = GlobalConstants.GAME_SCALE < 0.1f ? 0.1f : GlobalConstants.GAME_SCALE;
            GlobalConstants.GAME_SCALE = GlobalConstants.GAME_SCALE > 3 ? 3 : GlobalConstants.GAME_SCALE;
            Console.WriteLine(ScrollValue + ", " + GlobalConstants.GAME_SCALE);

            LastScrollValue = ms.ScrollWheelValue;
        }

        if (ks.IsKeyDown(Keys.M))
        {
            GlobalConstants.GAME_SCALE = (float)GlobalConstants.SCREEN_WIDTH / (float)GlobalConstants.GAME_WIDTH;
        }
    }

    public void Shake(float intensity, float reductionRate = 10)
    {
        ShakeIntensity = intensity;
        ShakeReductionRate = reductionRate;
    }

    public float GetX()
    {
        return X;
    }

    public float GetY()
    {
        return Y;
    }
}
