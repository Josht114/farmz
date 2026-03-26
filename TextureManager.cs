using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace FarmGame.Core
{
    /// <summary>
    /// Central store for all game textures.
    /// Falls back to coloured 8×8 placeholders when content files are absent.
    /// </summary>
    public static class TextureManager
    {
        public static Texture2D Pixel        { get; private set; }   // 1×1 white pixel
        public static Texture2D PlayerTex    { get; private set; }
        public static Texture2D GrassTex     { get; private set; }
        public static Texture2D DirtTex      { get; private set; }
        public static Texture2D PlowedTex    { get; private set; }
        public static Texture2D WaterTex     { get; private set; }
        public static Texture2D SeedlingTex  { get; private set; }
        public static Texture2D CropReadyTex { get; private set; }
        public static Texture2D TreeTex      { get; private set; }

        public static void Load(ContentManager content, GraphicsDevice gd)
        {
            Pixel = MakePixel(gd, Color.White);

            // Try loading real assets; fall back to solid-colour placeholders
            PlayerTex    = TryLoad(content, "player",     gd, Color.Blue);
            GrassTex     = TryLoad(content, "grass",      gd, Color.LimeGreen);
            DirtTex      = TryLoad(content, "dirt",       gd, Color.SaddleBrown);
            PlowedTex    = TryLoad(content, "plowed",     gd, Color.Peru);
            WaterTex     = TryLoad(content, "water",      gd, Color.CornflowerBlue);
            SeedlingTex  = TryLoad(content, "seedling",   gd, Color.YellowGreen);
            CropReadyTex = TryLoad(content, "crop_ready", gd, Color.Gold);
            TreeTex      = TryLoad(content, "tree",       gd, Color.DarkGreen);
        }

        // ------------------------------------------------------------------ helpers

        private static Texture2D TryLoad(ContentManager content, string name,
                                         GraphicsDevice gd, Color fallback)
        {
            try   { return content.Load<Texture2D>(name); }
            catch { return MakeSolid(gd, fallback, 8, 8); }
        }

        private static Texture2D MakePixel(GraphicsDevice gd, Color color)
        {
            var tex = new Texture2D(gd, 1, 1);
            tex.SetData(new[] { color });
            return tex;
        }

        private static Texture2D MakeSolid(GraphicsDevice gd, Color color, int w, int h)
        {
            var tex  = new Texture2D(gd, w, h);
            var data = new Color[w * h];
            for (int i = 0; i < data.Length; i++) data[i] = color;
            tex.SetData(data);
            return tex;
        }
    }
}
