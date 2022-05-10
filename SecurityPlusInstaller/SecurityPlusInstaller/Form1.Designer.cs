namespace SecurityPlusInstaller
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.lblPath = new System.Windows.Forms.Label();
            this.btnSelectDirectory = new System.Windows.Forms.Button();
            this.btnInstall = new System.Windows.Forms.Button();
            this.chkUltimateBackup = new System.Windows.Forms.CheckBox();
            this.chkEUP = new System.Windows.Forms.CheckBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.chkWardrobe = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // lblPath
            // 
            this.lblPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblPath.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.lblPath.Location = new System.Drawing.Point(12, 9);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(487, 23);
            this.lblPath.TabIndex = 0;
            this.lblPath.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // btnSelectDirectory
            // 
            this.btnSelectDirectory.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnSelectDirectory.Location = new System.Drawing.Point(515, 9);
            this.btnSelectDirectory.Name = "btnSelectDirectory";
            this.btnSelectDirectory.Size = new System.Drawing.Size(170, 23);
            this.btnSelectDirectory.TabIndex = 1;
            this.btnSelectDirectory.Text = "Select GTAV Directory";
            this.btnSelectDirectory.UseVisualStyleBackColor = true;
            this.btnSelectDirectory.Click += new System.EventHandler(this.btnSelectDirectory_Click);
            // 
            // btnInstall
            // 
            this.btnInstall.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.btnInstall.Location = new System.Drawing.Point(526, 56);
            this.btnInstall.Name = "btnInstall";
            this.btnInstall.Size = new System.Drawing.Size(159, 38);
            this.btnInstall.TabIndex = 2;
            this.btnInstall.Text = "Install Security+";
            this.btnInstall.UseVisualStyleBackColor = true;
            this.btnInstall.Click += new System.EventHandler(this.btnInstall_Click);
            // 
            // chkUltimateBackup
            // 
            this.chkUltimateBackup.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.chkUltimateBackup.Location = new System.Drawing.Point(12, 38);
            this.chkUltimateBackup.Name = "chkUltimateBackup";
            this.chkUltimateBackup.Size = new System.Drawing.Size(252, 23);
            this.chkUltimateBackup.TabIndex = 3;
            this.chkUltimateBackup.Text = "Install UltimateBackup SpecialUnits.xml";
            this.chkUltimateBackup.UseVisualStyleBackColor = true;
            this.chkUltimateBackup.CheckedChanged += new System.EventHandler(this.chkUltimateBackup_CheckedChanged);
            // 
            // chkEUP
            // 
            this.chkEUP.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.chkEUP.Location = new System.Drawing.Point(12, 67);
            this.chkEUP.Name = "chkEUP";
            this.chkEUP.Size = new System.Drawing.Size(144, 23);
            this.chkEUP.TabIndex = 4;
            this.chkEUP.Text = "Install in EUP mode";
            this.chkEUP.UseVisualStyleBackColor = true;
            this.chkEUP.CheckedChanged += new System.EventHandler(this.chkEUP_CheckedChanged);
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(233, 85);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(245, 28);
            this.progressBar.TabIndex = 5;
            this.progressBar.Visible = false;
            // 
            // chkWardrobe
            // 
            this.chkWardrobe.Enabled = false;
            this.chkWardrobe.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte) (0)));
            this.chkWardrobe.Location = new System.Drawing.Point(12, 90);
            this.chkWardrobe.Name = "chkWardrobe";
            this.chkWardrobe.Size = new System.Drawing.Size(168, 23);
            this.chkWardrobe.TabIndex = 6;
            this.chkWardrobe.Text = "Install new wardrobe.ini";
            this.chkWardrobe.UseVisualStyleBackColor = true;
            this.chkWardrobe.CheckedChanged += new System.EventHandler(this.chkWardrobe_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 125);
            this.Controls.Add(this.chkWardrobe);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.chkEUP);
            this.Controls.Add(this.chkUltimateBackup);
            this.Controls.Add(this.btnInstall);
            this.Controls.Add(this.btnSelectDirectory);
            this.Controls.Add(this.lblPath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Security+ Installer Beta";
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.CheckBox chkWardrobe;

        private System.Windows.Forms.ProgressBar progressBar;

        private System.Windows.Forms.CheckBox chkEUP;

        private System.Windows.Forms.CheckBox chkUltimateBackup;

        private System.Windows.Forms.CheckBox checkBox1;

        private System.Windows.Forms.Button btnInstall;

        private System.Windows.Forms.Button btnSelectDirectory;

        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.Button button1;

        private System.Windows.Forms.Label label1;

        #endregion
    }
}