using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

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
        List<Point> points;

        public Aisopos()
        {
            InitializeComponent();
            img = Image.FromFile("C:\\data\\projekt\\aisopos\\t.jpeg");
            zoom = 1f;
            ctrlDown = false;
            grid = new Grid(10, 10);
            sourceRect = new Rectangle(0, 0, img.Width, img.Height);
            destRect = new Rectangle(0, 0, img.Width, img.Height);
            gMode = false;
            points = new List<Point>();
        }

        private void Aisopos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.P)
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
            else if (e.KeyCode == Keys.Up && ctrlDown) camera.Y += 22;
            else if (e.KeyCode == Keys.Down && ctrlDown) camera.Y -= 22;
            else if (e.KeyCode == Keys.Left && ctrlDown) camera.X += 22;
            else if (e.KeyCode == Keys.Right && ctrlDown) camera.X -= 22;
            else if (e.KeyCode == Keys.Oemplus) zoom *= 1.03f;
            else if (e.KeyCode == Keys.OemMinus) zoom *= 0.97f;
            else if (e.KeyCode == Keys.E) grid.reStructure(0, 1);
            else if (e.KeyCode == Keys.Q) grid.reStructure(0, -1);
            else if (e.KeyCode == Keys.Z) grid.reStructure(-1, 0);
            else if (e.KeyCode == Keys.C) grid.reStructure(1, 0);
            else if (e.KeyCode == Keys.A) grid.position = new Point(grid.position.X - 1, grid.position.Y);
            else if (e.KeyCode == Keys.D) grid.position = new Point(grid.position.X + 1, grid.position.Y);
            else if (e.KeyCode == Keys.W) grid.position = new Point(grid.position.X, grid.position.Y - 1);
            else if (e.KeyCode == Keys.S) grid.position = new Point(grid.position.X, grid.position.Y + 1);
            else if (e.KeyCode == Keys.D1) grid.sqSize += 0.2f;
            else if (e.KeyCode == Keys.D2) grid.sqSize -= 0.2f;
            else if (e.KeyCode == Keys.T) saveImage();
            else if (e.KeyCode == Keys.G) gMode = !gMode;
            else if (e.KeyCode == Keys.Enter && gMode) createGrid();
            {

            }

            Invalidate();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            destRect = new Rectangle(camera.X, camera.Y, (int)(img.Width * zoom), (int)(img.Height * zoom));
            e.Graphics.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel);
            
            if (gMode)
            {
                grid.Draw(e.Graphics, camera, zoom);
            }
            foreach (Point p in points)
            {
                e.Graphics.FillEllipse(new SolidBrush(Color.Red),  (int)(zoom*p.X)-5+camera.X, (int)(zoom*p.Y)-5+camera.Y, 10, 10);
            }
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

        }

        private void saveImage()
        {
            Bitmap bitmap = new Bitmap(img.Width, img.Height);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.DrawImage(img, 0, 0);
                foreach (Point p in points)
                {
                    g.FillEllipse(new SolidBrush(Color.Red), p.X - 5, p.Y - 5, 10, 10);
                }
            }
            bitmap.Save("temp.png", ImageFormat.Png);
        }

        private void Aisopos_MouseDown(object sender, MouseEventArgs e)
        {
            points.Add(new Point((int)((e.X-camera.X)/zoom), (int)((e.Y-camera.Y)/zoom)));
            int x = (int)((e.X - camera.X) / zoom);
            int y = (int)((e.Y - camera.Y) / zoom);
            Invalidate();
        }
    }
}