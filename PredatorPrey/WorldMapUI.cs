using System.Drawing;
using System.Windows.Forms;

namespace PredatorPrey
{
    public partial class WorldMapUI : UserControl
    {
        Image wolf = new Bitmap("wolf_red.png");
        Image sheep = new Bitmap("sheep_blue.png");
        Image grass = new Bitmap("grass.png");
        int gridSizeX, gridSizeY;
        RectangleF[,] cellBoundingBoxes;
        Cell[,] map = null;
        Pen lineStyle = new Pen(Color.Black, 2);
        
        public WorldMapUI(int _positionX, int _positionY, int _width, int _height, Cell[,] _map)
        {
            DoubleBuffered = true;
            map = _map;
            Location = new Point(_positionX, _positionY);
            Width = _width;
            Height = _height;
            this.Paint += WorldMap_Paint;
            this.BackgroundImage = new Bitmap("ground_tex.jpg");
            gridSizeX = Utils.GridSize;
            gridSizeY = Utils.GridSize;
            cellBoundingBoxes = new RectangleF[gridSizeX, gridSizeY];

            float cellWidth = (float)Width / gridSizeX;
            float cellHeight = (float)Height / gridSizeY;

            for (int y = 0; y < gridSizeY; y++)
                for (int x = 0; x < gridSizeX; x++)
                    cellBoundingBoxes[x, y] = new RectangleF(x * cellWidth, y * cellHeight, cellWidth, cellHeight);

            InitializeComponent();
        }
        
        void DrawSheepAt(Graphics g, int gridX, int gridY)
        {
            g.DrawImage(sheep, cellBoundingBoxes[gridX, gridY]);
        }

        void DrawWolfAt(Graphics g, int gridX, int gridY)
        {
            g.DrawImage(wolf, cellBoundingBoxes[gridX, gridY]);
        }

        void DrawGrassAt(Graphics g, int gridX, int gridY)
        {
            g.DrawImage(grass, cellBoundingBoxes[gridX, gridY]);
        }

        private void WorldMap_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            
            if (map != null)
            {
                for(int y = 0; y < gridSizeY; y++)
                    for(int x = 0; x < gridSizeX; x++)
                    {
                        switch(map[x, y].State)
                        {
                            case CellState.Sheep:
                                DrawSheepAt(g, x, y);
                                break;

                            case CellState.Wolf:
                                DrawWolfAt(g, x, y);
                                break;

                            case CellState.Grass:
                                DrawGrassAt(g, x, y);
                                break;
                        }
                    }
            }

            g.DrawRectangle(lineStyle, ClientRectangle);
        }
    }
}
