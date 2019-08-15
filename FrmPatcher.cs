using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace LocaleParser
{
    public partial class FrmPatcher : Form
    {
        public string str_version;
        public string str_versionKey;
        public string str_config_url;
        public bool b_cus_config;
        public bool b_cus_patch;
        public bool b_auto_patching;
        private struct FLASHWINFO
        {
            public uint cbSize;

            public IntPtr hwnd;

            public uint dwFlags;

            public uint uCount;

            public uint dwTimeout;
        }

        private const uint FLASHW_ALL = 3u;

        private const uint FLASHW_TIMERNOFG = 12u;

        public const int WM_CLOSE = 16;

        [DllImport("user32.dll")]
        private static extern bool FlashWindowEx(ref FrmPatcher.FLASHWINFO fi);

        public FrmPatcher()
        {
            InitializeComponent();
        }

        private void FrmPatcher_Load(object sender, EventArgs e)
        {
            str_version = ConfigurationManager.AppSettings["version"];
            str_versionKey = ConfigurationManager.AppSettings["versionkey"];
            str_config_url = ConfigurationManager.AppSettings["configurl"];
            tb_minute.Text = "30";
            tb_version.Text = str_version;
            tb_versionkey.Text = str_versionKey;
            tb_cus_config_url.Enabled = false;
            b_cus_config = false;
            cb_cus_config.Enabled = false;
            tb_cus_patch_url.Enabled = false;
            b_cus_patch = false;
            cb_cus_patch.Enabled = false;
        }

        private void btn_upd_vkey_Click(object sender, EventArgs e)
        {
            //獲取Configuration對象
            Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            //根據Key讀取<add>元素的Value
            //string name = config.AppSettings.Settings["name"].Value;
            //寫入<add>元素的Value
            config.AppSettings.Settings["version"].Value = tb_version.Text;
            config.AppSettings.Settings["versionkey"].Value = tb_versionkey.Text;
            //增加<add>元素
            //config.AppSettings.Settings.Add("url", "http://www.fx163.net");
            //刪除<add>元素
            //config.AppSettings.Settings.Remove("name");
            //一定要記得保存，寫不帶參數的config.Save()也可以
            config.Save(ConfigurationSaveMode.Modified);
            //刷新，否則程序讀取的還是之前的值（可能已裝入內存）
            System.Configuration.ConfigurationManager.RefreshSection("appSettings");
        }

        private void btn_exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void tm_patch_Tick(object sender, EventArgs e)
        {
            Patch_Process();
        }

        private void Patch_Process()
        {
            //button1.PerformClick();
            string n_jsonStr = "";
            string o_jsonStr = "";
            string patch_url = "";
            StreamReader sr;

            string version = tb_version.Text;
            string versionKey = tb_versionkey.Text;

            string timeStamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
            string folder_name;
            
            //★★注意：此處網址是反組譯apk檔出來後得知的，他會回傳一個json檔，裡面會有一些參數，其中最重要的是patch_url_ios_list，這個參數的值是 目前所有數據包的「檔案名稱」清單 的 json檔案的網址
            Downloader.Dl_file(str_config_url + version + "-" + versionKey + "/android.json?timestamp=" + timeStamp, System.Windows.Forms.Application.StartupPath + "\\android.json");

            sr = new StreamReader("android.json", Encoding.UTF8);
            //載入JSON字串
            n_jsonStr = sr.ReadToEnd();
            sr.Close();

            JObject j_config = JsonConvert.DeserializeObject<JObject>(n_jsonStr);

            if (j_config["patch_url_android_list"].ToString() != null)
            {
                JArray array = (JArray)j_config["patch_url_android_list"];
                foreach (String obj_results in array)/*走訪JArray(results裡的每一筆JObject(這裡只有一筆)*/
                {
                    patch_url = obj_results.ToString();
                }
            }

            if (cb_cus_patch.Checked)
                patch_url = tb_cus_patch_url.Text;

            #region 下載patch
            //★★由這個patch_url_ios_list所提供的網址，可以再找到一個json檔案，它會列出目前所有數據包的「檔案名稱」清單，因此只要不斷下載這個檔案下來比對，一發現有差異時，就代表當下有更新檔
            Downloader.Dl_file(patch_url + "/patchInfo.json?t=" + timeStamp + "&version=" + version, System.Windows.Forms.Application.StartupPath + "\\n_patchInfo.json");

            sr = new StreamReader("n_patchInfo.json", Encoding.UTF8);
            /*載入JSON字串*/
            n_jsonStr = sr.ReadToEnd();
            sr.Close();

            if (File.Exists("o_patchInfo.json"))
            {
                sr = new StreamReader("o_patchInfo.json", Encoding.UTF8);
                /*載入JSON字串*/
                o_jsonStr = sr.ReadToEnd();
                sr.Close();
            }
            else
                o_jsonStr = "{\"currentVersion\": 38,\"items\": []}";
            JObject j_n_patchInfo = JsonConvert.DeserializeObject<JObject>(n_jsonStr);
            JObject j_o_patchInfo = JsonConvert.DeserializeObject<JObject>(o_jsonStr);

            n_jsonStr = JsonConvert.SerializeObject(j_n_patchInfo, Formatting.Indented);
            o_jsonStr = JsonConvert.SerializeObject(j_o_patchInfo, Formatting.Indented);

            bool b_patch = !(n_jsonStr.Equals(o_jsonStr));
            if (b_patch)
            {
                List<UnityFile> ufl = GetPatchlist(j_n_patchInfo, j_o_patchInfo);
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\PatchFile\\" + timeStamp + "\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\PatchFile\\" + timeStamp + "\\");
                }
                foreach (UnityFile uf in ufl)
                {
                    Downloader.Dl_file(patch_url + "/" + uf.name + ".unity3d", System.Windows.Forms.Application.StartupPath + "\\PatchFile\\" + timeStamp + "\\" + uf.name + ".unity3d");
                }
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\PatchHis\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\PatchHis\\");
                }
                //檔案已存在的FileMove
                File.Copy("n_patchInfo.json", System.Windows.Forms.Application.StartupPath + "\\PatchHis\\" + timeStamp + "_patchInfo.json");
                File.Delete("o_patchInfo.json");
                File.Move("n_patchInfo.json","o_patchInfo.json");
            }
            #endregion
            #region 下載dlc
            Downloader.Dl_file(patch_url + "/dlc" + "/info.json?t=" + timeStamp + "&version=" + version, System.Windows.Forms.Application.StartupPath + "\\n_info.json");
            sr = new StreamReader("n_info.json", Encoding.UTF8);
            /*載入JSON字串*/
            n_jsonStr = sr.ReadToEnd();
            sr.Close();

            if (File.Exists("o_info.json"))
            {
                sr = new StreamReader("o_info.json", Encoding.UTF8);
                /*載入JSON字串*/
                o_jsonStr = sr.ReadToEnd();
                sr.Close();
            }
            else
                o_jsonStr = "{}";
            JObject j_n_info = JsonConvert.DeserializeObject<JObject>(n_jsonStr);
            JObject j_o_info = JsonConvert.DeserializeObject<JObject>(o_jsonStr);

            n_jsonStr = JsonConvert.SerializeObject(j_n_info, Formatting.Indented);
            o_jsonStr = JsonConvert.SerializeObject(j_o_info, Formatting.Indented);

            bool b_info = !(n_jsonStr.Equals(o_jsonStr));
            if (b_info)
            {
                List<UnityFile> ufl = GetDlclist(j_n_info, j_o_info);
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\PatchFile\\" + timeStamp + "\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\PatchFile\\" + timeStamp + "\\");
                }
                foreach (UnityFile uf in ufl)
                {
                    Downloader.Dl_file(patch_url + "/dlc/" + uf.name + ".unity3d", System.Windows.Forms.Application.StartupPath + "\\PatchFile\\" + timeStamp + "\\" + uf.name + ".unity3d");
                }
                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\PatchHis\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\PatchHis\\");
                }
                //檔案已存在的FileMove
                File.Copy("n_info.json", System.Windows.Forms.Application.StartupPath + "\\PatchHis\\" + timeStamp + "_info.json");
                File.Move("n_info.json", System.Windows.Forms.Application.StartupPath + "\\" + "o_info.json");
            }
            #endregion

            if(b_patch || b_info)
            {
                //如果有更新檔時，工作列會跳通知
                FrmPatcher.FLASHWINFO fLASHWINFO = default(FrmPatcher.FLASHWINFO);
                fLASHWINFO.cbSize = Convert.ToUInt32(Marshal.SizeOf(fLASHWINFO));
                fLASHWINFO.hwnd = base.Handle;
                fLASHWINFO.dwFlags = 15u;
                fLASHWINFO.uCount = 4294967295u;
                fLASHWINFO.dwTimeout = 0u;
                FrmPatcher.FlashWindowEx(ref fLASHWINFO);
            }
        }

        private void btn_dl_switch_Click(object sender, EventArgs e)
        {
            if (b_auto_patching)
            {
                //txt_status.Text += "Stop auto patching. At:" + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                ((System.Windows.Forms.Button)sender).ForeColor = System.Drawing.Color.Black;
            }
            else
            {
                //txt_status.Text += "Start auto patching. At:" + DateTime.Now.ToString("HH:mm:ss.fff") + "\r\n";
                ((System.Windows.Forms.Button)sender).ForeColor = System.Drawing.Color.Red;
                Patch_Process();
            }

            //以分鐘為單位
            tm_patch.Interval = Int32.Parse(tb_minute.Text) * 60000;
            tm_patch.Enabled = (!b_auto_patching);
            b_auto_patching = (!b_auto_patching);
        }

        public List<UnityFile> GetPatchlist(JObject n_jo, JObject o_jo)
        {
            List<UnityFile> ufl = new List<UnityFile>();
            UnityFile uf;
            bool b_newFile;

            foreach (JObject tmp_n_jo in n_jo["items"])
            {
                b_newFile = true;
                foreach (JObject tmp_o_jo in o_jo["items"])
                {
                    if((tmp_n_jo["name"].ToString() == tmp_o_jo["name"].ToString()) && (tmp_n_jo["hash"].ToString() == tmp_o_jo["hash"].ToString()))
                    {
                        b_newFile = false;
                    }
                }

                if(b_newFile)
                {
                    uf = new UnityFile();
                    uf.name = tmp_n_jo["name"].ToString();
                    uf.hash = tmp_n_jo["hash"].ToString();
                    ufl.Add(uf);                    
                }
            }

            return ufl;
        }

        public List<UnityFile> GetDlclist(JObject n_jo, JObject o_jo)
        {
            List<UnityFile> ufl = new List<UnityFile>();
            UnityFile uf;
            bool b_newFile;

            foreach (JProperty tmp_n_jp in n_jo.Properties())
            {
                b_newFile = true;
                foreach (JProperty tmp_o_jp in o_jo.Properties())
                {
                    if ((tmp_n_jp.Name == tmp_o_jp.Name) && (tmp_n_jp.Value == tmp_o_jp.Value))
                    {
                        b_newFile = false;
                    }
                }

                if (b_newFile)
                {
                    uf = new UnityFile();
                    uf.name = tmp_n_jp.Name.ToString();
                    uf.hash = tmp_n_jp.Value.ToString();
                    ufl.Add(uf);
                }
            }

            return ufl;
        }

        private void tb_minute_KeyPress(object sender, KeyPressEventArgs e)
        {
            //用來限制，分鐘數的欄位只能輸入數字而已
            if (e.KeyChar == (Char)48 || e.KeyChar == (Char)49 ||
               e.KeyChar == (Char)50 || e.KeyChar == (Char)51 ||
               e.KeyChar == (Char)52 || e.KeyChar == (Char)53 ||
               e.KeyChar == (Char)54 || e.KeyChar == (Char)55 ||
               e.KeyChar == (Char)56 || e.KeyChar == (Char)57 ||
               e.KeyChar == (Char)13 || e.KeyChar == (Char)8)
            {
                e.Handled = false;
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
