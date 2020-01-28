using System;

namespace MiniChess
{
    class Program
    {
        static void Main(string[] args)
        {
            GameState game = new GameState();
            game.Game();
            Console.ReadLine();
        }
    }
}
