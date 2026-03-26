using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarmGame.Core;
using System.Collections.Generic;

namespace FarmGame.Entities
{
    public enum Tool { Hoe, Seeds, Scythe }

    public class Player
    {
        // ------------------------------------------------------------------ state

        public Vector2 Position     { get; set; }   // tile-space position
        public Tool    EquippedTool { get; set; } = Tool.Hoe;

        // Simple inventory: item name → quantity
        public Dictionary<string, int> Inventory { get; } = new()
        {
            ["Seeds"]  = 10,
            ["Carrot"] = 0
        };

        public int Gold { get; set; } = 100;

        // ------------------------------------------------------------------ movement

        private const float MOVE_SPEED = 5f;        // tiles per second
        private Vector2 _velocity;

        // ------------------------------------------------------------------ ctor

        public Player(Vector2 startTile)
        {
            Position = startTile;
        }

        // ------------------------------------------------------------------ update

        public void Update(GameTime gameTime, WorldMap world)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Apply velocity
            Vector2 next = Position + _velocity * delta * MOVE_SPEED;

            // Tile-level collision: only move if destination is walkable
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

        // ------------------------------------------------------------------ interact

        /// <summary>Use the equipped tool on the tile the player is standing on.</summary>
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
                            Inventory.GetValueOrDefault("Carrot") + 1;
                        return true;
                    }
                    return false;
            }
            return false;
        }

        // ------------------------------------------------------------------ draw

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
