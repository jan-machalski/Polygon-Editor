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
            removeButton = new Button();
            normalDrawButton = new RadioButton();
            bresenhamButton = new RadioButton();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            Canvas.BorderStyle = BorderStyle.FixedSingle;
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
            menuStrip1.Size = new Size(984, 24);
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
            // removeButton
            // 
            removeButton.Location = new Point(820, 24);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(143, 43);
            removeButton.TabIndex = 2;
            removeButton.Text = "Remove Polygon";
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += removeButton_Click;
            // 
            // normalDrawButton
            // 
            normalDrawButton.AutoSize = true;
            normalDrawButton.Checked = true;
            normalDrawButton.Location = new Point(824, 119);
            normalDrawButton.Name = "normalDrawButton";
            normalDrawButton.Size = new Size(118, 19);
            normalDrawButton.TabIndex = 3;
            normalDrawButton.TabStop = true;
            normalDrawButton.Text = "Default algorithm";
            normalDrawButton.UseVisualStyleBackColor = true;
            normalDrawButton.Click += normalDrawButton_Click;
            // 
            // bresenhamButton
            // 
            bresenhamButton.AutoSize = true;
            bresenhamButton.Location = new Point(824, 144);
            bresenhamButton.Name = "bresenhamButton";
            bresenhamButton.Size = new Size(139, 19);
            bresenhamButton.TabIndex = 4;
            bresenhamButton.Text = "Bresenham algorithm";
            bresenhamButton.UseVisualStyleBackColor = true;
            bresenhamButton.Click += bresenhamButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(820, 92);
            label1.Name = "label1";
            label1.Size = new Size(134, 15);
            label1.TabIndex = 5;
            label1.Text = "Edge drawing algorithm";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 450);
            Controls.Add(label1);
            Controls.Add(bresenhamButton);
            Controls.Add(normalDrawButton);
            Controls.Add(removeButton);
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
        private Button removeButton;
        private RadioButton normalDrawButton;
        private RadioButton bresenhamButton;
        private Label label1;
    }
}
