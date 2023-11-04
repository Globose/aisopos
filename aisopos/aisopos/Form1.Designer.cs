namespace aisopos
{
    partial class Aisopos
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            SuspendLayout();
            // 
            // Aisopos
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(688, 425);
            DoubleBuffered = true;
            Margin = new Padding(3, 2, 3, 2);
            Name = "Aisopos";
            Text = "Form1";
            KeyDown += Aisopos_KeyDown;
            KeyUp += Aisopos_KeyUp;
            MouseDown += Aisopos_MouseDown;
            ResumeLayout(false);
        }

        #endregion
    }
}