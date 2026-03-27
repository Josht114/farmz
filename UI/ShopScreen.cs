using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using FarmGame.Core;
using FarmGame.Entities;

namespace FarmGame.UI
{
    public class ShopScreen
    {
        private readonly Player _player;
        private SpriteFont?     _font;

        public bool IsOpen { get; private set; } = false;

        private KeyboardState _prevKeys;

        // Shop stock: item name, buy price (0 = not for sale), sell price
        private readonly (string Name, int BuyPrice, int SellPrice)[] _items =
        {
            ("Seeds",  10, 0),    // buy only
            ("Carrot",  0, 15),   // sell only
        };

        private int _selectedIndex = 0;

        public ShopScreen(Player player, SpriteFont? font = null)
        {
            _player = player;
            _font   = font;
        }

        public void SetFont(SpriteFont font) => _font = font;

        public void Update()
        {
            var keys = Keyboard.GetState();

            // Toggle open/close with B
            if (JustPressed(keys, Keys.B))
                IsOpen = !IsOpen;

            if (IsOpen)
            {
                // Navigate
                if (JustPressed(keys, Keys.Up))
                    _selectedIndex = (_selectedIndex - 1 + _items.Length) % _items.Length;
                if (JustPressed(keys, Keys.Down))
                    _selectedIndex = (_selectedIndex + 1) % _items.Length;

                // Buy with Enter
                if (JustPressed(keys, Keys.Enter))
                {
                    var item = _items[_selectedIndex];
                    if (item.BuyPrice > 0)
                        _player.BuySeeds(5, item.BuyPrice);   // buy 5 at a time
                }

                // Sell with S
                if (JustPressed(keys, Keys.S))
                {
                    var item = _items[_selectedIndex];
                    if (item.SellPrice > 0)
                        _player.SellAll(item.Name);
                }

                // Close with Escape or B
                if (JustPressed(keys, Keys.Escape))
                    IsOpen = false;
            }

            _prevKeys = keys;
        }

        public void Draw(SpriteBatch sb, GraphicsDevice gd)
        {
            if (!IsOpen) return;

            int sw = gd.Viewport.Width;
            int sh = gd.Viewport.Height;

            // Dim background
            DrawRect(sb, new Rectangle(0, 0, sw, sh), new Color((byte)0, (byte)0, (byte)0, (byte)160));

            // Shop panel
            int pw = 400, ph = 340;
            int px = sw / 2 - pw / 2;
            int py = sh / 2 - ph / 2;
            DrawRect(sb, new Rectangle(px, py, pw, ph),         new Color((byte)30,  (byte)20,  (byte)10,  (byte)240));
            DrawRect(sb, new Rectangle(px, py, pw, 4),          Color.Gold);
            DrawRect(sb, new Rectangle(px, py + ph - 4, pw, 4), Color.Gold);
            DrawRect(sb, new Rectangle(px, py, 4, ph),          Color.Gold);
            DrawRect(sb, new Rectangle(px + pw - 4, py, 4, ph), Color.Gold);

            if (_font == null) return;

            // Title
            sb.DrawString(_font, "~~ SHOP ~~",
                new Vector2(px + pw / 2 - 45, py + 16), Color.Gold,
                0, Vector2.Zero, 1f, SpriteEffects.None, 0);

            sb.DrawString(_font, $"Gold: {_player.Gold}g",
                new Vector2(px + 16, py + 48), Color.Yellow,
                0, Vector2.Zero, 0.85f, SpriteEffects.None, 0);

            // Column headers
            sb.DrawString(_font, "Item",      new Vector2(px + 40,  py + 80), Color.LightGray, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            sb.DrawString(_font, "Buy (x5)",  new Vector2(px + 180, py + 80), Color.LightGray, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            sb.DrawString(_font, "Sell all",  new Vector2(px + 290, py + 80), Color.LightGray, 0, Vector2.Zero, 0.75f, SpriteEffects.None, 0);
            DrawRect(sb, new Rectangle(px + 16, py + 98, pw - 32, 2), new Color((byte)80, (byte)60, (byte)20, (byte)255));

            // Items
            for (int i = 0; i < _items.Length; i++)
            {
                var item    = _items[i];
                bool sel    = i == _selectedIndex;
                int  rowY   = py + 110 + i * 48;
                Color rowBg = sel ? new Color((byte)80, (byte)60, (byte)10, (byte)200)
                                  : new Color((byte)0,  (byte)0,  (byte)0,  (byte)0);
                DrawRect(sb, new Rectangle(px + 16, rowY - 4, pw - 32, 40), rowBg);

                // Selector arrow
                if (sel)
                    sb.DrawString(_font, "▶", new Vector2(px + 18, rowY + 4),
                        Color.Gold, 0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

                // Item name + held qty
                int held = _player.Inventory.ContainsKey(item.Name) ? _player.Inventory[item.Name] : 0;
                sb.DrawString(_font, $"{item.Name} ({held})",
                    new Vector2(px + 40, rowY + 4), Color.White,
                    0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

                // Buy price
                string buyStr = item.BuyPrice > 0 ? $"{item.BuyPrice * 5}g" : "—";
                Color  buyCol = item.BuyPrice > 0 && _player.Gold >= item.BuyPrice * 5
                                ? Color.LightGreen : Color.Gray;
                sb.DrawString(_font, buyStr,
                    new Vector2(px + 190, rowY + 4), buyCol,
                    0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);

                // Sell price
                string sellStr = item.SellPrice > 0 ? $"{held * item.SellPrice}g" : "—";
                Color  sellCol = item.SellPrice > 0 && held > 0 ? Color.Gold : Color.Gray;
                sb.DrawString(_font, sellStr,
                    new Vector2(px + 295, rowY + 4), sellCol,
                    0, Vector2.Zero, 0.8f, SpriteEffects.None, 0);
            }

            // Footer controls
            DrawRect(sb, new Rectangle(px + 16, py + ph - 60, pw - 32, 2),
                new Color((byte)80, (byte)60, (byte)20, (byte)255));
            sb.DrawString(_font, "↑↓ Navigate    Enter: Buy x5    S: Sell all    B: Close",
                new Vector2(px + 20, py + ph - 48), Color.LightGray,
                0, Vector2.Zero, 0.6f, SpriteEffects.None, 0);
        }

        private bool JustPressed(KeyboardState cur, Keys key) =>
            cur.IsKeyDown(key) && _prevKeys.IsKeyUp(key);

        private void DrawRect(SpriteBatch sb, Rectangle rect, Color color) =>
            sb.Draw(TextureManager.Pixel, rect, color);
    }
}