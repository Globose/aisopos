﻿using System;
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

        char[] dd = new char[] { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J' };

        public Grid(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
            data = new char[cols, rows];
            current = new Point(0, 0);
            position = new Point(170, 580);
            sqSize = 116f;
            pen = new Pen(Color.Red);
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    data[i, j] = dd[j];
                }
            }
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

            Font f = new Font(FontFamily.GenericSerif, 50*zoom);
            SolidBrush b = new SolidBrush(Color.Black);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    g.DrawString(data[i, j].ToString(), f, b, (zoom * (position.X + sqSize * i)) + camera.X, (int)(zoom * (position.Y+sqSize*j)) + camera.Y);
                }
            }
        }

        public void reStructure(int rowChange, int colChange)
        {
            rows += rowChange;
            cols += colChange;
            if (rows < 0) rows = 0;
            if (cols < 0) cols = 0;
            char[,] data_2 = new char[cols, rows];
            int preCol = data.GetLength(0);
            int preRow = data.GetLength(1);
            for (int i = 0; i < data_2.GetLength(0); i++)
            {
                if (i >= preCol) break;
                for (int j = 0; j < data_2.GetLength(1); j++)
                {
                    if (j >= preRow) break;
                    data_2[i, j] = data[i, j];
                }
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
