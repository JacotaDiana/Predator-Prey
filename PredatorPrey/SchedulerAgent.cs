using ActressMas;
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading;

namespace PredatorPrey
{
    public class SchedulerAgent : TurnBasedAgent
    {
        private int _turn;
        private World _worldEnv;

        public override void Setup()
        {
            _turn = 1;
            _worldEnv = (World)this.Environment;

            _worldEnv.StartNewTurn();

            string name = _worldEnv.GetNextCreature();

            if (Utils.Verbose)
                Console.WriteLine("\t\tRunning " + name);

            Send(name, "act");
        }

        public override void Act(Queue<Message> messages)
        {
            if(messages.Count > 0)
            {
                Message message = messages.Dequeue();

                if (Utils.Verbose)
                    Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                string next = _worldEnv.GetNextCreature();

                if (Utils.Verbose)
                    Console.WriteLine("\t\tNext is: " + next);

                if (next == "") // all have moved
                {
                    int noWolves, noSheep, noGrass;
                    _worldEnv.CountCreatures(out noWolves, out noSheep, out noGrass);

                    string toWrite = String.Format("Turn {0}:    {1} sheep    {2} wolves    {3} grass", _turn, noSheep, noWolves, noGrass);
                    Console.WriteLine(toWrite);
                    WriteToUI(toWrite);
                    
                    _worldEnv.UpdateUI(_turn, noSheep, noWolves, noGrass);
                    
                    if(_turn == Utils.NoTurns)
                    {
                        string text = String.Format("\r\nSimulation finished. Turns expired.");
                        Console.WriteLine(text);
                        WriteToUI(text);
                        //this.Environment.StopAll();
                        return;
                    }

                    if (noWolves == 0 && noSheep == 0 && noGrass == 0)
                    {
                        string text = String.Format("\r\nSimulation finished. Both populations extinct.");
                        Console.WriteLine(text);
                        WriteToUI(text);
                        //this.Environment.StopAll();
                        return;
                    }

                    if (noSheep == Utils.GridSize * Utils.GridSize || noWolves == Utils.GridSize * Utils.GridSize || noGrass == Utils.GridSize * Utils.GridSize)
                    {
                        string text = String.Format("\r\nSimulation finished. One population increased uncontrollably.");
                        Console.WriteLine(text);
                        WriteToUI(text);
                        //this.Environment.StopAll();
                        return;
                    }

                    _turn++; // next turn
                    
                    Thread.Sleep(Utils.DelayBetweenTurns);

                    _worldEnv.StartNewTurn();

                    string name = _worldEnv.GetNextCreature();

                    if (Utils.Verbose)
                        Console.WriteLine("\t\tNext turn: Running " + name);

                    Send(name, "act");
                }
                else
                    Send(next, "act");
            }
        }

        public void WriteToUI(string text)
        {
            _worldEnv.WriteToUI(text);
        }
    }
}