using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using FarmGame.Entities;

namespace FarmGame.Systems
{
    /// <summary>
    /// Reads keyboard/gamepad input and drives the Player.
    ///
    /// Controls:
    ///   WASD / Arrow keys  - Move
    ///   E                  - Interact (use equipped tool)
    ///   1 / 2 / 3          - Switch tool  (Hoe / Seeds / Scythe)
    /// </summary>
    public class InputHandler
    {
        private readonly Player _player;
        private KeyboardState  _prevKeys;

        public InputHandler(Player player)
        {
            _player = player;
        }

        public void Update(GameTime gameTime)
        {
            var keys = Keyboard.GetState();

            // --- Movement ---
            var dir = Vector2.Zero;
            if (keys.IsKeyDown(Keys.W) || keys.IsKeyDown(Keys.Up))    dir.Y -= 1;
            if (keys.IsKeyDown(Keys.S) || keys.IsKeyDown(Keys.Down))  dir.Y += 1;
            if (keys.IsKeyDown(Keys.A) || keys.IsKeyDown(Keys.Left))  dir.X -= 1;
            if (keys.IsKeyDown(Keys.D) || keys.IsKeyDown(Keys.Right)) dir.X += 1;

            if (dir != Vector2.Zero) dir.Normalize();
            _player.SetVelocity(dir);

            // --- Tool switch (tap) ---
            if (JustPressed(keys, Keys.D1)) _player.EquippedTool = Tool.Hoe;
            if (JustPressed(keys, Keys.D2)) _player.EquippedTool = Tool.Seeds;
            if (JustPressed(keys, Keys.D3)) _player.EquippedTool = Tool.Scythe;

            // --- Interact (tap) ---
            // Interaction is handled by Game1 passing the world reference in,
            // but InputHandler calls into Player which already holds the logic.
            // We wire it here with a world reference-free wrapper; Game1 passes
            // the world to Player.Interact() directly. See SystemNote below.

            _prevKeys = keys;
        }

        // Returns true if key was just pressed this frame
        private bool JustPressed(KeyboardState cur, Keys key) =>
            cur.IsKeyDown(key) && _prevKeys.IsKeyUp(key);

        // ------------------------------------------------------------------ 
        // SystemNote: To avoid a circular reference, InputHandler exposes the
        // raw interact-pressed flag and Game1 calls player.Interact(world).
        // ------------------------------------------------------------------ 

        public bool InteractJustPressed()
        {
            var keys = Keyboard.GetState();
            return JustPressed(keys, Keys.E);
        }
    }
}
