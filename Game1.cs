using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarmGame.Systems;
using FarmGame.Entities;
using FarmGame.Core;

namespace FarmGame.Core
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private WorldMap     _worldMap;
        private Player       _player;
        private InputHandler _inputHandler;
        private TimeSystem   _timeSystem;
        private UI.HUD       _hud;

        public static Matrix CameraTransform { get; private set; }
        private Vector2 _cameraPosition;

        public const int SCREEN_WIDTH  = 1280;
        public const int SCREEN_HEIGHT = 720;
        public const int TILE_SIZE     = 32;

        // Night overlay
        private Texture2D _pixel;
        private KeyboardState _prevKeys;

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
            _worldMap     = new WorldMap(40, 30);
            _player       = new Player(new Vector2(10, 10));
            _inputHandler = new InputHandler(_player);
            _timeSystem   = new TimeSystem();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            TextureManager.Load(Content, GraphicsDevice);
            _hud = new UI.HUD(_player, _timeSystem);

            // 1x1 white pixel for the overlay
            _pixel = new Texture2D(GraphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });

            try
            {
                var font = Content.Load<Microsoft.Xna.Framework.Graphics.SpriteFont>("DefaultFont");
                _hud.SetFont(font);
            }
            catch { }
        }

        protected override void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            if (keys.IsKeyDown(Keys.Escape)) Exit();

            // Sleep key - Z
            if (keys.IsKeyDown(Keys.Z) && _prevKeys.IsKeyUp(Keys.Z))
                _timeSystem.Sleep();

            _prevKeys = keys;

            _inputHandler.Update(gameTime);

            if (_inputHandler.InteractPressed)
                _player.Interact(_worldMap);

            _player.Update(gameTime, _worldMap);
            _worldMap.Update(gameTime);
            _timeSystem.Update(gameTime);

            UpdateCamera();
            base.Update(gameTime);
        }

        private void UpdateCamera()
        {
            _cameraPosition = new Vector2(
                _player.Position.X * TILE_SIZE - SCREEN_WIDTH  / 2f,
                _player.Position.Y * TILE_SIZE - SCREEN_HEIGHT / 2f
            );
            CameraTransform = Matrix.CreateTranslation(-_cameraPosition.X, -_cameraPosition.Y, 0f);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.ForestGreen);

            // World layer
            _spriteBatch.Begin(transformMatrix: CameraTransform, samplerState: SamplerState.PointClamp);
            _worldMap.Draw(_spriteBatch);
            _player.Draw(_spriteBatch);
            _spriteBatch.End();

            // Night overlay — screen-space dark blue tint
            // DaylightAmount 1.0 = transparent, 0.0 = 75% dark
            float darkness = 1f - _timeSystem.DaylightAmount;
            byte alpha = (byte)(darkness * 190);  // max 190/255 opacity so it's never pitch black
            Color overlayColor = new Color((byte)10, (byte)10, (byte)40, alpha);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_pixel,
                new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),
                overlayColor);

            // HUD drawn on top of overlay so it stays readable
            _hud.Draw(_spriteBatch, GraphicsDevice);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}