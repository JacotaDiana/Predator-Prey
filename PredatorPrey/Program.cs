namespace PredatorPrey
{
    public class Program
    {
        private static void Main(string[] args)
        {
            var worldEnv = new World(0, 0, false); // derived from ActressMas.Environment();

            int noAgents = Utils.GridSize * Utils.GridSize;

            worldEnv.InitWorldMap();

            int[] randVect = Utils.RandomPermutation(noAgents);

            for (int i = 0; i < Utils.NoWolves; i++)
            {
                var a = new WolfAgent();
                worldEnv.Add(a, worldEnv.CreateName(a)); // unique name
                worldEnv.AddAgentToMap(a, randVect[i]);
            }

            for (int i = Utils.NoWolves; i < Utils.NoWolves + Utils.NoSheep; i++)
            {
                var a = new SheepAgent();
                worldEnv.Add(a, worldEnv.CreateName(a));
                worldEnv.AddAgentToMap(a, randVect[i]);
            }
            
            var s = new SchedulerAgent();
            worldEnv.Add(s, "scheduler");


            worldEnv.Start();
        }
    }
}