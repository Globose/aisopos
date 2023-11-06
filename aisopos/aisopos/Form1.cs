using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Policy;
using System.Text;

namespace aisopos
{
    public partial class Aisopos : Form
    {
        Image? img;
        Point camera;
        float zoom;
        Rectangle sourceRect, destRect;
        bool ctrlDown, gMode;
        Grid grid;
        string? imgUrl;
        Random rand;

        public Aisopos()
        {
            InitializeComponent();
            ctrlDown = false;
            gMode = false;
            zoom = 1f;
            grid = new Grid(10, 10);
            rand= new Random();
            LoadImg();
        }

        private void Aisopos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.V && ctrlDown)
            {
                if (Clipboard.ContainsImage())
                {
                    img = Clipboard.GetImage();
                    sourceRect = new Rectangle(0, 0, img.Width, img.Height);
                    destRect = new Rectangle(0, 0, img.Width, img.Height);
                    imgUrl = "u"+rand.Next(1000000)+".jpeg";
                    img.Save(imgUrl, ImageFormat.Jpeg);
                    grid = new Grid(10, 10);
                    List<string> allLines = new List<string>();
                    allLines.Add(toSaveString());
                    try
                    {
                        string[] t = File.ReadAllLines("data.txt");
                        foreach (string t1 in t)
                        {
                            allLines.Add(t1);
                        }
                    }
                    catch (Exception e1)
                    {
                        Debug.WriteLine(e1.Message);
                    }

                    try
                    {
                        File.WriteAllLines("data.txt", allLines.ToArray());
                    }
                    catch (Exception e1) 
                    {
                        Debug.WriteLine(e1.Message);
                    }

                    Invalidate();
                }
                else
                {
                    MessageBox.Show("No image");
                }
            }
            else if (e.KeyCode == Keys.ControlKey) ctrlDown = true;
            else if (e.KeyCode == Keys.Up && ctrlDown) camera.Y += 66;
            else if (e.KeyCode == Keys.Down && ctrlDown) camera.Y -= 66;
            else if (e.KeyCode == Keys.Left && ctrlDown) camera.X += 66;
            else if (e.KeyCode == Keys.Right && ctrlDown) camera.X -= 66;
            else if (e.KeyCode == Keys.Oemplus) zoom *= 1.03f;
            else if (e.KeyCode == Keys.OemMinus) zoom *= 0.97f;
            else if (e.KeyCode == Keys.E && gMode) grid.reStructure(0, 1);
            else if (e.KeyCode == Keys.Q && gMode) grid.reStructure(0, -1);
            else if (e.KeyCode == Keys.Z && gMode) grid.reStructure(-1, 0);
            else if (e.KeyCode == Keys.C && gMode) grid.reStructure(1, 0);
            else if (e.KeyCode == Keys.A && gMode) grid.moveGrid(-1, 0);
            else if (e.KeyCode == Keys.D && gMode) grid.moveGrid(1, 0);
            else if (e.KeyCode == Keys.W && gMode) grid.moveGrid(0, -1);
            else if (e.KeyCode == Keys.S && gMode && !ctrlDown) grid.moveGrid(0, 1);
            else if (e.KeyCode == Keys.D2) grid.changeSqSize(1);
            else if (e.KeyCode == Keys.D1) grid.changeSqSize(-1);
            else if (e.KeyCode == Keys.S && ctrlDown) saveImage();
            else if (e.KeyCode == Keys.G && ctrlDown) gMode = !gMode;
            else if (e.KeyCode == Keys.Enter && gMode) createGrid();
            else if (e.KeyCode == Keys.Up) grid.move(0, -1);
            else if (e.KeyCode == Keys.Down) grid.move(0, 1);
            else if (e.KeyCode == Keys.Left) grid.move(-1, 0);
            else if (e.KeyCode == Keys.Right) grid.move(1, 0);
            else if (e.KeyCode == Keys.Space) grid.swapDir();
            else if (e.KeyCode == Keys.Oem6 && !gMode) grid.changeLetter('Å');
            else if (e.KeyCode == Keys.Oem7 && !gMode) grid.changeLetter('Ä');
            else if (e.KeyCode == Keys.Oemtilde && !gMode) grid.changeLetter('Ö');
            else if (e.KeyData.ToString().Length == 1 && !gMode)
            {
                char c = e.KeyData.ToString()[0];
                if (c < 91 && c > 64)
                {
                    grid.changeLetter(c);
                }
            }
            else if (e.KeyData == Keys.Back && !gMode) grid.changeLetter(' ');
            

