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
        private int cols, rows;
        private Point current, position, dir;
        private float sqSize;
        private char[,] data;
        private bool[,] closed;
        private Pen pen;
        private SolidBrush solidBlack, solidBlue;
        private StringFormat stringFormat;
        
        public Grid(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
            data = new char[cols, rows];
            closed = new bool[cols, rows];
            current = new Point(0, 0);
            position = new Point(163, 588);
            sqSize = 126f;
            dir = new Point(1,0);

            pen = new Pen(Color.Red);
            solidBlack = new SolidBrush(Color.Black);
            solidBlue = new SolidBrush(Color.FromArgb(37, Color.Blue));
            stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
        }

        public float getSqSize()
        {
            return sqSize;
        }
        public void setSqSize(float size)
        {
            sqSize = size;
        }

        public void setPos(Point p)
        {
            position = p;
        }
        public Point pos()
        {
            return position;
        }

        public int rowCount()
        {
            return rows;
        }

        public int colCount()
        {
            return cols;
        }

        public string getText()
        {
            string a = string.Empty;
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j] == 0) a += ' ';
                    else a += data[i, j];
                }
            }
            return a;
        }

        public string getClosed()
        {
            string a = string.Empty;
            for (int i = 0; i < closed.GetLength(0); i++)
            {
                for (int j = 0; j < closed.GetLength(1); j++)
                {
                    if (closed[i, j]) a += '1';
                    else a += '0';
                }
            }
            return a;
        }

        public void swapDir()
        {
            if (dir.X == 0) dir = new Point(1, 0);
            else dir = new Point(0, 1);
        }

        public void Draw(Graphics g, Point camera, float zoom, bool gMode, bool marker)
        {
            //Drawing Text
            Font font = new Font(FontFamily.GenericMonospace, 35 * zoom);

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    g.DrawString(data[i, j].ToString(), font, solidBlack, 
                        (int)(zoom * (position.X + sqSize * i+sqSize*.5)) + camera.X, 
                        (int)(zoom * (position.Y+sqSize*j+sqSize*0.5)) + camera.Y,stringFormat);
                }
            }

            if (!marker) return;

            //Draw marker
            g.FillEllipse(solidBlue, zoom * (position.X + sqSize * current.X) + camera.X, 
                zoom*(position.Y + sqSize * current.Y) + camera.Y, sqSize*zoom, sqSize*zoom);

            int tx = current.X;
            int ty = current.Y;
            while (true)
            {
                tx += dir.X;
                ty += dir.Y;
                if (tx < 0 || ty < 0 || tx >= data.GetLength(0) ||ty >= data.GetLength(1)) break;
                if (closed[tx, ty]) break;
                g.FillEllipse(solidBlue, (int)(zoom*(position.X+sqSize*tx + sqSize*.33f))+camera.X, 
                    (int)(zoom*(position.Y+sqSize*ty+sqSize*.33))+camera.Y, zoom * sqSize * 0.33f, zoom * sqSize * 0.33f);
            }

            if (!gMode) return;

            //Drawing Grid
            for (int i = 0; i < cols; i++)
            {
                g.DrawLine(pen, (int)(zoom*(position.X+sqSize*i))+camera.X, (int)(zoom*position.Y)+camera.Y, (int)(zoom*(position.X+sqSize*i)+camera.X), (int)(zoom*(position.Y+sqSize*rows))+ camera.Y);
            }
            for (int i = 0; i < rows; i++)
            {
                g.DrawLine(pen, (int)(zoom*position.X)+camera.X, (int)(zoom*(position.Y+sqSize * i))+camera.Y, (int)(zoom*(position.X+sqSize*cols))+camera.X, (int)(zoom*(position.Y+sqSize*i))+camera.Y);
            }

            //Drawing circles
            Pen p = new Pen(Color.Red);
            for (int i = 0; i < closed.GetLength(0); i++)
            {
                for (int j = 0; j < closed.GetLength(1); j++)
                {
                    if (closed[i,j])
                    {
                        g.DrawEllipse(p, (zoom * (position.X + sqSize * i)) + camera.X, 
                            (int)(zoom * (position.Y + sqSize * j)) + camera.Y, (int)sqSize*zoom, (int)sqSize*zoom);
                    }
                }
            }
        }

        public void changeLetter(char letter)
        {
            if (current.X < data.GetLength(0) && current.Y < data.GetLength(1) && current.X >= 0 && current.Y >= 0)
            {
                data[current.X, current.Y] = letter;
            }
            if (letter == ' ') move(-dir.X, -dir.Y);
            else move(dir.X, dir.Y);
        }

        public void setLetter(char letter, int x, int y)
        {
            data[x,y] = letter;
        }

        public void changeOpen(int x, int y)
        {
            int i = (int)((x-position.X) / sqSize);
            int j = (int)((y-position.Y) / sqSize);
            if (i < closed.GetLength(0) && j < closed.GetLength(1) && i >= 0 && j >= 0)
            {
                closed[i, j] = !closed[i, j];
            }
        }

        public void close(int i, int j)
        {
            if (i < closed.GetLength(0) && j < closed.GetLength(1) && i >= 0 && j >= 0)
            {
                closed[i, j] = true;
            }
        }

        public void changeSqSize(int diff)
        {
            sqSize += 0.4f * diff;
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
            }
        }

        public void moveGrid(int x, int y)
        {
            position = new Point(position.X + x, position.Y + y);
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
                for (int j = 0; j < data_2.GetLength(1); j++)
                {
                    if (j >= preRow || i>= preCol) data_2[i, j] = ' ';
                    else data_2[i, j] = data[i, j];
                }
            }
            data = data_2;
            closed = new bool[cols, rows];
        }

        public void moveTo(int x, int y)
        {
            int i = (int)((x - position.X) / sqSize);
            int j = (int)((y - position.Y) / sqSize);
            if (i < data.GetLength(0) && j < data.GetLength(1) && i >= 0 && j >= 0)
            {
                if (!closed[i, j])
                {
                    current = new Point(i, j);
                }
            }
        }

        public void move(int x, int y)
        {
            if (x > 1 || x < -1 || y > 1 || y < -1 || x == y) return;
            int tX = current.X;
            int tY = current.Y;
            while (true)
            {
                tX += x;
                tY += y;

                if (tX < 0 || tX >= data.GetLength(0)) return;
                if (tY < 0 || tY >= data.GetLength(1)) return;

                if (!closed[tX, tY])
                {
                    current = new Point(tX, tY);
                    break;
                }
            }
        }
    }
}
