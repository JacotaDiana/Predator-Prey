using ActressMas;
using System;
using System.Collections.Generic;

namespace PredatorPrey
{
    public class GrassAgent : CreatureAgent
    {
        private int _lastEaten;

        public override void Setup()
        {
            _turnsSurvived = 0;
            _lastEaten = 0;

            _worldEnv = (World)this.Environment;

            if (Utils.Verbose)
                Console.WriteLine("Grass {0} started in ({1},{2})", this.Name, Line, Column);
        }

        public override void Act(Queue<Message> messages)
        {
            if(messages.Count > 0)
            {
                Message message = messages.Dequeue();
                if (Utils.Verbose)
                    Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                GrassAction();
                Send("scheduler", "done");
            }
        }

        private void GrassAction()
        {
            _turnsSurvived++;
            _lastEaten++;

            // eat
            bool success = TryToEat();
            if (success)
                _lastEaten = 0;

            // move
            if (!success)
                TryToMove(); // implemented in base class CreatureAgent

            // breed
            if (_turnsSurvived >= Utils.NoTurnsUntilGrassBreeds)
            {
                if (TryToBreed()) // implemented in base class CreatureAgent
                    _turnsSurvived = 0;
            }

            // starve
            if (_lastEaten >= Utils.NoTurnsUntilGrassStarves)
                Die();
        }

        private bool TryToEat()
        {
            List<Direction> allowedDirections = new List<Direction>();
            int newLine, newColumn;

            for (int i = 0; i < 4; i++)
            {
                if (_worldEnv.ValidMovement(this, (Direction)i, CellState.Wolf, out newLine, out newColumn))
                    allowedDirections.Add((Direction)i);
            }

            if (allowedDirections.Count == 0)
                return false;

            int r = Utils.Rand.Next(allowedDirections.Count);
            _worldEnv.ValidMovement(this, allowedDirections[r], CellState.Wolf, out newLine, out newColumn);

            _worldEnv.Eat(this, newLine, newColumn);

            return true;
        }

        private void Die()
        {
            // removing the grass

            if (Utils.Verbose)
                Console.WriteLine("Removing " + this.Name);

            this.Stop();
            _worldEnv.Die(this);
        }
    }
}