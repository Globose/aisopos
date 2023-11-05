using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace aisopos
{
    public partial class Aisopos : Form
    {
        Image img;
        Point camera;
        float zoom;
        Rectangle sourceRect, destRect;
        bool ctrlDown, gMode;
        Grid grid;

        public Aisopos()
        {
            InitializeComponent();
            img = Image.FromFile("img.jpeg");
            zoom = 1f;
            ctrlDown = false;
            grid = new Grid(10, 10);
            sourceRect = new Rectangle(0, 0, img.Width, img.Height);
            destRect = new Rectangle(0, 0, img.Width, img.Height);
            gMode = true;
        }

        public string GridInfo()
        {
            StringBuilder sb = new StringBuilder();
            

            return "";
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
            else if (e.KeyCode == Keys.S && gMode) grid.moveGrid(0, 1);
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
            else if (e.KeyCode == Keys.Oem6) grid.changeLetter("Å");
            else if (e.KeyCode == Keys.Oem7) grid.changeLetter("Ä");
            else if (e.KeyCode == Keys.Oemtilde) grid.changeLetter("Ö");
            else if (e.KeyData.ToString().Length == 1)
            {
                char c = e.KeyData.ToString()[0];
                if (c < 91 && c > 64)
                {
                    grid.changeLetter(c.ToString());
                }
            }
            else if (e.KeyData == Keys.Back) grid.changeLetter("");
            

            Invalidate();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
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
            grid.findOpen(img);
        }

        private void saveImage()
        {
            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, 0, 0);
                grid.Draw(g, new Point(0, 0), 1, false, false);
            }
            bitmap.Save("solved.png", ImageFormat.Png);
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