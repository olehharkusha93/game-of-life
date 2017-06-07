using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        bool[,] universe = new bool[30, 30];
        bool[,] scratchpad = new bool[30, 30];
        int gen = 0;
        int cells = 0;
        int seed = 2001;
        string boundry;
        int cellx = 30;
        int celly = 30;
        int size = 30;
        Timer timer = new Timer();
        public Form1()
        {
            InitializeComponent();

            //Disabled for now
            editToolStripMenuItem.Enabled = false;
            customizeToolStripMenuItem.Enabled = false;
            contentsToolStripMenuItem.Enabled = false;
            indexToolStripMenuItem.Enabled = false;
            searchToolStripMenuItem.Enabled = false;
            //View check mark            
            gridVisibleToolStripMenuItem.CheckOnClick = true;
            gridVisibleToolStripMenuItem.Checked = true;
            headsUpVisibleToolStripMenuItem.Checked = true;
            neighborCountVisibleToolStripMenuItem.Checked = true;
            //Buttons disable
            pauseF8ToolStripMenuItem.Enabled = false;
            pauseButton.Enabled = false;

            toolStripStatusLabel.Text = "Generation: " + gen.ToString();
            toolStripStatusLabelCells.Text = "Cells: " + cells.ToString();
            timer.Interval = 200;
            timer.Tick += Timer_Tick;        
            timer.Enabled = true;
            timer.Stop();     
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
           toolStripStatusLabel.Text = "Generation: " + gen.ToString();
           toolStripStatusLabelCells.Text = "Cells: " + cells.ToString();
           gen++;
           NextGeneration();
           graphicsPanel1.Invalidate();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            float width = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            float height = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);
            //Draw Universe
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    RectangleF rect = RectangleF.Empty;
                    rect.X = j * width;
                    rect.Y = i * height;
                    rect.Width = width;
                    rect.Height = height;

                    if (universe[j, i] == true)
                    {
                        e.Graphics.FillRectangle(Brushes.Gray, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                    else
                    {
                        e.Graphics.DrawRectangle(Pens.Black, rect.X, rect.Y, rect.Width, rect.Height);
                    }              
                }
            }

            Pen pen = new Pen(Color.Gray, 3);
            //Draw columns
            for (int i = 0; i <= universe.GetLength(0); i += 10)
            {
                if (i == 0)
                {
                    e.Graphics.DrawLine(pen, 0, 0, 0, graphicsPanel1.Height);
                }
                else if (i == universe.GetLength(0))
                {
                    e.Graphics.DrawLine(pen, graphicsPanel1.Width - 1, 0, graphicsPanel1.Width - 1, graphicsPanel1.Height - 1);
                }
                else
                {
                    e.Graphics.DrawLine(pen, (graphicsPanel1.Width / (float)universe.GetLength(0)) * i, 0, (graphicsPanel1.Width / (float)universe.GetLength(0)) * i, graphicsPanel1.Height);
                }
                //Draw rows
                for (int k = 0; k <= universe.GetLength(1); k += 10)
                {
                    if (i == 0)
                    {
                        e.Graphics.DrawLine(pen, 0, 0, graphicsPanel1.Height, 0);
                    }
                    else if (i == universe.GetLength(1))
                    {
                        e.Graphics.DrawLine(pen, 0, graphicsPanel1.Height - 1, graphicsPanel1.Width - 1, graphicsPanel1.Height - 1);
                    }
                    else
                    {
                        e.Graphics.DrawLine(pen, 0, graphicsPanel1.Height / (float)universe.GetLength(1) * k, graphicsPanel1.Width, graphicsPanel1.Height / (float)universe.GetLength(1) * k);
                    }
                }
            }
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            float width = (float)graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            float height = (float)graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            if (e.Button == MouseButtons.Left)
            {
                int x = (int)(e.X / width);
                int y = (int)(e.Y / height);

                universe[x, y] = !universe[x, y];
                //if (universe[x, y] == true)
                //{
                //    cells++;
                //}
                //else
                //{
                //    cells--;
                //}
                graphicsPanel1.Invalidate();
            }
        }
        //void CellsCount()
        //{
            
        //}
        void NextGeneration()
        {
            for (int i = 0; i < cellx; i++)
            {
                for (int j = 0; j < celly; j++)
                {
                    scratchpad[i, j] = universe[i, j];
                }
            }
            for (int i = 0; i < cellx; i++)
            {
                for (int j = 0; j < celly; j++)
                {
                    int neighbour = Neighbour(universe, size, i, j, -1, 0) +
                         Neighbour(universe, size, i, j, -1, -1) +
                         Neighbour(universe, size, i, j, 0, -1) +
                         Neighbour(universe, size, i, j, 1, -1) +
                         Neighbour(universe, size, i, j, 1, 0) +
                         Neighbour(universe, size, i, j, 1, 1) +
                         Neighbour(universe, size, i, j, 0, 1) +
                         Neighbour(universe, size, i, j, -1, 1);
                    bool isAlive = universe[i, j];
                    bool stayAlive = false;
                    if(isAlive && neighbour == 2 || neighbour == 3)
                    {
                        stayAlive = true;
                    }
                    else if(!isAlive && neighbour == 3)
                    {
                        stayAlive = true;      
                    }
                    scratchpad[i, j] = stayAlive;                                   
                }
            }
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    universe[i, j] = scratchpad[i, j];
                }
            }
        }
        public int Neighbour(bool[,] universe,int size, int x, int y, int newx, int newy)
        {
            int result = 0;
            int newX = x + newx;
            int newY = y + newy;
            bool outBound = newX < 0 || newX >= size || newY < 0 || newY >= size;
            if(!outBound)
            {
                result = universe[x + newx, y + newy] ? 1 : 0;
            }
            return result;
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < universe.GetLength(0); i++)
            {
                for (int j = 0; j < universe.GetLength(1); j++)
                {
                    universe[j, i] = false;
                }
            }
            gen = 0;
            toolStripStatusLabel.Text = "Generation: " + gen.ToString();
            //toolStripStatusLabel.Text = "Generation: " + gen.ToString();
            timer.Stop();
            startF7ToolStripMenuItem.Enabled = true;
            newButton.Enabled = true;
            pauseF8ToolStripMenuItem.Enabled = false;
            pauseButton.Enabled = false;
            nextToolStripMenuItem.Enabled = true;
            nextButton.Enabled = true;
            graphicsPanel1.Invalidate();
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open = new OpenFileDialog();
            open.InitialDirectory = Application.StartupPath;
            open.Filter = "Cells|*.cells";
            if(open.ShowDialog() == DialogResult.OK)
            {
                //Load in the file
            }
            graphicsPanel1.Invalidate();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog save = new SaveFileDialog();
            save.InitialDirectory = Application.StartupPath;
            save.Filter = "Cells|*.cells";
            if(save.ShowDialog() == DialogResult.OK)
            {
                //Save the file
            }
        }
        private void startF7ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Start();
            startF7ToolStripMenuItem.Enabled = false;
            newButton.Enabled = false;
            pauseF8ToolStripMenuItem.Enabled = true;
            pauseButton.Enabled = true;
            nextToolStripMenuItem.Enabled = false;
            nextButton.Enabled = false;
        }
        private void pauseF8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer.Stop();

            startF7ToolStripMenuItem.Enabled = true;
            newButton.Enabled = true;
            pauseF8ToolStripMenuItem.Enabled = false;
            pauseButton.Enabled = false;
            nextToolStripMenuItem.Enabled = true;
            nextButton.Enabled = true;
        }
        private void nextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NextGeneration();
            gen++;
            toolStripStatusLabel.Text = "Generation: " + gen.ToString();
            graphicsPanel1.Invalidate();
        }
        private void gridVisibleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gridVisibleToolStripMenuItem.Checked == false)
            {

            }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 dlg = new AboutBox1();
            dlg.ShowDialog();
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}

