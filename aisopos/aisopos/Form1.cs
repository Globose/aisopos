using System.Diagnostics;

namespace aisopos
{
    public partial class Aisopos : Form
    {
        Image? img;
        Point imgPos;
        float zoom;
        Rectangle sourceRect, destRect;
        bool ctrlDown;
        Grid grid;

        public Aisopos()
        {
            InitializeComponent();
            imgPos = new Point(0, 0);
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
            else if (e.KeyCode == Keys.ControlKey)
            {
                ctrlDown= true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (ctrlDown)
                {
                    imgPos.Y += 50;
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (ctrlDown)
                {
                    imgPos.Y -= 50;
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Left)
            {
                if (ctrlDown)
                {
                    imgPos.X += 50;
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Right)
            {
                if (ctrlDown)
                {
                    imgPos.X -= 50;
                    Invalidate();
                }
            }
            else if (e.KeyCode == Keys.Oemplus)
            {
                zoom += 0.1f;
                Invalidate();
            }
            else if (e.KeyCode == Keys.OemMinus)
            {
                zoom -= 0.1f;
                Invalidate();
            }

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (img != null) 
            {
                //Rectangle sourceRect = new Rectangle(0, 0, img.Width, img.Height);
                //Rectangle destRect = new Rectangle(10, 10, 400, 400); // Adjust position as needed
                destRect = new Rectangle(imgPos.X, imgPos.Y, (int)(img.Width * zoom), (int)(img.Height * zoom));
                e.Graphics.DrawImage(img, destRect, sourceRect, GraphicsUnit.Pixel);
            }
            grid.Draw(e.Graphics);
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