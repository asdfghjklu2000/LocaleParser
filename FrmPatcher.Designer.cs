namespace LocaleParser
{
    partial class FrmPatcher
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
            this.components = new System.ComponentModel.Container();
            this.tb_version = new System.Windows.Forms.TextBox();
            this.tb_versionkey = new System.Windows.Forms.TextBox();
            this.tb_cus_config_url = new System.Windows.Forms.TextBox();
            this.tb_cus_patch_url = new System.Windows.Forms.TextBox();
            this.btn_dl_switch = new System.Windows.Forms.Button();
            this.btn_dl_all = new System.Windows.Forms.Button();
            this.btn_upd_vkey = new System.Windows.Forms.Button();
            this.btn_exit = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_cus_config = new System.Windows.Forms.CheckBox();
            this.cb_cus_patch = new System.Windows.Forms.CheckBox();
            this.tm_patch = new System.Windows.Forms.Timer(this.components);
            this.label3 = new System.Windows.Forms.Label();
            this.tb_minute = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // tb_version
            // 
            this.tb_version.Location = new System.Drawing.Point(172, 4);
            this.tb_version.Name = "tb_version";
            this.tb_version.Size = new System.Drawing.Size(205, 22);
            this.tb_version.TabIndex = 0;
            // 
            // tb_versionkey
            // 
            this.tb_versionkey.Location = new System.Drawing.Point(172, 41);
            this.tb_versionkey.Name = "tb_versionkey";
            this.tb_versionkey.Size = new System.Drawing.Size(205, 22);
            this.tb_versionkey.TabIndex = 1;
            // 
            // tb_cus_config_url
            // 
            this.tb_cus_config_url.Location = new System.Drawing.Point(106, 83);
            this.tb_cus_config_url.Name = "tb_cus_config_url";
            this.tb_cus_config_url.Size = new System.Drawing.Size(166, 22);
            this.tb_cus_config_url.TabIndex = 2;
            // 
            // tb_cus_patch_url
            // 
            this.tb_cus_patch_url.Location = new System.Drawing.Point(106, 124);
            this.tb_cus_patch_url.Name = "tb_cus_patch_url";
            this.tb_cus_patch_url.Size = new System.Drawing.Size(166, 22);
            this.tb_cus_patch_url.TabIndex = 3;
            // 
            // btn_dl_switch
            // 
            this.btn_dl_switch.Location = new System.Drawing.Point(10, 205);
            this.btn_dl_switch.Name = "btn_dl_switch";
            this.btn_dl_switch.Size = new System.Drawing.Size(75, 45);
            this.btn_dl_switch.TabIndex = 4;
            this.btn_dl_switch.Text = "Start";
            this.btn_dl_switch.UseVisualStyleBackColor = true;
            this.btn_dl_switch.Click += new System.EventHandler(this.btn_dl_switch_Click);
            // 
            // btn_dl_all
            // 
            this.btn_dl_all.Location = new System.Drawing.Point(91, 205);
            this.btn_dl_all.Name = "btn_dl_all";
            this.btn_dl_all.Size = new System.Drawing.Size(75, 45);
            this.btn_dl_all.TabIndex = 5;
            this.btn_dl_all.Text = "Download\r\nAll";
            this.btn_dl_all.UseVisualStyleBackColor = true;
            // 
            // btn_upd_vkey
            // 
            this.btn_upd_vkey.Location = new System.Drawing.Point(25, 12);
            this.btn_upd_vkey.Name = "btn_upd_vkey";
            this.btn_upd_vkey.Size = new System.Drawing.Size(75, 45);
            this.btn_upd_vkey.TabIndex = 6;
            this.btn_upd_vkey.Text = "Update\r\nversion";
            this.btn_upd_vkey.UseVisualStyleBackColor = true;
            this.btn_upd_vkey.Click += new System.EventHandler(this.btn_upd_vkey_Click);
            // 
            // btn_exit
            // 
            this.btn_exit.Location = new System.Drawing.Point(302, 205);
            this.btn_exit.Name = "btn_exit";
            this.btn_exit.Size = new System.Drawing.Size(75, 45);
            this.btn_exit.TabIndex = 7;
            this.btn_exit.Text = "Exit";
            this.btn_exit.UseVisualStyleBackColor = true;
            this.btn_exit.Click += new System.EventHandler(this.btn_exit_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(113, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 8;
            this.label1.Text = "Version：";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(113, 44);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "VerKey：";
            // 
            // cb_cus_config
            // 
            this.cb_cus_config.AutoSize = true;
            this.cb_cus_config.Location = new System.Drawing.Point(10, 89);
            this.cb_cus_config.Name = "cb_cus_config";
            this.cb_cus_config.Size = new System.Drawing.Size(90, 16);
            this.cb_cus_config.TabIndex = 10;
            this.cb_cus_config.Text = "Cus_config：";
            this.cb_cus_config.UseVisualStyleBackColor = true;
            // 
            // cb_cus_patch
            // 
            this.cb_cus_patch.AutoSize = true;
            this.cb_cus_patch.Location = new System.Drawing.Point(10, 130);
            this.cb_cus_patch.Name = "cb_cus_patch";
            this.cb_cus_patch.Size = new System.Drawing.Size(85, 16);
            this.cb_cus_patch.TabIndex = 11;
            this.cb_cus_patch.Text = "Cus_patch：";
            this.cb_cus_patch.UseVisualStyleBackColor = true;
            // 
            // tm_patch
            // 
            this.tm_patch.Tick += new System.EventHandler(this.tm_patch_Tick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 12);
            this.label3.TabIndex = 13;
            this.label3.Text = "In Minutes：";
            // 
            // tb_minute
            // 
            this.tb_minute.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.tb_minute.Location = new System.Drawing.Point(85, 177);
            this.tb_minute.Name = "tb_minute";
            this.tb_minute.Size = new System.Drawing.Size(44, 22);
            this.tb_minute.TabIndex = 12;
            this.tb_minute.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_minute_KeyPress);
            // 
            // FrmPatcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 262);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tb_minute);
            this.Controls.Add(this.cb_cus_patch);
            this.Controls.Add(this.cb_cus_config);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btn_exit);
            this.Controls.Add(this.btn_upd_vkey);
            this.Controls.Add(this.btn_dl_all);
            this.Controls.Add(this.btn_dl_switch);
            this.Controls.Add(this.tb_cus_patch_url);
            this.Controls.Add(this.tb_cus_config_url);
            this.Controls.Add(this.tb_versionkey);
            this.Controls.Add(this.tb_version);
            this.Name = "FrmPatcher";
            this.Text = "FrmPatcher";
            this.Load += new System.EventHandler(this.FrmPatcher_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_version;
        private System.Windows.Forms.TextBox tb_versionkey;
        private System.Windows.Forms.TextBox tb_cus_config_url;
        private System.Windows.Forms.TextBox tb_cus_patch_url;
        private System.Windows.Forms.Button btn_dl_switch;
        private System.Windows.Forms.Button btn_dl_all;
        private System.Windows.Forms.Button btn_upd_vkey;
        private System.Windows.Forms.Button btn_exit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox cb_cus_config;
        private System.Windows.Forms.CheckBox cb_cus_patch;
        private System.Windows.Forms.Timer tm_patch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_minute;
    }
}