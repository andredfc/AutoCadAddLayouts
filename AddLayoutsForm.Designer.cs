namespace AutoCadAddLayouts
{
    partial class AddLayoutsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblInfo = new System.Windows.Forms.Label();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.lstDwgs = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // lblInfo
            // 
            this.lblInfo.AutoSize = true;
            this.lblInfo.Location = new System.Drawing.Point(18, 376);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(34, 13);
            this.lblInfo.TabIndex = 13;
            this.lblInfo.Text = "Info...";
            // 
            // btnConfirm
            // 
            this.btnConfirm.Location = new System.Drawing.Point(278, 400);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(75, 23);
            this.btnConfirm.TabIndex = 12;
            this.btnConfirm.Text = "Confirm";
            this.btnConfirm.UseVisualStyleBackColor = true;
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);
            // 
            // lstDwgs
            // 
            this.lstDwgs.FormattingEnabled = true;
            this.lstDwgs.Location = new System.Drawing.Point(18, 118);
            this.lstDwgs.Name = "lstDwgs";
            this.lstDwgs.Size = new System.Drawing.Size(336, 251);
            this.lstDwgs.TabIndex = 11;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 91);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Files to merge layout";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(279, 28);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 9;
            this.btnBrowse.Text = "Browse...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // txtPath
            // 
            this.txtPath.Location = new System.Drawing.Point(53, 28);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(220, 20);
            this.txtPath.TabIndex = 8;
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(15, 28);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(32, 13);
            this.lblPath.TabIndex = 7;
            this.lblPath.Text = "Path:";
            // 
            // AddLayoutsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(369, 450);
            this.Controls.Add(this.lblInfo);
            this.Controls.Add(this.btnConfirm);
            this.Controls.Add(this.lstDwgs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnBrowse);
            this.Controls.Add(this.txtPath);
            this.Controls.Add(this.lblPath);
            this.Name = "AddLayoutsForm";
            this.Text = "AddLayoutsForm";
            this.Load += new System.EventHandler(this.AddLayoutsForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblInfo;
        private System.Windows.Forms.Button btnConfirm;
        private System.Windows.Forms.ListBox lstDwgs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Label lblPath;
    }
}