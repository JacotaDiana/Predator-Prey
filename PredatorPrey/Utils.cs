using System;
using System.Linq;

namespace PredatorPrey
{
    public class Utils
    {
        public static int GridSize = 16;
        public static int NoTurns = 1000;
        public static int NoSheep = 20;
        public static int NoWolves = 10;
        public static bool Verbose = false;
        public static Random Rand = new Random();
        public static int DelayBetweenTurns = 100;
        public static int NoTurnsUntilSheepBreeds = 3;
        public static int NoTurnsUntilWolfBreeds = 8;
        public static int NoTurnsUntilWolfStarves = 3;

        public static int[] RandomPermutation(int n)
        {
            int[] numbers = new int[n];
            for (int i = 0; i < n; i++)
                numbers[i] = i;
            int[] randPerm = numbers.OrderBy(x => Rand.Next()).ToArray();
            return randPerm;
        }
    }
}