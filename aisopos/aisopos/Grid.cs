using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public Point position { get; set; }
        public float sqSize { get; set; }
        public char[,] data;
        Pen pen;

        public Grid(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
            data = new char[rows,cols];
            current = new Point(0, 0);
            position = new Point(170, 580);
            sqSize = 116f;
            pen = new Pen(Color.Red);
        }

        public void Draw(Graphics g, Point camera, float zoom)
        {
            for (int i = 0; i < cols; i++)
            {
                g.DrawLine(pen, (int)(zoom*(position.X+sqSize*i))+camera.X, (int)(zoom*position.Y)+camera.Y, (int)(zoom*(position.X+sqSize*i)+camera.X), (int)(zoom*(position.Y+sqSize*rows))+ camera.Y);
            }
            for (int i = 0; i < rows; i++)
            {
                g.DrawLine(pen, (int)(zoom*position.X)+camera.X, (int)(zoom*(position.Y+sqSize * i))+camera.Y, (int)(zoom*(position.X+sqSize*cols))+camera.X, (int)(zoom*(position.Y+sqSize*i))+camera.Y);
            }
        }

        public void reStructure(int rowChange, int colChange)
        {
            rows += rowChange;
            cols += colChange;
            char[,] data_2 = new char[rows, cols];
            int preCol = data.GetLength(0);
            int preRow = data.GetLength(1);
            for (int i = 0; i < data_2.Length; i++)
            {
                if (preCol > i/cols && preRow > i%rows)
                    data_2[i/cols,i%rows] = data[i/cols,i%rows];
            }
            data = data_2;
        }

        public void move(int x, int y)
        {
            current.Y += y;
            current.X += x;

            if (current.Y >= rows) current.Y = rows - 1;
            else if (current.Y < 0) current.Y = 0;
            if (current.X >= cols) current.X = cols - 1;
            else if (current.X < 0) current.X = 0;
        }
    }
}
