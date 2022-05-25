using GameTest.Content;
using GameTest.Entities;
using GameTest.Entities.Characters;
using GameTest.Entities.Terrain;
using GameTest.Misc;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace GameTest
{
    public class Game1 : Game
    {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private TextureManager TextureManager;
        private EntityList EList;
        private TerrainMap Map;
        private Camera Cam;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = GlobalConstants.SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = GlobalConstants.SCREEN_HEIGHT;
            _graphics.ApplyChanges();

            TextureManager = new TextureManager();

            Map = new TerrainMap(EList);
            EList = new EntityList(Map);

            Cam = new Camera(EList);
            Cam.Zoom = GlobalConstants.GAME_SCALE;

            Player p = new Player(250, 100, EList, Cam, Map);
            EList.AddObject(p, ObjectType.ID.Actor);

            for(int i = 0; i < 1; i++)
            {
                BasicEnemy e = new BasicEnemy(300 + i * 40, 300, EList, Map);
                EList.AddObject(e, ObjectType.ID.Actor);
            }
            
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureManager.LoadTextures(Content);

            Map.LoadMap();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float GTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Cam.UpdateCamera(GTime);
            EList.Tick(GTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, Cam.Transform);

            MouseState mouse = Mouse.GetState();
            EList.Draw(_spriteBatch);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
