using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PredatorPrey
{
    public partial class PredPreyPlot : UserControl
    {
        List<int> dataSheep = new List<int>();
        List<int> dataWolves = new List<int>();
        List<int> dataGrass = new List<int>();
        int currentTurn = 0;
        const int minPopulationSize = 100;
        int maxY = minPopulationSize; // max population size over each turn
        const int minTurns = 100; // minimum domain on the graph
        int maxX = minTurns;
        Brush plotFill = new SolidBrush(Color.White);
        Pen plotOutline = new Pen(Color.Black, 2);
        Pen lineStyleSheep = new Pen(Color.Blue, 2);
        Pen lineStyleWolves = new Pen(Color.Red, 2);
        Pen lineStyleGrass = new Pen(Color.Green, 2);
        Pen lineStylePlot = new Pen(Color.Black, 2);
        Font notchFont = new Font("Arial", 8);
        Brush textFill = new SolidBrush(Color.Black);
        Rectangle plotArea;
        int maxSheep = 0;
        int maxWolves = 0;
        int maxGrass = 0;

        int LMargin = 0;
        int RMargin = 0;
        int UMargin = 0;
        int DMargin = 0;

        public PredPreyPlot(int _positionX, int _positionY, int _width, int _height)
        {
            this.Location = new Point(_positionX, _positionY);
            this.Width = _width;
            this.Height = _height;
            DoubleBuffered = true;
            plotArea = new Rectangle(LMargin, UMargin,
                Width - LMargin - RMargin, Height - UMargin - DMargin);
            this.Paint += PredPreyPlot_Paint;

            InitializeComponent();
        }
        
        int XDisplay(float x)
        {
            return plotArea.X + (int)(x * plotArea.Width / maxX);
        }

        int YDisplay(float y)
        {
            return plotArea.Y + plotArea.Height - (int)(y * plotArea.Height / maxY);
        }

        void DrawGraph(Graphics g)
        {
            for (int x = 1; x < currentTurn; x++)
            {
                int x1 = XDisplay(x - 1);
                int x2 = XDisplay(x);
                int ySheep1 = YDisplay(dataSheep[x - 1]);
                int ySheep2 = YDisplay(dataSheep[x]);
                int yWolves1 = YDisplay(dataWolves[x - 1]);
                int yWolves2 = YDisplay(dataWolves[x]);
                int yGrass1 = YDisplay(dataGrass[x - 1]);
                int yGrass2 = YDisplay(dataGrass[x]);
                g.DrawLine(lineStyleSheep, x1, ySheep1, x2, ySheep2);
                g.DrawLine(lineStyleWolves, x1, yWolves1, x2, yWolves2);
                g.DrawLine(lineStyleGrass, x1, yGrass1, x2, yGrass2);
            }
        }

        private void PredPreyPlot_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.FillRectangle(plotFill, plotArea);
            g.DrawRectangle(lineStylePlot, plotArea);
            DrawGraph(g);
        }

        public void UpdatePlot(int _currentTurn, int currentNrSheep, int currentNrWolves, int currentNrGrass)
        {
            currentTurn = _currentTurn;
            maxX = (currentTurn > minTurns) ? currentTurn : minTurns;
            dataSheep.Add(currentNrSheep);
            dataWolves.Add(currentNrWolves);
            dataGrass.Add(currentNrGrass);
            maxSheep = currentNrSheep > maxSheep ? currentNrSheep : maxSheep;
            maxWolves = currentNrWolves > maxWolves ? currentNrWolves : maxWolves;
            maxGrass = currentNrGrass > maxGrass ? currentNrGrass : maxGrass;
            maxY = maxSheep > maxWolves ? maxSheep : maxWolves;
            maxY = maxGrass > maxY ? maxGrass : maxY;
            maxY = maxY > minPopulationSize ? maxY : minPopulationSize;
            Refresh();
        }
    }
}
