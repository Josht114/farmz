using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarmGame.Core;
using FarmGame.Entities;
using FarmGame.Systems;

namespace FarmGame.UI
{
    /// <summary>
    /// Heads-up display drawn in screen space (no camera transform).
    /// Shows: clock, tool, inventory, gold.
    /// Uses coloured rectangles as placeholders until a SpriteFont is loaded.
    /// </summary>
    public class HUD
    {
        private readonly Player     _player;
        private readonly TimeSystem _time;

        private SpriteFont? _font;   // null until a font is loaded

        public HUD(Player player, TimeSystem time)
        {
            _player = player;
            _time   = time;
        }

        /// <summary>Call after Content.Load to optionally supply a font.</summary>
        public void SetFont(SpriteFont font) => _font = font;

        public void Draw(SpriteBatch sb, GraphicsDevice gd)
        {
            int sw = gd.Viewport.Width;
            int sh = gd.Viewport.Height;

            // ---- Tool bar background ----
            DrawRect(sb, new Rectangle(10, sh - 60, 200, 50), new Color(0, 0, 0, 160));

            // ---- Tool indicators ----
            string[] toolLabels = { "[1] Hoe", "[2] Seeds", "[3] Scythe" };
            for (int i = 0; i < toolLabels.Length; i++)
            {
                bool active = (int)_player.EquippedTool == i;
                DrawRect(sb, new Rectangle(15 + i * 65, sh - 55, 60, 40),
                         active ? Color.Gold : new Color(80, 80, 80));

                if (_font != null)
                    sb.DrawString(_font, toolLabels[i],
                                  new Vector2(18 + i * 65, sh - 48), Color.White, 0,
                                  Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            }

            // ---- Clock / season panel ----
            DrawRect(sb, new Rectangle(sw - 220, 10, 210, 40), new Color(0, 0, 0, 160));
            if (_font != null)
                sb.DrawString(_font, _time.TimeString,
                              new Vector2(sw - 215, 18), Color.White, 0,
                              Vector2.Zero, 0.65f, SpriteEffects.None, 0);

            // ---- Inventory / gold ----
            DrawRect(sb, new Rectangle(10, 10, 180, 40), new Color(0, 0, 0, 160));
            if (_font != null)
            {
                int seeds   = _player.Inventory.ContainsKey("Seeds")  ? _player.Inventory["Seeds"]  : 0;
int carrots = _player.Inventory.ContainsKey("Carrot") ? _player.Inventory["Carrot"] : 0;
string inv  = $"Seeds:{seeds}  Carrots:{carrots}  G:{_player.Gold}";
                sb.DrawString(_font, inv,
                              new Vector2(15, 18), Color.White, 0,
                              Vector2.Zero, 0.65f, SpriteEffects.None, 0);
            }

            // ---- Controls hint ----
            DrawRect(sb, new Rectangle(10, sh - 120, 240, 55), new Color(0, 0, 0, 120));
            if (_font != null)
            {
                sb.DrawString(_font, "WASD: Move   E: Interact",
                              new Vector2(15, sh - 115), Color.LightGray, 0,
                              Vector2.Zero, 0.6f, SpriteEffects.None, 0);
                sb.DrawString(_font, "1/2/3: Hoe / Seeds / Scythe",
                              new Vector2(15, sh - 98), Color.LightGray, 0,
                              Vector2.Zero, 0.6f, SpriteEffects.None, 0);
            }
        }

        private void DrawRect(SpriteBatch sb, Rectangle rect, Color color)
        {
            sb.Draw(TextureManager.Pixel, rect, color);
        }
    }
}
