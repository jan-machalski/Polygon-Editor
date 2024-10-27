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
            removeButton = new Button();
            normalDrawButton = new RadioButton();
            bresenhamButton = new RadioButton();
            label1 = new Label();
            controlsButton = new Button();
            descriptionButton = new Button();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Canvas.BorderStyle = BorderStyle.FixedSingle;
            Canvas.Location = new Point(0, 0);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(800, 450);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            Canvas.Paint += Canvas_Paint;
            Canvas.MouseClick += Canvas_MouseClick;
            Canvas.MouseDown += Canvas_MouseDown;
            Canvas.MouseMove += Canvas_MouseMove;
            Canvas.MouseUp += Canvas_MouseUp;
            // 
            // removeButton
            // 
            removeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            removeButton.Location = new Point(820, 12);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(143, 43);
            removeButton.TabIndex = 2;
            removeButton.Text = "Usuń wielokąt";
            removeButton.UseVisualStyleBackColor = true;
            removeButton.Click += removeButton_Click;
            // 
            // normalDrawButton
            // 
            normalDrawButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            normalDrawButton.AutoSize = true;
            normalDrawButton.Checked = true;
            normalDrawButton.Location = new Point(824, 98);
            normalDrawButton.Name = "normalDrawButton";
            normalDrawButton.Size = new Size(129, 19);
            normalDrawButton.TabIndex = 3;
            normalDrawButton.TabStop = true;
            normalDrawButton.Text = "Domyślny algorytm";
            normalDrawButton.UseVisualStyleBackColor = true;
            normalDrawButton.Click += normalDrawButton_Click;
            // 
            // bresenhamButton
            // 
            bresenhamButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            bresenhamButton.AutoSize = true;
            bresenhamButton.Location = new Point(824, 123);
            bresenhamButton.Name = "bresenhamButton";
            bresenhamButton.Size = new Size(143, 19);
            bresenhamButton.TabIndex = 4;
            bresenhamButton.Text = "Algorytm Bresenhama";
            bresenhamButton.UseVisualStyleBackColor = true;
            bresenhamButton.Click += bresenhamButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(820, 80);
            label1.Name = "label1";
            label1.Size = new Size(162, 15);
            label1.TabIndex = 5;
            label1.Text = "Algorytm rysowania krawędzi";
            // 
            // controlsButton
            // 
            controlsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlsButton.Location = new Point(820, 166);
            controlsButton.Name = "controlsButton";
            controlsButton.Size = new Size(150, 49);
            controlsButton.TabIndex = 6;
            controlsButton.Text = "Opis sterowania";
            controlsButton.UseVisualStyleBackColor = true;
            controlsButton.Click += controlsButton_Click;
            // 
            // descriptionButton
            // 
            descriptionButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            descriptionButton.Location = new Point(820, 230);
            descriptionButton.Name = "descriptionButton";
            descriptionButton.Size = new Size(147, 49);
            descriptionButton.TabIndex = 7;
            descriptionButton.Text = "Opis działania programu";
            descriptionButton.UseVisualStyleBackColor = true;
            descriptionButton.Click += descriptionButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 450);
            Controls.Add(descriptionButton);
            Controls.Add(controlsButton);
            Controls.Add(label1);
            Controls.Add(bresenhamButton);
            Controls.Add(normalDrawButton);
            Controls.Add(removeButton);
            Controls.Add(Canvas);
            Name = "Form1";
            Text = "Polygon Editor";
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Canvas;
        private Button removeButton;
        private RadioButton normalDrawButton;
        private RadioButton bresenhamButton;
        private Label label1;
        private Button controlsButton;
        private Button descriptionButton;
    }
}
