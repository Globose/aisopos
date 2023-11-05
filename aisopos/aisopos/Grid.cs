using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.Linq;
using System.Security.Cryptography.Xml;
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
        public string[,] data { get; set; }
        public bool[,] closed { get; set; }
        Pen pen;

        string[] dd = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J" };

        public Grid(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
            data = new string[cols, rows];
            closed = new bool[cols, rows];
            current = new Point(0, 0);
            position = new Point(163, 588);
            sqSize = 126f;
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
            Pen p = new Pen(Color.Red);
            SolidBrush b = new SolidBrush(Color.Black);
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    g.DrawString(data[i, j].ToString(), f, b, (zoom * (position.X + sqSize * i)) + camera.X, (int)(zoom * (position.Y+sqSize*j)) + camera.Y);
                }
            }
            for (int i = 0; i < closed.GetLength(0); i++)
            {
                for (int j = 0; j < closed.GetLength(1); j++)
                {
                    if (closed[i,j])
                    {
                        g.DrawEllipse(p, (zoom * (position.X + sqSize * i)) + camera.X, (int)(zoom * (position.Y + sqSize * j)) + camera.Y, (int)sqSize*zoom, (int)sqSize*zoom);
                    }
                }
            }
        }

        public void changeOpen(int x, int y)
        {
            int i = (int)((x-position.X) / sqSize);
            int j = (int)((y-position.Y) / sqSize);
            Debug.WriteLine("CHangeing " + i + "; " + j);
            if (i < closed.GetLength(0) && j < closed.GetLength(1) && i >= 0 && j >= 0)
            {
                closed[i, j] = !closed[i, j];
            }
        }

        public void findOpen(Image img)
        {
            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, 0, 0);
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        int x = (int)(i * sqSize)+position.X;
                        int y = (int)(j * sqSize)+position.Y;
                        float avgAlpha = 0;
                        float[] darkest = new float[] { 255, 255, 255, 255, 255, 255, 255, 255};
                        for (int k = 0; k < 12; k++)
                        {
                            for (int l = 0; l < 12; l++)
                            {
                                int x1 = (int)(x + ((float)sqSize / 4) + ((float)sqSize / 25) * (k+1));
                                int y1 = (int)(y + ((float)sqSize / 4) + ((float)sqSize / 25) * (l+1));

                                int alpha = bitmap.GetPixel(x1, y1).R+ bitmap.GetPixel(x1, y1).G+ bitmap.GetPixel(x1, y1).B;
                                alpha /= 3;
                                for (int m = 0; m < darkest.Length; m++)
                                {
                                    if (darkest[m] > alpha)
                                    {
                                        darkest[m] = alpha;
                                        break;
                                    }
                                }
                                
                                bitmap.SetPixel(x1, y1, Color.Red);
                            }
                        }
                        avgAlpha = darkest.Sum()/darkest.Length;
                        closed[i, j] = avgAlpha < 150;
                    }
                }
                bitmap.Save("temp.png", ImageFormat.Png);
            }
        }

        public void reStructure(int rowChange, int colChange)
        {
            rows += rowChange;
            cols += colChange;
            if (rows < 0) rows = 0;
            if (cols < 0) cols = 0;
            string[,] data_2 = new string[cols, rows];
            int preCol = data.GetLength(0);
            int preRow = data.GetLength(1);
            for (int i = 0; i < data_2.GetLength(0); i++)
            {
                for (int j = 0; j < data_2.GetLength(1); j++)
                {
                    if (j >= preRow || i>= preCol) data_2[i, j] = "";
                    else data_2[i, j] = data[i, j];
                }
            }
            data = data_2;
            closed = new bool[cols, rows];
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
