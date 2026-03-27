using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarmGame.Core;
using System.Collections.Generic;

namespace FarmGame.Entities
{
    public enum Tool { Hoe, Seeds, Scythe }

    public class Player
    {
        public Vector2 Position     { get; set; }
        public Tool    EquippedTool { get; set; } = Tool.Hoe;

        public Dictionary<string, int> Inventory { get; } = new()
        {
            ["Seeds"]  = 10,
            ["Carrot"] = 0
        };

        public int Gold { get; set; } = 100;

        // Crop sell prices
        public static readonly Dictionary<string, int> SellPrices = new()
        {
            ["Carrot"] = 15,
        };

        private const float MOVE_SPEED = 5f;
        private Vector2 _velocity;

        public Player(Vector2 startTile)
        {
            Position = startTile;
        }

        public void Update(GameTime gameTime, WorldMap world)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Vector2 next = Position + _velocity * delta * MOVE_SPEED;

            int tx = (int)next.X;
            int ty = (int)next.Y;

            if (world.InBounds(tx, ty) && world.GetTile(tx, ty).IsWalkable)
                Position = next;
            else if (world.InBounds((int)next.X, (int)Position.Y) &&
                     world.GetTile((int)next.X, (int)Position.Y).IsWalkable)
                Position = new Vector2(next.X, Position.Y);
            else if (world.InBounds((int)Position.X, (int)next.Y) &&
                     world.GetTile((int)Position.X, (int)next.Y).IsWalkable)
                Position = new Vector2(Position.X, next.Y);
        }

        public void SetVelocity(Vector2 v) => _velocity = v;

        public bool Interact(WorldMap world)
        {
            int tx = (int)Position.X;
            int ty = (int)Position.Y;

            switch (EquippedTool)
            {
                case Tool.Hoe:
                    return world.Plow(tx, ty);

                case Tool.Seeds:
                    if (Inventory["Seeds"] > 0 && world.Plant(tx, ty))
                    {
                        Inventory["Seeds"]--;
                        return true;
                    }
                    return false;

                case Tool.Scythe:
                    if (world.Harvest(tx, ty))
                    {
                        Inventory["Carrot"] =
                            (Inventory.ContainsKey("Carrot") ? Inventory["Carrot"] : 0) + 1;
                        return true;
                    }
                    return false;
            }
            return false;
        }

        /// <summary>Sell all of a crop. Returns gold earned, or 0 if none to sell.</summary>
        public int SellAll(string cropName)
        {
            if (!Inventory.ContainsKey(cropName) || Inventory[cropName] <= 0) return 0;
            if (!SellPrices.ContainsKey(cropName)) return 0;

            int qty      = Inventory[cropName];
            int earned   = qty * SellPrices[cropName];
            Gold        += earned;
            Inventory[cropName] = 0;
            return earned;
        }

        /// <summary>Buy seeds. Returns true on success.</summary>
        public bool BuySeeds(int qty, int priceEach)
        {
            int total = qty * priceEach;
            if (Gold < total) return false;
            Gold -= total;
            Inventory["Seeds"] = (Inventory.ContainsKey("Seeds") ? Inventory["Seeds"] : 0) + qty;
            return true;
        }

        public void Draw(SpriteBatch sb)
        {
            int ts = Game1.TILE_SIZE;
            var rect = new Rectangle(
                (int)(Position.X * ts),
                (int)(Position.Y * ts),
                ts, ts);
            sb.Draw(TextureManager.PlayerTex, rect, Color.White);
        }
    }
}