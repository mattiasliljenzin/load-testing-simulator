using System;

namespace RequestSimulation
{
    public class Program
    {
        static void Main(string[] args)
        {
            new App().Run().Wait();

            Console.WriteLine(" ");
            Console.WriteLine("Good-bye world!");
        }
    }

    
}
