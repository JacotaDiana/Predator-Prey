using ActressMas;
using System;
using System.Collections.Generic;

namespace PredatorPrey
{
    public class SheepAgent : CreatureAgent
    {
        private int _lastEaten;

        public override void Setup()
        {
            _turnsSurvived = 0;
            _lastEaten = 0;

            _worldEnv = (World)this.Environment;

            if (Utils.Verbose)
                Console.WriteLine("AntAgent {0} started in ({1},{2})", this.Name, Line, Column);
        }

        public override void Act(Queue<Message> messages)
        {
            if (messages.Count > 0)
            {
                Message message = messages.Dequeue();
                if (Utils.Verbose)
                    Console.WriteLine("\t[{1} -> {0}]: {2}", this.Name, message.Sender, message.Content);

                SheepAction();
                Send("scheduler", "done");
            }
        }

        private void SheepAction()
        {
            /*
              • Move: For every time step, the sheep randomly try to move up, down, left, or right. If the neighboring
                cell in the selected direction is occupied or would move the sheep off the grid, then the sheep stays in the
                current cell.
                • Breed: If a sheep survives for NoTurnsUntilSheepBreeds time steps, at the end of the time step (i.e., after moving) the sheep will
                breed. This is simulated by creating a new sheep in an adjacent (up, down, left, or right) cell that is
                empty. If there is no empty cell available, no breeding occurs. Once an offspring is produced, an sheep
                cannot produce an offspring again until it has survived for NoTurnsUntilSheepBreeds more time steps.
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
            if (_turnsSurvived >= Utils.NoTurnsUntilSheepBreeds)
            {
                if (TryToBreed()) // implemented in base class CreatureAgent
                    _turnsSurvived = 0;
            }

            // starve
            if (_lastEaten >= Utils.NoTurnsUntilSheepStarves)
                Die();
        }

        public bool TryToEat()
        {
            List<Direction> allowedDirections = new List<Direction>();
            int newLine, newColumn;

            for (int i = 0; i < 4; i++)
            {
                if (_worldEnv.ValidMovement(this, (Direction)i, CellState.Grass, out newLine, out newColumn))
                    allowedDirections.Add((Direction)i);
            } 

            if (allowedDirections.Count == 0)
                return false;

            int r = Utils.Rand.Next(allowedDirections.Count);
            _worldEnv.ValidMovement(this, allowedDirections[r], CellState.Grass, out newLine, out newColumn);

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