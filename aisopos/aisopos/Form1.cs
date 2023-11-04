using System.Diagnostics;

namespace aisopos
{
    public partial class Aisopos : Form
    {
        Image? img;
        Point camera;
        float zoom;
        Rectangle sourceRect, destRect;
        bool ctrlDown;
        Grid grid;

        public Aisopos()
        {
            InitializeComponent();
            zoom = 1f;
            ctrlDown = false;
            grid = new Grid(10, 10);
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
            else if (e.KeyCode == Keys.Up && ctrlDown) camera.Y += 50;
            else if (e.KeyCode == Keys.Down && ctrlDown) camera.Y -= 50;
            else if (e.KeyCode == Keys.Left && ctrlDown) camera.X += 50;
            else if (e.KeyCode == Keys.Right && ctrlDown) camera.X -= 50;
            else if (e.KeyCode == Keys.Oemplus) zoom += 0.1f;
            else if (e.KeyCode == Keys.OemMinus) zoom -= 0.1f;
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

            Invalidate();

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (img != null) 
            {
                //Rectangle sourceRect = new Rectangle(0, 0, img.Width, img.Height);
                //Rectangle destRect = new Rectangle(10, 10, 400, 400); // Adjust position as needed
                destRect = new Rectangle(camera.X, camera.Y, (int)(img.Width * zoom), (int)(img.Height * zoom));
                e.Graphics.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel);
            }
            grid.Draw(e.Graphics, camera, zoom);
        }

        private void Aisopos_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.ControlKey)
            {
                ctrlDown = false;
            }
        }
    }
}