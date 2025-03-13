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
            initialFigureButton = new Button();
            wuButton = new RadioButton();
            saveButton = new Button();
            loadButton = new Button();
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
            controlsButton.Location = new Point(822, 194);
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
            descriptionButton.Location = new Point(822, 261);
            descriptionButton.Name = "descriptionButton";
            descriptionButton.Size = new Size(147, 49);
            descriptionButton.TabIndex = 7;
            descriptionButton.Text = "Opis działania programu";
            descriptionButton.UseVisualStyleBackColor = true;
            descriptionButton.Click += descriptionButton_Click;
            // 
            // initialFigureButton
            // 
            initialFigureButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            initialFigureButton.Location = new Point(824, 316);
            initialFigureButton.Name = "initialFigureButton";
            initialFigureButton.Size = new Size(143, 51);
            initialFigureButton.TabIndex = 8;
            initialFigureButton.Text = "Wróć do figury początkowej";
            initialFigureButton.UseVisualStyleBackColor = true;
            initialFigureButton.Click += initialFigureButton_Click;
            // 
            // wuButton
            // 
            wuButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            wuButton.AutoSize = true;
            wuButton.Location = new Point(824, 145);
            wuButton.Margin = new Padding(2, 2, 2, 2);
            wuButton.Name = "wuButton";
            wuButton.Size = new Size(97, 19);
            wuButton.TabIndex = 9;
            wuButton.TabStop = true;
            wuButton.Text = "Algorytm WU";
            wuButton.UseVisualStyleBackColor = true;
            wuButton.Click += wuButton_Click;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveButton.Location = new Point(824, 380);
            saveButton.Margin = new Padding(2, 2, 2, 2);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(66, 58);
            saveButton.TabIndex = 10;
            saveButton.Text = "Zapisz figurę";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // loadButton
            // 
            loadButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            loadButton.Location = new Point(902, 380);
            loadButton.Margin = new Padding(2, 2, 2, 2);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(65, 58);
            loadButton.TabIndex = 11;
            loadButton.Text = "Wyświetl zapisaną figurę";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 450);
            Controls.Add(loadButton);
            Controls.Add(saveButton);
            Controls.Add(wuButton);
            Controls.Add(initialFigureButton);
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
        private Button initialFigureButton;
        private RadioButton wuButton;
        private Button saveButton;
        private Button loadButton;
    }
}
