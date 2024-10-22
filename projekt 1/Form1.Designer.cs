namespace projekt_1
{
    partial class Form1
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
            Canvas = new PictureBox();
            menuStrip1 = new MenuStrip();
            clearButton = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.BorderStyle = BorderStyle.FixedSingle;
            Canvas.Dock = DockStyle.Fill;
            Canvas.Location = new Point(0, 24);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(800, 426);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            Canvas.Paint += Canvas_Paint;
            Canvas.MouseClick += Canvas_MouseClick;
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.MouseUp += Canvas_MouseUp;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { clearButton });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // clearButton
            // 
            clearButton.Name = "clearButton";
            clearButton.Size = new Size(46, 20);
            clearButton.Text = "Clear";
            clearButton.Click += clearButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Canvas);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Polygon Editor";
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Canvas;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem clearButton;
    }
}
