namespace FlotsamTypist
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.TextOverlay = new WinFormsControls.TransparentLabel();
            this.TypewriterPhrase = new WinFormsControls.TransparentLabel();
            this.Underliner = new WinFormsControls.TransparentLabel();
            this.KeyboardHint = new WinFormsControls.TransparentLabel();
            this.SuspendLayout();
            // 
            // TextOverlay
            // 
            this.TextOverlay.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TextOverlay.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.TextOverlay.Location = new System.Drawing.Point(28, 32);
            this.TextOverlay.Name = "TextOverlay";
            this.TextOverlay.Size = new System.Drawing.Size(775, 32);
            this.TextOverlay.TabIndex = 2;
            this.TextOverlay.TabStop = false;
            this.TextOverlay.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // TypewriterPhrase
            // 
            this.TypewriterPhrase.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypewriterPhrase.ForeColor = System.Drawing.Color.Khaki;
            this.TypewriterPhrase.Location = new System.Drawing.Point(28, 32);
            this.TypewriterPhrase.Name = "TypewriterPhrase";
            this.TypewriterPhrase.Size = new System.Drawing.Size(775, 32);
            this.TypewriterPhrase.TabIndex = 0;
            this.TypewriterPhrase.TabStop = false;
            this.TypewriterPhrase.Text = "YMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMY";
            this.TypewriterPhrase.TextAlign = System.Drawing.ContentAlignment.TopLeft;
            // 
            // Underliner
            // 
            this.Underliner.Font = new System.Drawing.Font("Consolas", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Underliner.ForeColor = System.Drawing.Color.Crimson;
            this.Underliner.Location = new System.Drawing.Point(28, 34);
            this.Underliner.Name = "Underliner";
            this.Underliner.Size = new System.Drawing.Size(775, 41);
            this.Underliner.TabIndex = 1;
            this.Underliner.TabStop = false;
            this.Underliner.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // KeyboardHint
            // 
            this.KeyboardHint.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyboardHint.ForeColor = System.Drawing.Color.Orange;
            this.KeyboardHint.Location = new System.Drawing.Point(28, 5);
            this.KeyboardHint.Name = "KeyboardHint";
            this.KeyboardHint.Size = new System.Drawing.Size(20, 27);
            this.KeyboardHint.TabIndex = 2;
            this.KeyboardHint.TabStop = false;
            this.KeyboardHint.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(826, 104);
            this.Controls.Add(this.TextOverlay);
            this.Controls.Add(this.TypewriterPhrase);
            this.Controls.Add(this.Underliner);
            this.Controls.Add(this.KeyboardHint);
            this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Flotsam Typist";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private WinFormsControls.TransparentLabel TypewriterPhrase;
        private WinFormsControls.TransparentLabel Underliner;
        private WinFormsControls.TransparentLabel TextOverlay;
        private WinFormsControls.TransparentLabel KeyboardHint;
    }
}

