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
        public int cols { get; private set; }
        public int rows { get; private set; }
        private Point selected, dir;
        public Point Position { get; set; }
        public float squareSize { get; set; }
        private char[,] data;
        private Pen pen;
        private SolidBrush solidBlack, solidBlue, solidRed;
        private StringFormat stringFormat;
        
        #pragma warning disable CS8618
        public Grid(int cols, int rows)
        {
            this.cols = cols;
            this.rows = rows;
            Position = new Point(163, 588);
            squareSize = 126f;
            data = new char[cols, rows];
            init();
        }

        public Grid(int cols, int rows, Point Position, float squareSize, string data)
        {
            this.cols = cols;
            this.rows = rows;
            this.Position = Position;
            this.squareSize = squareSize;
            init();
            this.data = new char[cols, rows];

            int counter = 0;
            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    this.data[i, j] = data[counter];
                    counter++;
                }
            }
        }

        private void init()
        {
            dir = new Point(1,0);
            selected = new Point(0, 0);
            pen = new Pen(Color.Red);
            solidBlack = new SolidBrush(Color.Black);
            solidBlue = new SolidBrush(Color.FromArgb(33, Color.Blue));
            solidRed = new SolidBrush(Color.FromArgb(45,Color.Red));
            stringFormat = new StringFormat();
            stringFormat.LineAlignment = StringAlignment.Center;
            stringFormat.Alignment = StringAlignment.Center;
        }

        public void MoveGrid(int dX, int dY, bool shiftDown)
        {
            if (shiftDown) Position = new Point(Position.X + dX*10, Position.Y + dY*10);
            else Position = new Point(Position.X + dX, Position.Y + dY);
        }

        public override string ToString()
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

        public void ChangeDirection()
        {
            if (dir.X == 0) dir = new Point(1, 0);
            else dir = new Point(0, 1);
        }

        public void Draw(Graphics g, Point camera, float zoom, int mode, bool marker)
        {
            //Drawing Text
            Font font = new Font(FontFamily.GenericMonospace, (squareSize*0.26f) * zoom);

            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j] == '0' || data[i, j] == '1') continue;
                    g.DrawString(data[i, j].ToString(), font, solidBlack, 
                        (int)(zoom * (Position.X + squareSize * i+squareSize*.5)) + camera.X, 
                        (int)(zoom * (Position.Y+squareSize*j+squareSize*0.5)) + camera.Y,stringFormat);
                }
            }

            //Drawing Hints
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i, j] == '1')
                    {
                        g.FillEllipse(solidRed, (zoom * (Position.X + squareSize * i)) + camera.X,
                            (int)(zoom * (Position.Y + squareSize * j)) + camera.Y, (int)squareSize * zoom, (int)squareSize * zoom);
                    }
                }
            }

            if (!marker) return;

            //Draw marker
            g.FillEllipse(solidBlue, zoom * (Position.X + squareSize * selected.X) + camera.X, 
                zoom*(Position.Y + squareSize * selected.Y) + camera.Y, squareSize*zoom, squareSize*zoom);

            int tx = selected.X;
            int ty = selected.Y;
            while (true && mode != 2)
            {
                tx += dir.X;
                ty += dir.Y;
                if (tx < 0 || ty < 0 || tx >= data.GetLength(0) ||ty >= data.GetLength(1) || data[tx, ty] == '0' || data[tx,ty] == '1') break;
                g.FillEllipse(solidBlue, (int)(zoom*(Position.X+squareSize*tx + squareSize*.33f))+camera.X, 
                    (int)(zoom*(Position.Y+squareSize*ty+squareSize*.33))+camera.Y, zoom * squareSize * 0.33f, zoom * squareSize * 0.33f);
            }

            if (mode != 1) return;

            //Drawing Grid
            for (int i = 0; i < cols; i++)
            {
                g.DrawLine(pen, (int)(zoom*(Position.X+squareSize*i))+camera.X, (int)(zoom*Position.Y)+camera.Y, (int)(zoom*(Position.X+squareSize*i)+camera.X), (int)(zoom*(Position.Y+squareSize*rows))+ camera.Y);
            }
            for (int i = 0; i < rows; i++)
            {
                g.DrawLine(pen, (int)(zoom*Position.X)+camera.X, (int)(zoom*(Position.Y+squareSize * i))+camera.Y, (int)(zoom*(Position.X+squareSize*cols))+camera.X, (int)(zoom*(Position.Y+squareSize*i))+camera.Y);
            }

            //Drawing circles
            for (int i = 0; i < data.GetLength(0); i++)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                {
                    if (data[i,j] == '0' || data[i,j] == '1')
                    {
                        g.DrawEllipse(pen, (zoom * (Position.X + squareSize * i)) + camera.X, 
                            (int)(zoom * (Position.Y + squareSize * j)) + camera.Y, (int)squareSize*zoom, (int)squareSize*zoom);
                    }
                }
            }
        }

        public void ChangeSelectedLetter(char letter)
        {
            if (selected.X < data.GetLength(0) && selected.Y < data.GetLength(1) && selected.X >= 0 && selected.Y >= 0)
            {
                data[selected.X, selected.Y] = letter;
            }
            if (letter == ' ') MoveSelectedInDir(-dir.X, -dir.Y, true, 0);
            else MoveSelectedInDir(dir.X, dir.Y, true, 0);
        }

        public void ChangeLetterAtGridPosition(char letter, int x, int y)
        {
            data[x,y] = letter;
        }

        public void ChangeOpenFromCoords(int x, int y)
        {
            int i = (int)((x-Position.X) / squareSize);
            int j = (int)((y-Position.Y) / squareSize);

            if (i < data.GetLength(0) && j < data.GetLength(1) && i >= 0 && j >= 0)
            {
                if (data[i, j] == '0' || data[i, j] == '1' ) data[i, j] = ' ';
                else data[i, j] = '0';
            }
        }

        public void ChangeOpenSelected()
        {
            if (data[selected.X, selected.Y] == '0' || 
                data[selected.X, selected.Y] == '1') data[selected.X, selected.Y] = ' ';
            else data[selected.X, selected.Y] = '0';
        }

        public void ChangeSelectedHint()
        {
            if (data[selected.X, selected.Y] == '0') data[selected.X, selected.Y] = '1';
            else if (data[selected.X, selected.Y] == '1') data[selected.X, selected.Y] = '0';
        }

        public void AnalyseGrid(Image img)
        {
            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, 0, 0);
                for (int i = 0; i < data.GetLength(0); i++)
                {
                    for (int j = 0; j < data.GetLength(1); j++)
                    {
                        int x = (int)(i * squareSize)+Position.X;
                        int y = (int)(j * squareSize)+Position.Y;
                        float avgAlpha = 0;
                        float[] darkest = new float[] { 255, 255, 255, 255, 255, 255, 255, 255};

                        for (int k = 0; k < 12; k++)
                        {
                            for (int l = 0; l < 12; l++)
                            {
                                int x1 = (int)(x + ((float)squareSize / 4) + ((float)squareSize / 25) * (k+1));
                                int y1 = (int)(y + ((float)squareSize / 4) + ((float)squareSize / 25) * (l+1));
                                if (x1 < 0 || bitmap.Width <= x1 || y1 < 0 || bitmap.Height <= y1) continue;

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
                        if (avgAlpha < 150)
                        {
                            data[i, j] = '0';
                        }
                    }
                }
            }
        }

        public void RebuildGrid(int rowChange, int colChange)
        {
            rows += rowChange;
            cols += colChange;
            if (rows < 0) rows = 0;
            if (cols < 0) cols = 0;

            char[,] data_2 = new char[cols, rows];
            int preCol = data.GetLength(0);
            int preRow = data.GetLength(1);

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    if (j >= preRow || i>= preCol) data_2[i, j] = ' ';
                    else data_2[i, j] = data[i, j];
                }
            }
            data = data_2;
        }

        public void MoveSelectedToCoords(int x, int y)
        {
            int i = (int)((x - Position.X) / squareSize);
            int j = (int)((y - Position.Y) / squareSize);
            if (i < data.GetLength(0) && j < data.GetLength(1) && i >= 0 && j >= 0)
            {
                if (data[i, j] != '0' && data[i,j] != '1') selected = new Point(i, j);
            }
        }

        public void MoveSelectedInDir(int i, int j, bool autoMove, int mode)
        {
            if (i > 1 || i < -1 || j > 1 || j < -1 || i == j) return;
            int tX = selected.X;
            int tY = selected.Y;
            while (true)
            {
                tX += i;
                tY += j;

                if (tX < 0)
                {
                    if (mode == 2)
                    {
                        tX = cols;
                        tY -= 1;
                        continue;
                    }
                    else return;
                }
                else if (tX >= data.GetLength(0))
                {
                    if (mode == 2)
                    {
                        tX = -1;
                        tY += 1;
                        continue;
                    }
                    else return; 
                }
                else if (tY < 0 || tY >= data.GetLength(1)) return;

                if ((data[tX, tY] != '0' && mode != 2) || ((data[tX,tY] == '0' || data[tX,tY] == '1') && mode == 2) || mode == 1)
                {
                    selected = new Point(tX, tY);
                    break;
                }
                if (autoMove) break;
            }
        }
    }
}
