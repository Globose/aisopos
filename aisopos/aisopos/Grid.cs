using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace aisopos
{
    internal class Grid
    {
        public int cols { get; set; }
        public int rows { get; set; }
        Point current;
        Point position;
        public int sqSize { get; set; }
        public char[,] data;
        Pen pen;

        public Grid(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
            data = new char[rows,cols];
            current = new Point(0, 0);
            position = new Point(0, 0);
            sqSize = 30;
            pen = new Pen(Color.Red);
        }

        public void Draw(Graphics g)
        {
            for (int i = 0; i < cols; i++)
            {
                g.DrawLine(pen, position.X+sqSize*i, position.Y, position.X+sqSize*i, position.Y+sqSize*rows);
            }
            for (int i = 0; i < rows; i++)
            {
                g.DrawLine(pen, position.X, position.Y+sqSize * i, position.X+sqSize*cols, position.Y+sqSize*i);
            }
        }

        public void down()
        {
            if (current.Y < rows-1)
            {
                current.Y += 1;
            }
        }
        public void up()
        {
            if (current.Y > 0)
            {
                current.Y -= 1;
            }
        }
        public void left()
        {
            if (current.X > 0)
            {
                current.X -= 1;
            }
        }

        public void right()
        {
            if (current.X < cols - 1)
            {
                current.X += 1;
            }
        }
    }
}
