using ActressMas;
using System;
using System.Collections.Generic;

namespace PredatorPrey
{
    public class WolfAgent : CreatureAgent
    {
        private int _lastEaten;

        public override void Setup()
        {
            _turnsSurvived = 0;
            _lastEaten = 0;

            _worldEnv = (World)this.Environment;

            if (Utils.Verbose)
                Console.WriteLine("Wolf {0} started in ({1},{2})", this.Name, Line, Column);
        }

        public override void Act(Queue<Message> messages)
        {
            if(messages.Count > 0)
            {
                Message message = messages.Dequeue();
                if (Utils.Verbose)
                    Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                WolfAction();
                Send("scheduler", "done");
            }
        }

        private void WolfAction()
        {
            /*
                • Move. For every time step, the wolf will move to an adjacent cell containing a sheep and eat the
                sheep. If there are no sheep in adjoining cells, the wolf moves according to the same rules as the
                sheep. Note that a wolf cannot eat other wolves.
                • Breed. If a wolf survives for NoTurnsUntilWolfBreed time steps, at the end of the time step it will spawn off a new
                wolf in the same manner as the sheep.
                • Starve. If a wolf has not eaten a sheep within NoTurnsUntilWolfStarves time steps, at the end of the third time step it
                will starve and die. The wolf should then be removed from the grid of cells.
             */

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
            if (_turnsSurvived >= Utils.NoTurnsUntilWolfBreeds)
            {
                if (TryToBreed()) // implemented in base class CreatureAgent
                    _turnsSurvived = 0;
            }

            // starve
            if (_lastEaten >= Utils.NoTurnsUntilWolfStarves)
                Die();
        }

        private bool TryToEat()
        {
            List<Direction> allowedDirections = new List<Direction>();
            int newLine, newColumn;

            for (int i = 0; i < 4; i++)
            {
                if (_worldEnv.ValidMovement(this, (Direction)i, CellState.Sheep, out newLine, out newColumn))
                    allowedDirections.Add((Direction)i);
            }

            if (allowedDirections.Count == 0)
                return false;

            int r = Utils.Rand.Next(allowedDirections.Count);
            _worldEnv.ValidMovement(this, allowedDirections[r], CellState.Sheep, out newLine, out newColumn);

            _worldEnv.Eat(this, newLine, newColumn);

            return true;
        }

        private void Die()
        {
            // removing the wolf

            if (Utils.Verbose)
                Console.WriteLine("Removing " + this.Name);

            this.Stop();
            _worldEnv.Die(this);
        }
    }
}