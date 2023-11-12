using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Security.Policy;
using System.Text;

namespace aisopos
{
    public partial class Aisopos : Form
    {
        string data_path = "C:/data/cross/";
        Image? img;
        Point camera;
        Rectangle sourceRect, destRect;
        bool ctrlDown, shiftDown;
        int mode;
        float zoom;
        string? imgUrl;
        Grid? grid;
        Random rand;

        public Aisopos()
        {
            InitializeComponent();
            ctrlDown = false;
            shiftDown = false;
            mode = 0;
            zoom = 1;
            rand= new Random();
            LoadData();
        }

        private void Aisopos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) ctrlDown = true;
            else if (e.KeyCode == Keys.ShiftKey) shiftDown = true;
            else if (ctrlDown)
            {
                if (e.KeyCode == Keys.V) PasteImage();
                else if (e.KeyCode == Keys.Up) camera.Y += 66;
                else if (e.KeyCode == Keys.Down) camera.Y -= 66;
                else if (e.KeyCode == Keys.Left) camera.X += 66;
                else if (e.KeyCode == Keys.Right) camera.X -= 66;
                else if (e.KeyCode == Keys.S) SaveData();
                else if (e.KeyCode == Keys.N) mode = 0;
                else if (e.KeyCode == Keys.G) mode = 1;
                else if (e.KeyCode == Keys.M) mode = 2;
            }
            else if (img is null || grid is null) return;
            else if (e.KeyCode == Keys.Oemplus) zoom *= 1.03f;
            else if (e.KeyCode == Keys.OemMinus) zoom *= 0.97f;
            else if (e.KeyCode == Keys.Up) grid.MoveSelectedInDir(0, -1, false, mode);
            else if (e.KeyCode == Keys.Down) grid.MoveSelectedInDir(0, 1, false, mode);
            else if (e.KeyCode == Keys.Left) grid.MoveSelectedInDir(-1, 0, false, mode);
            else if (e.KeyCode == Keys.Right) grid.MoveSelectedInDir(1, 0, false, mode);
            else if (mode == 1)
            {
                if (e.KeyCode == Keys.E) grid.RebuildGrid(0, 1);
                else if (e.KeyCode == Keys.Q) grid.RebuildGrid(0, -1);
                else if (e.KeyCode == Keys.Z) grid.RebuildGrid(-1, 0);
                else if (e.KeyCode == Keys.C) grid.RebuildGrid(1, 0);
                else if (e.KeyCode == Keys.A) grid.MoveGrid(-1, 0, shiftDown);
                else if (e.KeyCode == Keys.D) grid.MoveGrid(1, 0, shiftDown);
                else if (e.KeyCode == Keys.W) grid.MoveGrid(0, -1, shiftDown);
                else if (e.KeyCode == Keys.S) grid.MoveGrid(0, 1, shiftDown);
                else if (e.KeyCode == Keys.D2 && shiftDown) grid.squareSize += 2f;
                else if (e.KeyCode == Keys.D1 && shiftDown) grid.squareSize -= 2f;
                else if (e.KeyCode == Keys.D2) grid.squareSize += 0.3f;
                else if (e.KeyCode == Keys.D1) grid.squareSize -= 0.3f;
                else if (e.KeyCode == Keys.Enter) grid.AnalyseGrid(img);
                else if (e.KeyCode == Keys.Space) grid.ChangeOpenSelected();
            }
            else if (mode == 0)
            {
                if (e.KeyCode == Keys.Oem6) grid.ChangeSelectedLetter('Å');
                else if (e.KeyCode == Keys.Space) grid.ChangeDirection();
                else if (e.KeyCode == Keys.Oem7) grid.ChangeSelectedLetter('Ä');
                else if (e.KeyCode == Keys.Oemtilde) grid.ChangeSelectedLetter('Ö');
                else if (e.KeyData.ToString().Length == 1)
                {
                    char c = e.KeyData.ToString()[0];
                    if (c < 91 && c > 64)
                    {
                        grid.ChangeSelectedLetter(c);
                    }
                }
                else if (e.KeyCode == Keys.Back) grid.ChangeSelectedLetter(' ');
            }
            else if (mode == 2 && e.KeyCode == Keys.Enter) grid.ChangeSelectedHint();
            Invalidate();
        }

        private void PasteImage()
        {
            if (!Clipboard.ContainsImage())
            {
                MessageBox.Show("No Image in clipboard");
                return;
            }
            img = Clipboard.GetImage();
            imgUrl = "u" + rand.Next(1000000) + ".jpeg";
            img.Save(data_path + imgUrl, ImageFormat.Jpeg);
            sourceRect = new Rectangle(0, 0, img.Width, img.Height);
            destRect = new Rectangle(0, 0, img.Width, img.Height);
            grid = new Grid(23, 30);

            List<string> allLines = new List<string>();
            try
            {
                string[] t = File.ReadAllLines(data_path + "data.txt");
                foreach (string t1 in t)
                {
                    allLines.Add(t1);
                }
            }
            catch { }

            allLines.Add(GetSaveString());

            try { File.WriteAllLines(data_path + "data.txt", allLines.ToArray()); }
            catch { }
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (img is null || grid is null) return;
            destRect = new Rectangle(camera.X, camera.Y, (int)(img.Width * zoom), (int)(img.Height * zoom));
            e.Graphics.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel);
            grid.Draw(e.Graphics, camera, zoom, mode, true);
            
        }
        private void Aisopos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey) ctrlDown = false;
            else if (e.KeyCode == Keys.ShiftKey) shiftDown = false;
        }

        private void LoadData()
        {
            string[] rows;
            try { rows = File.ReadAllLines(data_path + "data.txt");}
            catch {rows = new string[0];}
            if (rows.Length == 0) return;

            string[]? data = null;
            for (int i = 1; i <= rows.Length; i++)
            {
                data = rows[rows.Length - i].Split(';');
                if (data.Length != 8) return;
                try
                {
                    imgUrl = data[0];
                    img = Image.FromFile(data_path + data[0]);
                    break;
                }
                catch {
                    string[] rows2 = new string[rows.Length-1];
                    for (int j = 0; j < rows2.Length; j++)
                    {
                        rows2[j] = rows[j];
                    }
                    File.WriteAllLines(data_path + "data.txt", rows2);
                };
            }
            if (data is null || img is null) return;
            int rowCount = 1;
            int colCount = 1;
            int gridPosX = 0;
            int gridPosY = 0;
            float squareSize = 126;
            string gridData = data[7];
                
            int.TryParse(data[1], out rowCount);
            int.TryParse(data[2], out colCount);
            int.TryParse(data[3], out gridPosX);
            int.TryParse(data[4], out gridPosY);
            float.TryParse(data[5], out squareSize);
            float.TryParse(data[6], out zoom);

            if (gridData.Length == colCount * rowCount)
            {
                grid = new Grid(colCount, rowCount, new Point(gridPosX, gridPosY), squareSize, gridData);
            }

            sourceRect = new Rectangle(0, 0, img.Width, img.Height);
            destRect = new Rectangle(0, 0, img.Width, img.Height);
        }

        private void SaveData()
        {
            if (img is null || imgUrl is null || grid is null) return;
            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, 0, 0);
                grid.Draw(g, new Point(0, 0), 1, mode, false);
            }

            string[] allLines;
            try {allLines = File.ReadAllLines(data_path + "data.txt");}
            catch {allLines = new string[1];}
            try
            {
                allLines[allLines.Length - 1] = GetSaveString();
                File.WriteAllLines(data_path + "data.txt", allLines);
            }
            catch { }
            bitmap.Save(data_path + "solved_" +imgUrl, ImageFormat.Jpeg);
        }

        private string GetSaveString()
        {
            if (grid is null) return string.Empty;
            return imgUrl + ";"+grid.rows + ";" + grid.cols + ";" + grid.Position.X + 
                ";" + grid.Position.Y + ";" + grid.squareSize + ";" + zoom + ";" + grid.ToString();
        }

        private void Aisopos_FormClosed(object sender, FormClosedEventArgs e)
        {
            SaveData();
        }

        private void Aisopos_MouseDown(object sender, MouseEventArgs e)
        {
            if (grid is null) return;
            int x = (int)((e.X - camera.X) / zoom);
            int y = (int)((e.Y - camera.Y) / zoom);
            if (mode == 1) grid.ChangeOpenFromCoords(x, y);
            else grid.MoveSelectedToCoords(x, y);
            Invalidate();
        }
    }
}