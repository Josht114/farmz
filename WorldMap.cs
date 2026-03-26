using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarmGame.Core;
using System;

namespace FarmGame.Entities
{
    public class WorldMap
    {
        public int Width  { get; }
        public int Height { get; }

        private Tile[,] _tiles;
        private readonly Random _rng = new Random(42);

        public WorldMap(int width, int height)
        {
            Width  = width;
            Height = height;
            _tiles = new Tile[width, height];

            Generate();
        }

        // ------------------------------------------------------------------ generation

        private void Generate()
        {
            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                _tiles[x, y] = Tile.Default;

                // Scatter a few trees and a water strip
                if (x == 0 || x == Width - 1 || y == 0 || y == Height - 1)
                    _tiles[x, y].Type = TileType.Tree;
                else if (x > 5 && x < 9 && y > 5 && y < 25)
                    _tiles[x, y].Type = TileType.Water;   // pond
                else if (_rng.NextDouble() < 0.05)
                    _tiles[x, y].Type = TileType.Tree;
            }
        }

        // ------------------------------------------------------------------ public API

        public ref Tile GetTile(int x, int y) => ref _tiles[x, y];

        public bool InBounds(int x, int y) =>
            x >= 0 && x < Width && y >= 0 && y < Height;

        /// <summary>Attempt to plow a tile. Returns true on success.</summary>
        public bool Plow(int x, int y)
        {
            if (!InBounds(x, y) || !_tiles[x, y].CanPlow) return false;
            _tiles[x, y].Type = TileType.Plowed;
            return true;
        }

        /// <summary>Attempt to plant seeds on a tile. Returns true on success.</summary>
        public bool Plant(int x, int y)
        {
            if (!InBounds(x, y) || !_tiles[x, y].CanSeed) return false;
            _tiles[x, y].Crop         = CropStage.Seedling;
            _tiles[x, y].GrowTimer    = 0f;
            _tiles[x, y].GrowDuration = 20f;
            return true;
        }

        /// <summary>Attempt to harvest a tile. Returns true on success.</summary>
        public bool Harvest(int x, int y)
        {
            if (!InBounds(x, y) || !_tiles[x, y].CanHarvest) return false;
            _tiles[x, y].Crop      = CropStage.None;
            _tiles[x, y].GrowTimer = 0f;
            _tiles[x, y].Type      = TileType.Dirt;
            return true;
        }

        // ------------------------------------------------------------------ update

        public void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                if (_tiles[x, y].Crop == CropStage.None ||
                    _tiles[x, y].Crop == CropStage.Ready) continue;

                _tiles[x, y].GrowTimer += delta;
                float progress = _tiles[x, y].GrowTimer / _tiles[x, y].GrowDuration;

                _tiles[x, y].Crop = progress switch
                {
                    < 0.4f => CropStage.Seedling,
                    < 1.0f => CropStage.Growing,
                    _      => CropStage.Ready
                };
            }
        }

        // ------------------------------------------------------------------ draw

        public void Draw(SpriteBatch sb)
        {
            int ts = Game1.TILE_SIZE;

            for (int x = 0; x < Width; x++)
            for (int y = 0; y < Height; y++)
            {
                var rect = new Rectangle(x * ts, y * ts, ts, ts);
                var tile = _tiles[x, y];

                // Base tile
                Texture2D baseTex = tile.Type switch
                {
                    TileType.Dirt   => TextureManager.DirtTex,
                    TileType.Plowed => TextureManager.PlowedTex,
                    TileType.Water  => TextureManager.WaterTex,
                    TileType.Tree   => TextureManager.GrassTex,   // grass under tree
                    _               => TextureManager.GrassTex
                };
                sb.Draw(baseTex, rect, Color.White);

                // Tree overlay
                if (tile.Type == TileType.Tree)
                    sb.Draw(TextureManager.TreeTex, rect, Color.White);

                // Crop overlay
                if (tile.Crop == CropStage.Seedling || tile.Crop == CropStage.Growing)
                    sb.Draw(TextureManager.SeedlingTex, rect, Color.White);
                else if (tile.Crop == CropStage.Ready)
                    sb.Draw(TextureManager.CropReadyTex, rect, Color.White);
            }
        }
    }
}
