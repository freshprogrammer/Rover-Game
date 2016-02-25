using System;

namespace RoverGame
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (RoverGame game = new RoverGame())
            {
                game.Run();
            }
        }
    }
#endif
}

