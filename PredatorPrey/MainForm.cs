using System.Windows.Forms;
using System.Drawing;

namespace PredatorPrey
{
    public partial class MainForm : Form
    {
        private WorldMapUI mapUI;
        private PredPreyPlot plot;
        private TextBox textBox;
        public Cell[,] map = null;

        public MainForm(Cell[,] _map)
        {
            map = _map;
            mapUI = new WorldMapUI(10, 10, 600, 600, map);
            plot = new PredPreyPlot(640, 10, 600, 300);
            textBox = new TextBox();
            textBox.Multiline = true;
            textBox.ScrollBars = ScrollBars.Vertical;
            textBox.ReadOnly = true;
            textBox.BorderStyle = BorderStyle.FixedSingle;
            textBox.BackColor = Color.White;
            textBox.Location = new Point(640, 340);
            textBox.Size = new Size(600, 270);
            this.Controls.Add(mapUI);
            this.Controls.Add(plot);
            this.Controls.Add(textBox);
            Form.CheckForIllegalCrossThreadCalls = false;

            InitializeComponent();
        }

        public void AddText(string text)
        {
            textBox.Text += text;
            textBox.SelectionStart = textBox.Text.Length;
            textBox.ScrollToCaret();
        }

        public void UpdatePlot(int currentTurn, int currentNrSheep, int currentNrWolves)
        {
            plot.UpdatePlot(currentTurn, currentNrSheep, currentNrWolves);
        }

    }
}
