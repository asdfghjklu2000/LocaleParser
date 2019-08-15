namespace LocaleParser
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.label1 = new System.Windows.Forms.Label();
            this.txt_path = new System.Windows.Forms.TextBox();
            this.gb_locale = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btn_txt = new System.Windows.Forms.Button();
            this.btn_xlsx = new System.Windows.Forms.Button();
            this.btn_file = new System.Windows.Forms.Button();
            this.btn_close = new System.Windows.Forms.Button();
            this.gb_patcher = new System.Windows.Forms.GroupBox();
            this.cb_cus_patch = new System.Windows.Forms.CheckBox();
            this.cb_cus_config = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_upd_vkey = new System.Windows.Forms.Button();
            this.btn_dl_all = new System.Windows.Forms.Button();
            this.btn_dl_switch = new System.Windows.Forms.Button();
            this.tb_cus_patch_url = new System.Windows.Forms.TextBox();
            this.tb_cus_config_url = new System.Windows.Forms.TextBox();
            this.tb_versionkey = new System.Windows.Forms.TextBox();
            this.tb_version = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.tb_minute = new System.Windows.Forms.TextBox();
            this.tm_patch = new System.Windows.Forms.Timer(this.components);
            this.gb_cutter = new System.Windows.Forms.GroupBox();
            this.cb_100px = new System.Windows.Forms.CheckBox();
            this.btn_merge = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.btn_cut = new System.Windows.Forms.Button();
            this.rtb_pos = new System.Windows.Forms.RichTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_chs_rgb = new System.Windows.Forms.Button();
            this.txt_rgb = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btn_chs_alp = new System.Windows.Forms.Button();
            this.txt_alp = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txt_zone = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_logindata_en = new System.Windows.Forms.Button();
            this.btn_data_switch = new System.Windows.Forms.Button();
            this.btn_gamedata = new System.Windows.Forms.Button();
            this.btn_dairylog = new System.Windows.Forms.Button();
            this.btn_logindata = new System.Windows.Forms.Button();
            this.tm_login_packet = new System.Windows.Forms.Timer(this.components);
            this.gb_locale.SuspendLayout();
            this.gb_patcher.SuspendLayout();
            this.gb_cutter.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 119);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "路徑：";
            // 
            // txt_path
            // 
            this.txt_path.Location = new System.Drawing.Point(53, 109);
            this.txt_path.Name = "txt_path";
            this.txt_path.Size = new System.Drawing.Size(213, 22);
            this.txt_path.TabIndex = 1;
            // 
            // gb_locale
            // 
            this.gb_locale.Controls.Add(this.label2);
            this.gb_locale.Controls.Add(this.btn_txt);
            this.gb_locale.Controls.Add(this.btn_xlsx);
            this.gb_locale.Controls.Add(this.btn_file);
            this.gb_locale.Controls.Add(this.label1);
            this.gb_locale.Controls.Add(this.txt_path);
            this.gb_locale.Location = new System.Drawing.Point(454, 13);
            this.gb_locale.Name = "gb_locale";
            this.gb_locale.Size = new System.Drawing.Size(269, 216);
            this.gb_locale.TabIndex = 2;
            this.gb_locale.TabStop = false;
            this.gb_locale.Text = "功能區";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("新細明體", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            this.label2.ForeColor = System.Drawing.Color.Red;
            this.label2.Location = new System.Drawing.Point(168, 169);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(87, 36);
            this.label2.TabIndex = 7;
            this.label2.Text = "※ 轉換後檔案\r\n會放在locale\r\n同資料夾";
            // 
            // btn_txt
            // 
            this.btn_txt.BackColor = System.Drawing.Color.OliveDrab;
            this.btn_txt.Location = new System.Drawing.Point(87, 142);
            this.btn_txt.Name = "btn_txt";
            this.btn_txt.Size = new System.Drawing.Size(75, 67);
            this.btn_txt.TabIndex = 6;
            this.btn_txt.Text = "轉為txt";
            this.btn_txt.UseVisualStyleBackColor = false;
            this.btn_txt.Click += new System.EventHandler(this.btn_xlsx_Click);
            // 
            // btn_xlsx
            // 
            this.btn_xlsx.BackColor = System.Drawing.Color.Tomato;
            this.btn_xlsx.Location = new System.Drawing.Point(6, 142);
            this.btn_xlsx.Name = "btn_xlsx";
            this.btn_xlsx.Size = new System.Drawing.Size(75, 67);
            this.btn_xlsx.TabIndex = 5;
            this.btn_xlsx.Text = "轉為xlsx";
            this.btn_xlsx.UseVisualStyleBackColor = false;
            this.btn_xlsx.Click += new System.EventHandler(this.btn_xlsx_Click);
            // 
            // btn_file
            // 
            this.btn_file.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_file.Location = new System.Drawing.Point(6, 22);
            this.btn_file.Name = "btn_file";
            this.btn_file.Size = new System.Drawing.Size(156, 67);
            this.btn_file.TabIndex = 4;
            this.btn_file.Text = "選擇檔案\r\n(或直接於下方輸入路徑)";
            this.btn_file.UseVisualStyleBackColor = false;
            this.btn_file.Click += new System.EventHandler(this.btn_file_Click);
            // 
            // btn_close
            // 
            this.btn_close.Location = new System.Drawing.Point(648, 409);
            this.btn_close.Name = "btn_close";
            this.btn_close.Size = new System.Drawing.Size(75, 67);
            this.btn_close.TabIndex = 3;
            this.btn_close.Text = "離開";
            this.btn_close.UseVisualStyleBackColor = true;
            this.btn_close.Click += new System.EventHandler(this.btn_close_Click);
            // 
            // gb_patcher
            // 
            this.gb_patcher.Controls.Add(this.cb_cus_patch);
            this.gb_patcher.Controls.Add(this.cb_cus_config);
            this.gb_patcher.Controls.Add(this.label3);
            this.gb_patcher.Controls.Add(this.label4);
            this.gb_patcher.Controls.Add(this.btn_upd_vkey);
            this.gb_patcher.Controls.Add(this.btn_dl_all);
            this.gb_patcher.Controls.Add(this.btn_dl_switch);
            this.gb_patcher.Controls.Add(this.tb_cus_patch_url);
            this.gb_patcher.Controls.Add(this.tb_cus_config_url);
            this.gb_patcher.Controls.Add(this.tb_versionkey);
            this.gb_patcher.Controls.Add(this.tb_version);
            this.gb_patcher.Controls.Add(this.label5);
            this.gb_patcher.Controls.Add(this.tb_minute);
            this.gb_patcher.Location = new System.Drawing.Point(12, 12);
            this.gb_patcher.Name = "gb_patcher";
            this.gb_patcher.Size = new System.Drawing.Size(436, 217);
            this.gb_patcher.TabIndex = 4;
            this.gb_patcher.TabStop = false;
            this.gb_patcher.Text = "抓取更新檔";
            // 
            // cb_cus_patch
            // 
            this.cb_cus_patch.AutoSize = true;
            this.cb_cus_patch.Location = new System.Drawing.Point(218, 175);
            this.cb_cus_patch.Name = "cb_cus_patch";
            this.cb_cus_patch.Size = new System.Drawing.Size(85, 16);
            this.cb_cus_patch.TabIndex = 25;
            this.cb_cus_patch.Text = "Cus_patch：";
            this.cb_cus_patch.UseVisualStyleBackColor = true;
            this.cb_cus_patch.Visible = false;
            // 
            // cb_cus_config
            // 
            this.cb_cus_config.AutoSize = true;
            this.cb_cus_config.Location = new System.Drawing.Point(218, 134);
            this.cb_cus_config.Name = "cb_cus_config";
            this.cb_cus_config.Size = new System.Drawing.Size(90, 16);
            this.cb_cus_config.TabIndex = 24;
            this.cb_cus_config.Text = "Cus_config：";
            this.cb_cus_config.UseVisualStyleBackColor = true;
            this.cb_cus_config.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(138, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 23;
            this.label3.Text = "VerKey：";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 22;
            this.label4.Text = "Version：";
            // 
            // btn_upd_vkey
            // 
            this.btn_upd_vkey.Location = new System.Drawing.Point(50, 30);
            this.btn_upd_vkey.Name = "btn_upd_vkey";
            this.btn_upd_vkey.Size = new System.Drawing.Size(75, 45);
            this.btn_upd_vkey.TabIndex = 20;
            this.btn_upd_vkey.Text = "Update\r\nversion";
            this.btn_upd_vkey.UseVisualStyleBackColor = true;
            this.btn_upd_vkey.Click += new System.EventHandler(this.btn_upd_vkey_Click);
            // 
            // btn_dl_all
            // 
            this.btn_dl_all.Location = new System.Drawing.Point(116, 144);
            this.btn_dl_all.Name = "btn_dl_all";
            this.btn_dl_all.Size = new System.Drawing.Size(75, 45);
            this.btn_dl_all.TabIndex = 19;
            this.btn_dl_all.Text = "Download\r\nAll";
            this.btn_dl_all.UseVisualStyleBackColor = true;
            // 
            // btn_dl_switch
            // 
            this.btn_dl_switch.Location = new System.Drawing.Point(35, 144);
            this.btn_dl_switch.Name = "btn_dl_switch";
            this.btn_dl_switch.Size = new System.Drawing.Size(75, 45);
            this.btn_dl_switch.TabIndex = 18;
            this.btn_dl_switch.Text = "Start";
            this.btn_dl_switch.UseVisualStyleBackColor = true;
            this.btn_dl_switch.Click += new System.EventHandler(this.btn_dl_switch_Click);
            // 
            // tb_cus_patch_url
            // 
            this.tb_cus_patch_url.Location = new System.Drawing.Point(314, 169);
            this.tb_cus_patch_url.Name = "tb_cus_patch_url";
            this.tb_cus_patch_url.Size = new System.Drawing.Size(166, 22);
            this.tb_cus_patch_url.TabIndex = 17;
            this.tb_cus_patch_url.Visible = false;
            // 
            // tb_cus_config_url
            // 
            this.tb_cus_config_url.Location = new System.Drawing.Point(314, 128);
            this.tb_cus_config_url.Name = "tb_cus_config_url";
            this.tb_cus_config_url.Size = new System.Drawing.Size(166, 22);
            this.tb_cus_config_url.TabIndex = 16;
            this.tb_cus_config_url.Visible = false;
            // 
            // tb_versionkey
            // 
            this.tb_versionkey.Location = new System.Drawing.Point(197, 59);
            this.tb_versionkey.Name = "tb_versionkey";
            this.tb_versionkey.Size = new System.Drawing.Size(205, 22);
            this.tb_versionkey.TabIndex = 15;
            // 
            // tb_version
            // 
            this.tb_version.Location = new System.Drawing.Point(197, 22);
            this.tb_version.Name = "tb_version";
            this.tb_version.Size = new System.Drawing.Size(205, 22);
            this.tb_version.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(37, 119);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 12);
            this.label5.TabIndex = 27;
            this.label5.Text = "In Minutes：";
            // 
            // tb_minute
            // 
            this.tb_minute.ImeMode = System.Windows.Forms.ImeMode.Alpha;
            this.tb_minute.Location = new System.Drawing.Point(110, 116);
            this.tb_minute.Name = "tb_minute";
            this.tb_minute.Size = new System.Drawing.Size(44, 22);
            this.tb_minute.TabIndex = 26;
            this.tb_minute.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_minute_KeyPress);
            // 
            // tm_patch
            // 
            this.tm_patch.Tick += new System.EventHandler(this.tm_patch_Tick);
            // 
            // gb_cutter
            // 
            this.gb_cutter.Controls.Add(this.cb_100px);
            this.gb_cutter.Controls.Add(this.btn_merge);
            this.gb_cutter.Controls.Add(this.label8);
            this.gb_cutter.Controls.Add(this.btn_cut);
            this.gb_cutter.Controls.Add(this.rtb_pos);
            this.gb_cutter.Controls.Add(this.label7);
            this.gb_cutter.Controls.Add(this.btn_chs_rgb);
            this.gb_cutter.Controls.Add(this.txt_rgb);
            this.gb_cutter.Controls.Add(this.label6);
            this.gb_cutter.Controls.Add(this.btn_chs_alp);
            this.gb_cutter.Controls.Add(this.txt_alp);
            this.gb_cutter.Location = new System.Drawing.Point(12, 235);
            this.gb_cutter.Name = "gb_cutter";
            this.gb_cutter.Size = new System.Drawing.Size(436, 241);
            this.gb_cutter.TabIndex = 5;
            this.gb_cutter.TabStop = false;
            this.gb_cutter.Text = "圖片切割器";
            // 
            // cb_100px
            // 
            this.cb_100px.AutoSize = true;
            this.cb_100px.Location = new System.Drawing.Point(308, 139);
            this.cb_100px.Name = "cb_100px";
            this.cb_100px.Size = new System.Drawing.Size(66, 16);
            this.cb_100px.TabIndex = 15;
            this.cb_100px.Text = "to 100px";
            this.cb_100px.UseVisualStyleBackColor = true;
            // 
            // btn_merge
            // 
            this.btn_merge.BackColor = System.Drawing.Color.Tomato;
            this.btn_merge.Location = new System.Drawing.Point(8, 158);
            this.btn_merge.Name = "btn_merge";
            this.btn_merge.Size = new System.Drawing.Size(75, 67);
            this.btn_merge.TabIndex = 14;
            this.btn_merge.Text = "合成去背";
            this.btn_merge.UseVisualStyleBackColor = false;
            this.btn_merge.Click += new System.EventHandler(this.btn_merge_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(96, 185);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 12);
            this.label8.TabIndex = 13;
            this.label8.Text = "分割坐標：";
            // 
            // btn_cut
            // 
            this.btn_cut.BackColor = System.Drawing.Color.OliveDrab;
            this.btn_cut.Location = new System.Drawing.Point(308, 158);
            this.btn_cut.Name = "btn_cut";
            this.btn_cut.Size = new System.Drawing.Size(75, 67);
            this.btn_cut.TabIndex = 12;
            this.btn_cut.Text = "切割";
            this.btn_cut.UseVisualStyleBackColor = false;
            this.btn_cut.Click += new System.EventHandler(this.btn_cut_Click);
            // 
            // rtb_pos
            // 
            this.rtb_pos.Location = new System.Drawing.Point(167, 139);
            this.rtb_pos.Name = "rtb_pos";
            this.rtb_pos.Size = new System.Drawing.Size(120, 96);
            this.rtb_pos.TabIndex = 11;
            this.rtb_pos.Text = "2005,17,1600,800,,";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(216, 111);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 10;
            this.label7.Text = "路徑：";
            // 
            // btn_chs_rgb
            // 
            this.btn_chs_rgb.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_chs_rgb.Location = new System.Drawing.Point(263, 21);
            this.btn_chs_rgb.Name = "btn_chs_rgb";
            this.btn_chs_rgb.Size = new System.Drawing.Size(80, 67);
            this.btn_chs_rgb.TabIndex = 9;
            this.btn_chs_rgb.Text = "選擇檔案\r\n(Rgb檔)";
            this.btn_chs_rgb.UseVisualStyleBackColor = false;
            this.btn_chs_rgb.Click += new System.EventHandler(this.btn_chs_rgb_Click);
            // 
            // txt_rgb
            // 
            this.txt_rgb.Location = new System.Drawing.Point(263, 108);
            this.txt_rgb.Name = "txt_rgb";
            this.txt_rgb.Size = new System.Drawing.Size(138, 22);
            this.txt_rgb.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 111);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 7;
            this.label6.Text = "路徑：";
            // 
            // btn_chs_alp
            // 
            this.btn_chs_alp.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_chs_alp.Location = new System.Drawing.Point(53, 21);
            this.btn_chs_alp.Name = "btn_chs_alp";
            this.btn_chs_alp.Size = new System.Drawing.Size(80, 67);
            this.btn_chs_alp.TabIndex = 6;
            this.btn_chs_alp.Text = "選擇檔案\r\n(Alpha檔)";
            this.btn_chs_alp.UseVisualStyleBackColor = false;
            this.btn_chs_alp.Click += new System.EventHandler(this.btn_chs_alp_Click);
            // 
            // txt_alp
            // 
            this.txt_alp.Location = new System.Drawing.Point(53, 108);
            this.txt_alp.Name = "txt_alp";
            this.txt_alp.Size = new System.Drawing.Size(138, 22);
            this.txt_alp.TabIndex = 5;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txt_zone);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btn_logindata_en);
            this.groupBox1.Controls.Add(this.btn_data_switch);
            this.groupBox1.Controls.Add(this.btn_gamedata);
            this.groupBox1.Controls.Add(this.btn_dairylog);
            this.groupBox1.Controls.Add(this.btn_logindata);
            this.groupBox1.Location = new System.Drawing.Point(462, 235);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(261, 163);
            this.groupBox1.TabIndex = 6;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "每日更新Data";
            // 
            // txt_zone
            // 
            this.txt_zone.Location = new System.Drawing.Point(176, 119);
            this.txt_zone.Name = "txt_zone";
            this.txt_zone.Size = new System.Drawing.Size(79, 22);
            this.txt_zone.TabIndex = 29;
            this.txt_zone.Text = "0,29,30,33,34,42";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(174, 87);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(81, 24);
            this.label9.TabIndex = 28;
            this.label9.Text = "Additional Zone\r\n(Split：\',\')";
            // 
            // btn_logindata_en
            // 
            this.btn_logindata_en.BackColor = System.Drawing.Color.Tomato;
            this.btn_logindata_en.Location = new System.Drawing.Point(180, 14);
            this.btn_logindata_en.Name = "btn_logindata_en";
            this.btn_logindata_en.Size = new System.Drawing.Size(75, 67);
            this.btn_logindata_en.TabIndex = 12;
            this.btn_logindata_en.Text = "Login_en";
            this.btn_logindata_en.UseVisualStyleBackColor = false;
            this.btn_logindata_en.Click += new System.EventHandler(this.btn_logindata_en_Click);
            // 
            // btn_data_switch
            // 
            this.btn_data_switch.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.btn_data_switch.Location = new System.Drawing.Point(10, 14);
            this.btn_data_switch.Name = "btn_data_switch";
            this.btn_data_switch.Size = new System.Drawing.Size(75, 67);
            this.btn_data_switch.TabIndex = 11;
            this.btn_data_switch.Text = "開始更新";
            this.btn_data_switch.UseVisualStyleBackColor = false;
            this.btn_data_switch.Click += new System.EventHandler(this.btn_data_switch_Click);
            // 
            // btn_gamedata
            // 
            this.btn_gamedata.BackColor = System.Drawing.Color.Tomato;
            this.btn_gamedata.Location = new System.Drawing.Point(10, 87);
            this.btn_gamedata.Name = "btn_gamedata";
            this.btn_gamedata.Size = new System.Drawing.Size(75, 67);
            this.btn_gamedata.TabIndex = 9;
            this.btn_gamedata.Text = "GameData";
            this.btn_gamedata.UseVisualStyleBackColor = false;
            this.btn_gamedata.Click += new System.EventHandler(this.btn_gamedata_Click);
            // 
            // btn_dairylog
            // 
            this.btn_dairylog.BackColor = System.Drawing.Color.Tomato;
            this.btn_dairylog.Location = new System.Drawing.Point(96, 87);
            this.btn_dairylog.Name = "btn_dairylog";
            this.btn_dairylog.Size = new System.Drawing.Size(75, 67);
            this.btn_dairylog.TabIndex = 8;
            this.btn_dairylog.Text = "Dairy";
            this.btn_dairylog.UseVisualStyleBackColor = false;
            this.btn_dairylog.Click += new System.EventHandler(this.btn_dairylog_Click);
            // 
            // btn_logindata
            // 
            this.btn_logindata.BackColor = System.Drawing.Color.Tomato;
            this.btn_logindata.Location = new System.Drawing.Point(96, 14);
            this.btn_logindata.Name = "btn_logindata";
            this.btn_logindata.Size = new System.Drawing.Size(75, 67);
            this.btn_logindata.TabIndex = 7;
            this.btn_logindata.Text = "Login";
            this.btn_logindata.UseVisualStyleBackColor = false;
            this.btn_logindata.Click += new System.EventHandler(this.btn_logindata_Click);
            // 
            // tm_login_packet
            // 
            this.tm_login_packet.Interval = 1000;
            this.tm_login_packet.Tick += new System.EventHandler(this.tm_login_packet_Tick);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(731, 486);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.gb_cutter);
            this.Controls.Add(this.gb_patcher);
            this.Controls.Add(this.btn_close);
            this.Controls.Add(this.gb_locale);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrmMain";
            this.Text = "神魔字串檔轉換器";
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.gb_locale.ResumeLayout(false);
            this.gb_locale.PerformLayout();
            this.gb_patcher.ResumeLayout(false);
            this.gb_patcher.PerformLayout();
            this.gb_cutter.ResumeLayout(false);
            this.gb_cutter.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txt_path;
        private System.Windows.Forms.GroupBox gb_locale;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn_txt;
        private System.Windows.Forms.Button btn_xlsx;
        private System.Windows.Forms.Button btn_file;
        private System.Windows.Forms.Button btn_close;
        private System.Windows.Forms.GroupBox gb_patcher;
        private System.Windows.Forms.CheckBox cb_cus_patch;
        private System.Windows.Forms.CheckBox cb_cus_config;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_upd_vkey;
        private System.Windows.Forms.Button btn_dl_all;
        private System.Windows.Forms.Button btn_dl_switch;
        private System.Windows.Forms.TextBox tb_cus_patch_url;
        private System.Windows.Forms.TextBox tb_cus_config_url;
        private System.Windows.Forms.TextBox tb_versionkey;
        private System.Windows.Forms.TextBox tb_version;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox tb_minute;
        private System.Windows.Forms.Timer tm_patch;
        private System.Windows.Forms.GroupBox gb_cutter;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btn_chs_alp;
        private System.Windows.Forms.TextBox txt_alp;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btn_cut;
        private System.Windows.Forms.RichTextBox rtb_pos;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_chs_rgb;
        private System.Windows.Forms.TextBox txt_rgb;
        private System.Windows.Forms.Button btn_merge;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btn_data_switch;
        private System.Windows.Forms.Button btn_gamedata;
        private System.Windows.Forms.Button btn_dairylog;
        private System.Windows.Forms.Button btn_logindata;
        private System.Windows.Forms.Button btn_logindata_en;
        private System.Windows.Forms.Timer tm_login_packet;
        private System.Windows.Forms.CheckBox cb_100px;
        private System.Windows.Forms.TextBox txt_zone;
        private System.Windows.Forms.Label label9;
    }
}