using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PredatorPrey
{
    public enum CellState { Empty, Sheep, Wolf, Grass };

    public enum Direction { Up, Down, Left, Right };

    public class Cell
    {
        public CellState State;
        public CreatureAgent AgentInCell;

        public Cell()
        {
            State = CellState.Empty;
            AgentInCell = null;
        }
    }

    public class World : ActressMas.TurnBasedEnvironment
    {
        private Cell[,] _map;
        private int _currentId;
        private List<string> _activeWolves, _activeSheep, _activeGrass;
        MainForm mainForm;

        private void FormThread()
        {
            mainForm.ShowDialog();
        }

        public World(int numberOfTurns = 0, int delayAfterTurn = 0, bool randomOrder = true)
            : base(numberOfTurns, delayAfterTurn, randomOrder)
        {
        }

        public void InitWorldMap()
        {
            _map = new Cell[Utils.GridSize, Utils.GridSize];
            for (int i = 0; i < Utils.GridSize; i++)
                for (int j = 0; j < Utils.GridSize; j++)
                    _map[i, j] = new Cell();

            _currentId = 0;

            mainForm = new MainForm(_map);
            Thread t = new Thread(new ThreadStart(FormThread));
            t.Start();
        }

        public void UpdateUI(int turn, int nrSheep, int nrWolves, int nrGrass)
        {
            mainForm.UpdatePlot(turn, nrSheep, nrWolves, nrGrass);
            mainForm.Refresh();
        }

        public void AddAgentToMap(CreatureAgent a, int line, int column)
        {
            if (a.GetType().Name == "SheepAgent")
                _map[line, column].State = CellState.Sheep;
            else if (a.GetType().Name == "WolfAgent")
                _map[line, column].State = CellState.Wolf;
            else if (a.GetType().Name == "GrassAgent")
                _map[line, column].State = CellState.Grass;

            a.Line = line; a.Column = column;
            _map[line, column].AgentInCell = a;
        }

        public void AddAgentToMap(CreatureAgent a, int vectorPosition)
        {
            int line = vectorPosition / Utils.GridSize;
            int column = vectorPosition % Utils.GridSize;

            AddAgentToMap(a, line, column);
        }

        public string CreateName(CreatureAgent a)
        {
            if (a.GetType().Name == "SheepAgent")
                return string.Format("a{0}", _currentId++);
            else if (a.GetType().Name == "WolfAgent")
                return string.Format("d{0}", _currentId++);
            else if (a.GetType().Name == "GrassAgent")
                return string.Format("a{0}", _currentId++);

            throw new Exception("Unknown agent type: " + a.GetType().ToString());
        }

        public void StartNewTurn()
        {
            _activeWolves = new List<string>();
            _activeSheep = new List<string>();
            _activeGrass = new List<string>();

            for (int i = 0; i < Utils.GridSize; i++)
                for (int j = 0; j < Utils.GridSize; j++)
                {
                    if (_map[i, j].State == CellState.Wolf)
                        _activeWolves.Add(_map[i, j].AgentInCell.Name);
                    else if (_map[i, j].State == CellState.Sheep)
                        _activeSheep.Add(_map[i, j].AgentInCell.Name);
                    else if (_map[i, j].State == CellState.Grass)
                        _activeGrass.Add(_map[i, j].AgentInCell.Name);
                }
        }

        public void CountCreatures(out int noWolves, out int noSheep, out int noGrass)
        {
            noSheep = 0;
            noWolves = 0;
            noGrass = 0;

            for (int i = 0; i < Utils.GridSize; i++)
                for (int j = 0; j < Utils.GridSize; j++)
                {
                    if (_map[i, j].State == CellState.Wolf)
                        noWolves++;
                    else if (_map[i, j].State == CellState.Sheep)
                        noSheep++;
                    else if (_map[i, j].State == CellState.Grass)
                        noGrass++;
                }
        }

        public string GetNextCreature()
        {
            if (_activeWolves.Count != 0)
            {
                int r = Utils.Rand.Next(_activeWolves.Count);
                string name = _activeWolves[r];
                _activeWolves.Remove(name);
                return name;
            }
            else if (_activeSheep.Count != 0)
            {
                int r = Utils.Rand.Next(_activeSheep.Count);
                string name = _activeSheep[r];
                _activeSheep.Remove(name);
                return name;
            }
            else if (_activeGrass.Count != 0)
            {
                int r = Utils.Rand.Next(_activeGrass.Count);
                string name = _activeGrass[r];
                _activeGrass.Remove(name);
                return name;
            }
            else
                return "";
        }

        public void Move(CreatureAgent a, int newLine, int newColumn)
        {
            // moving the agent

            _map[newLine, newColumn].State = _map[a.Line, a.Column].State;
            _map[newLine, newColumn].AgentInCell = _map[a.Line, a.Column].AgentInCell;

            _map[a.Line, a.Column].State = CellState.Empty;
            _map[a.Line, a.Column].AgentInCell = null;

            // updating agent position

            a.Line = newLine;
            a.Column = newColumn;
        }

        public void Breed(CreatureAgent a, int newLine, int newColumn)
        {
            CreatureAgent offspring = null;

            if (a.GetType().Name == "SheepAgent")
                offspring = new SheepAgent();
            else if (a.GetType().Name == "WolfAgent")
                offspring = new WolfAgent();
            else if (a.GetType().Name == "GrassAgent")
                offspring = new GrassAgent();

            Add(offspring, CreateName(offspring)); // Add is from ActressMas.TurnBasedEnvironment
            AddAgentToMap(offspring, newLine, newColumn);

            if (a.GetType().Name == "SheepAgent")
                _activeSheep.Add(offspring.Name);
            else if (a.GetType().Name == "WolfAgent")
                _activeWolves.Add(offspring.Name);
            else if (a.GetType().Name == "GrassAgent")
                _activeGrass.Add(offspring.Name);

            if (Utils.Verbose)
                Console.WriteLine("Breeding " + offspring.Name);

            //offspring.Start();
        }

        public void Eat(CreatureAgent creatureAgent, int newLine, int newColumn)
        {
            // removing the sheep
            CreatureAgent ant = null;
            CellState currentState = CellState.Empty;

            if (creatureAgent.GetType().Name == "SheepAgent")
            {
                ant = (GrassAgent)_map[newLine, newColumn].AgentInCell;
                ant.Stop();
                _activeGrass.Remove(ant.Name);

                currentState = CellState.Sheep;
            }
            else if (creatureAgent.GetType().Name == "WolfAgent")
            {
                ant = (SheepAgent)_map[newLine, newColumn].AgentInCell;
                ant.Stop();
                _activeSheep.Remove(ant.Name);

                currentState = CellState.Wolf;
            }
            else if (creatureAgent.GetType().Name == "GrassAgent")
            {
                ant = (WolfAgent)_map[newLine, newColumn].AgentInCell;
                ant.Stop();
                _activeWolves.Remove(ant.Name);

                currentState = CellState.Grass;
            }          


            if (Utils.Verbose)
                Console.WriteLine("Removing " + ant.Name);

            // moving the wolf

            if (Utils.Verbose)
                Console.WriteLine("Moving " + creatureAgent.Name);

            _map[newLine, newColumn].State = currentState;
            _map[newLine, newColumn].AgentInCell = _map[creatureAgent.Line, creatureAgent.Column].AgentInCell;

            _map[creatureAgent.Line, creatureAgent.Column].State = CellState.Empty;
            _map[creatureAgent.Line, creatureAgent.Column].AgentInCell = null;

            // updating wolf position

            creatureAgent.Line = newLine;
            creatureAgent.Column = newColumn;
        }

        public void Die(CreatureAgent creature)
        {
            if(creature.GetType() == typeof(WolfAgent))
            {
                _activeWolves.Remove(creature.Name);
            }

            if (creature.GetType() == typeof(GrassAgent))
            {
                _activeGrass.Remove(creature.Name);
            }

            if (creature.GetType() == typeof(SheepAgent))
            {
                _activeSheep.Remove(creature.Name);
            }
            //this.Remove(wolf); // from ActressMas.Environment

            _map[creature.Line, creature.Column].State = CellState.Empty;
            _map[creature.Line, creature.Column].AgentInCell = null;
        }

        public bool ValidMovement(CreatureAgent a, Direction direction, CellState desiredState, out int newLine, out int newColumn)
        {
            int currentLine = a.Line; int currentColumn = a.Column;
            newLine = currentLine; newColumn = currentColumn;

            switch (direction)
            {
                case Direction.Up:
                    if (currentLine == 0) return false;
                    if (_map[currentLine - 1, currentColumn].State != desiredState) return false;
                    newLine = currentLine - 1;
                    return true;

                case Direction.Down:
                    if (currentLine == Utils.GridSize - 1) return false;
                    if (_map[currentLine + 1, currentColumn].State != desiredState) return false;
                    newLine = currentLine + 1;
                    return true;

                case Direction.Left:
                    if (currentColumn == 0) return false;
                    if (_map[currentLine, currentColumn - 1].State != desiredState) return false;
                    newColumn = currentColumn - 1;
                    return true;

                case Direction.Right:
                    if (currentColumn == Utils.GridSize - 1) return false;
                    if (_map[currentLine, currentColumn + 1].State != desiredState) return false;
                    newColumn = currentColumn + 1;
                    return true;

                default:
                    break;
            }

            throw new Exception("Invalid direction");
        }

        public void WriteToUI(string text)
        {
            mainForm.AddText(text + "\r\n");
        }
    }
}