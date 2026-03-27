using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarmGame.Entities;

namespace FarmGame.Systems
{
    public class InputHandler
    {
        private readonly Player _player;
        private KeyboardState _prevKeys;
        private KeyboardState _currKeys;

        public bool InteractPressed { get; private set; }

        public InputHandler(Player player)
        {
            _player = player;
        }

        public void Update(GameTime gameTime)
        {
            _prevKeys = _currKeys;
            _currKeys = Keyboard.GetState();  // ONE snapshot per frame

            // Movement
            var dir = Vector2.Zero;
            if (_currKeys.IsKeyDown(Keys.W) || _currKeys.IsKeyDown(Keys.Up))    dir.Y -= 1;
            if (_currKeys.IsKeyDown(Keys.S) || _currKeys.IsKeyDown(Keys.Down))  dir.Y += 1;
            if (_currKeys.IsKeyDown(Keys.A) || _currKeys.IsKeyDown(Keys.Left))  dir.X -= 1;
            if (_currKeys.IsKeyDown(Keys.D) || _currKeys.IsKeyDown(Keys.Right)) dir.X += 1;
            if (dir != Vector2.Zero) dir.Normalize();
            _player.SetVelocity(dir);

            // Tool switch
            if (JustPressed(Keys.D1)) _player.EquippedTool = Tool.Hoe;
            if (JustPressed(Keys.D2)) _player.EquippedTool = Tool.Seeds;
            if (JustPressed(Keys.D3)) _player.EquippedTool = Tool.Scythe;

            // Interact flag — set once per frame, read by Game1
            InteractPressed = JustPressed(Keys.E);
        }

        private bool JustPressed(Keys key) =>
            _currKeys.IsKeyDown(key) && _prevKeys.IsKeyUp(key);
    }
}