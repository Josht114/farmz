using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarmGame.Systems;
using FarmGame.Entities;

namespace FarmGame.Core
{
    public class Game1 : Game
    {
        // --- Graphics ---
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // --- Game Systems ---
        private WorldMap _worldMap;
        private Player _player;
        private InputHandler _inputHandler;
        private TimeSystem _timeSystem;
        private UI.HUD _hud;

        // --- Camera ---
        public static Matrix CameraTransform { get; private set; }
        private Vector2 _cameraPosition;

        public const int SCREEN_WIDTH  = 1280;
        public const int SCREEN_HEIGHT = 720;
        public const int TILE_SIZE     = 32;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth  = SCREEN_WIDTH;
            _graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
        }

        protected override void Initialize()
        {
            _worldMap     = new WorldMap(40, 30);          // 40×30 tile map
            _player       = new Player(new Vector2(10, 10));
            _inputHandler = new InputHandler(_player);
            _timeSystem   = new TimeSystem();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Load textures (provide placeholder 1x1 textures if assets missing)
            TextureManager.Load(Content, GraphicsDevice);

            _hud = new UI.HUD(_player, _timeSystem);
        }

        protected override void Update(GameTime gameTime)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            _inputHandler.Update(gameTime);

            // Wire interact: E key → player uses tool on world
            if (_inputHandler.InteractJustPressed())
                _player.Interact(_worldMap);

            _player.Update(gameTime, _worldMap);
            _worldMap.Update(gameTime);
            _timeSystem.Update(gameTime);

            UpdateCamera();

            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            // Centre camera on player
            _cameraPosition = new Vector2(
                _player.Position.X * TILE_SIZE - SCREEN_WIDTH  / 2f,
                _player.Position.Y * TILE_SIZE - SCREEN_HEIGHT / 2f
            );

            CameraTransform = Matrix.CreateTranslation(
                -_cameraPosition.X, -_cameraPosition.Y, 0f
            );
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            // World layer (camera-transformed)
            _spriteBatch.Begin(transformMatrix: CameraTransform, samplerState: SamplerState.PointClamp);
            _worldMap.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            // HUD layer (screen-space, no transform)
            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _hud.Draw(_spriteBatch, GraphicsDevice);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