            Invalidate();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (img is null) return;
            destRect = new Rectangle(camera.X, camera.Y, (int)(img.Width * zoom), (int)(img.Height * zoom));
            e.Graphics.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel);
            grid.Draw(e.Graphics, camera, zoom, gMode, true);
            
        }
        private void Aisopos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                ctrlDown = false;
            }
        }

        private void createGrid()
        {
            if (img is null) return;
            grid.findOpen(img);
        }

        private void LoadImg()
        {
            string[] rows;
            try
            {
                rows = File.ReadAllLines("data.txt");
            }
            catch
            {
                rows = new string[0];
            }
            

            if (rows.Length > 0) 
            {
                string row = rows[rows.Length-1];
                string[] parts = row.Split(';');
                int rowCount = 1;
                int colCount = 1;
                string gridDataText = string.Empty;
                string gridDataClosed = string.Empty;
                int gridPosX = 0;
                int gridPosY = 0;
                float sqSize = 126f;
                foreach (string part in parts)
                {
                    string[] action = part.Split('=');
                    if (action.Length != 2) continue;
                    switch (action[0])
                    {
                        case "imgUrl":
                            try
                            {
                                imgUrl = action[1];
                                img = Image.FromFile(action[1]);
                            }
                            catch 
                            {
                                Debug.WriteLine("Failed to read Image=" + action[1]);
                            };
                            break;
                        case "rowCount":
                            int.TryParse(action[1], out rowCount);
                            break;
                        case "colCount":
                            int.TryParse(action[1],out colCount);
                            break;
                        case "dataText":
                            gridDataText = action[1];
                            break;
                        case "dataClosed":
                            gridDataClosed = action[1];
                            break;
                        case "zoom":
                            float.TryParse(action[1], out zoom);
                            break;
                        case "gridPosX":
                            int.TryParse(action[1], out gridPosX);
                            break;
                        case "gridPosY":
                            int.TryParse(action[1], out gridPosY);
                            break;
                        case "sqSize":
                            float.TryParse(action[1], out sqSize);
                            break;
                    }
                }


                if (gridDataText.Length == gridDataClosed.Length && gridDataClosed.Length == colCount * rowCount)
                {
                    grid = new Grid(colCount, rowCount);
                    grid.setPos(new Point(gridPosX, gridPosY));
                    grid.setSqSize(sqSize);
                    int c = 0;
                    for (int i = 0; i < colCount; i++)
                    {
                        for (int j = 0; j < rowCount; j++)
                        {
                            if (gridDataClosed[c] == '1') grid.close(i, j);
                            grid.setLetter(gridDataText[c], i, j);
                            c++;
                        }
                    }
                }

                if (img is null) return;
                sourceRect = new Rectangle(0, 0, img.Width, img.Height);
                destRect = new Rectangle(0, 0, img.Width, img.Height);

                return;
            }
        }

        private void saveImage()
        {
            if (img is null || imgUrl is null) return;

            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, 0, 0);
                grid.Draw(g, new Point(0, 0), 1, false, false);
            }

            string[] allLines;
            try
            {
                allLines = File.ReadAllLines("data.txt");
            }
            catch (Exception e)
            {
                allLines = new string[1];
                Debug.WriteLine(e.Message);
            }
            try
            {
                allLines[0] = toSaveString();
                File.WriteAllLines("data.txt", allLines);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            bitmap.Save("solved.jpeg", ImageFormat.Jpeg);
        }

        private string toSaveString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("zoom=" + zoom + ";");
            sb.Append("imgUrl=" + imgUrl + ";");
            sb.Append("rowCount=" + grid.rowCount() + ";");
            sb.Append("colCount=" + grid.colCount() + ";");
            sb.Append("dataText=" + grid.getText() + ";");
            sb.Append("dataClosed=" + grid.getClosed() + ";");
            sb.Append("gridPosX=" + grid.pos().X + ";");
            sb.Append("gridPosY=" + grid.pos().Y +";") ;
            sb.Append("sqSize="+grid.getSqSize());
            return sb.ToString();
        }

        private void Aisopos_MouseDown(object sender, MouseEventArgs e)
        {
            int x = (int)((e.X - camera.X) / zoom);
            int y = (int)((e.Y - camera.Y) / zoom);
            if (gMode) grid.changeOpen(x, y);
            else grid.moveTo(x, y);
            Invalidate();
        }
    }
}