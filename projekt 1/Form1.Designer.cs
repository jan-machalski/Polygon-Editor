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
            Canvas.Margin = new Padding(4, 5, 4, 5);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(1142, 749);
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
            removeButton.Location = new Point(1171, 20);
            removeButton.Margin = new Padding(4, 5, 4, 5);
            removeButton.Name = "removeButton";
            removeButton.Size = new Size(204, 72);
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
            normalDrawButton.Location = new Point(1167, 163);
            normalDrawButton.Margin = new Padding(4, 5, 4, 5);
            normalDrawButton.Name = "normalDrawButton";
            normalDrawButton.Size = new Size(194, 29);
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
            bresenhamButton.Location = new Point(1168, 205);
            bresenhamButton.Margin = new Padding(4, 5, 4, 5);
            bresenhamButton.Name = "bresenhamButton";
            bresenhamButton.Size = new Size(213, 29);
            bresenhamButton.TabIndex = 4;
            bresenhamButton.Text = "Algorytm Bresenhama";
            bresenhamButton.UseVisualStyleBackColor = true;
            bresenhamButton.Click += bresenhamButton_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label1.AutoSize = true;
            label1.Location = new Point(1171, 133);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(245, 25);
            label1.TabIndex = 5;
            label1.Text = "Algorytm rysowania krawędzi";
            // 
            // controlsButton
            // 
            controlsButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            controlsButton.Location = new Point(1175, 323);
            controlsButton.Margin = new Padding(4, 5, 4, 5);
            controlsButton.Name = "controlsButton";
            controlsButton.Size = new Size(214, 82);
            controlsButton.TabIndex = 6;
            controlsButton.Text = "Opis sterowania";
            controlsButton.UseVisualStyleBackColor = true;
            controlsButton.Click += controlsButton_Click;
            // 
            // descriptionButton
            // 
            descriptionButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            descriptionButton.Location = new Point(1175, 435);
            descriptionButton.Margin = new Padding(4, 5, 4, 5);
            descriptionButton.Name = "descriptionButton";
            descriptionButton.Size = new Size(210, 82);
            descriptionButton.TabIndex = 7;
            descriptionButton.Text = "Opis działania programu";
            descriptionButton.UseVisualStyleBackColor = true;
            descriptionButton.Click += descriptionButton_Click;
            // 
            // initialFigureButton
            // 
            initialFigureButton.Location = new Point(1177, 527);
            initialFigureButton.Margin = new Padding(4, 5, 4, 5);
            initialFigureButton.Name = "initialFigureButton";
            initialFigureButton.Size = new Size(204, 85);
            initialFigureButton.TabIndex = 8;
            initialFigureButton.Text = "Wróć do figury początkowej";
            initialFigureButton.UseVisualStyleBackColor = true;
            initialFigureButton.Click += initialFigureButton_Click;
            // 
            // wuButton
            // 
            wuButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            wuButton.AutoSize = true;
            wuButton.Location = new Point(1170, 242);
            wuButton.Name = "wuButton";
            wuButton.Size = new Size(146, 29);
            wuButton.TabIndex = 9;
            wuButton.TabStop = true;
            wuButton.Text = "Algorytm WU";
            wuButton.UseVisualStyleBackColor = true;
            wuButton.Click += wuButton_Click;
            // 
            // saveButton
            // 
            saveButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            saveButton.Location = new Point(1177, 633);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(95, 96);
            saveButton.TabIndex = 10;
            saveButton.Text = "Zapisz figurę";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // loadButton
            // 
            loadButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            loadButton.Location = new Point(1288, 633);
            loadButton.Name = "loadButton";
            loadButton.Size = new Size(93, 96);
            loadButton.TabIndex = 11;
            loadButton.Text = "Wyświetl zapisaną figurę";
            loadButton.UseVisualStyleBackColor = true;
            loadButton.Click += loadButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1406, 750);
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
            Margin = new Padding(4, 5, 4, 5);
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
