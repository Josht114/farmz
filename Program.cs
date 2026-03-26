using System;

namespace FarmGame
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using var game = new Core.Game1();
            game.Run();
        }
    }
}
