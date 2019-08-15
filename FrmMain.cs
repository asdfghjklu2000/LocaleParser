using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;

using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml.Serialization;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;
using System.Net;
using System.Xml;

namespace LocaleParser
{
    public partial class FrmMain : Form
    {
        protected string fileName = null; //文檔名
        protected IWorkbook workbook = null;

        public string str_version;
        public string str_versionKey;
        public string str_config_url;
        public bool b_cus_config;
        public bool b_cus_patch;
        public bool b_auto_patching;
        public bool b_auto_login;
        public Dictionary<string, string> dict_locale_csv = new Dictionary<string, string>();
        public Dictionary<string, string> dict_locale_csv_eng = new Dictionary<string, string>();
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
        private static extern bool FlashWindowEx(ref FrmMain.FLASHWINFO fi);

        protected DataTable TxtConvertToDataTable(string File, string TableName, string delimiter)
        {
            DataTable dt = new DataTable();
            DataSet ds = new DataSet();
            StreamReader s = new StreamReader(File, System.Text.Encoding.UTF8);
            //string ss = s.ReadLine();//skip the first line
            string[] columns = s.ReadLine().Split(delimiter.ToCharArray());
            ds.Tables.Add(TableName);
            foreach (string col in columns)
            {
                bool added = false;
                string next = "";
                int i = 0;
                while (!added)
                {
                    string columnname = col + next;
                    columnname = columnname.Replace("#", "");
                    columnname = columnname.Replace("'", "");
                    columnname = columnname.Replace("&", "");

                    if (!ds.Tables[TableName].Columns.Contains(columnname))
                    {
                        ds.Tables[TableName].Columns.Add(columnname.ToUpper());
                        added = true;
                    }
                    else
                    {
                        i++;
                        next = "_" + i.ToString();
                    }
                }
            }

            string AllData = s.ReadToEnd();
            string[] rows = AllData.Split("\n".ToCharArray());

            foreach (string r in rows)
            {
                if (r.Trim() != "")
                {
                    string[] items = r.Split(delimiter.ToCharArray());
                    ds.Tables[TableName].Rows.Add(items);
                }
            }

            s.Close();
            s.Dispose();

            dt = ds.Tables[0];

            return dt;
        }

        public FrmMain()
        {
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            txt_path.Text = System.Windows.Forms.Application.StartupPath + "\\locale.csv.txt";
            txt_alp.Text = System.Windows.Forms.Application.StartupPath + "\\alpha8.png";
            txt_rgb.Text = System.Windows.Forms.Application.StartupPath + "\\rgb.png";

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
            b_auto_patching = false;
            b_auto_login = false;
            loadLocaleCsv();
        }

        public byte[] ReadFile(string filePath)
        {
            byte[] buffer;
            FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            try
            {
                int length = (int)fileStream.Length;  // get file length
                buffer = new byte[length];            // create buffer
                int count;                            // actual number of bytes read
                int sum = 0;                          // total number of bytes read

                // read until Read method returns 0 (end of the stream has been reached)
                while ((count = fileStream.Read(buffer, sum, length - sum)) > 0)
                    sum += count;  // sum is a buffer offset for next reading
            }
            finally
            {
                fileStream.Close();
            }
            return buffer;
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

            n_jsonStr = JsonConvert.SerializeObject(j_n_patchInfo, Newtonsoft.Json.Formatting.Indented);
            o_jsonStr = JsonConvert.SerializeObject(j_o_patchInfo, Newtonsoft.Json.Formatting.Indented);

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
                File.Move("n_patchInfo.json", "o_patchInfo.json");
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

            n_jsonStr = JsonConvert.SerializeObject(j_n_info, Newtonsoft.Json.Formatting.Indented);
            o_jsonStr = JsonConvert.SerializeObject(j_o_info, Newtonsoft.Json.Formatting.Indented);

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
                File.Delete("o_info.json");
                File.Move("n_info.json", System.Windows.Forms.Application.StartupPath + "\\" + "o_info.json");
            }
            #endregion

            if (b_patch || b_info)
            {
                //如果有更新檔時，工作列會跳通知
                FrmMain.FLASHWINFO fLASHWINFO = default(FrmMain.FLASHWINFO);
                fLASHWINFO.cbSize = Convert.ToUInt32(Marshal.SizeOf(fLASHWINFO));
                fLASHWINFO.hwnd = base.Handle;
                fLASHWINFO.dwFlags = 15u;
                fLASHWINFO.uCount = 4294967295u;
                fLASHWINFO.dwTimeout = 0u;
                FrmMain.FlashWindowEx(ref fLASHWINFO);
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
                    if ((tmp_n_jo["name"].ToString() == tmp_o_jo["name"].ToString()) && (tmp_n_jo["hash"].ToString() == tmp_o_jo["hash"].ToString()))
                    {
                        b_newFile = false;
                    }
                }

                if (b_newFile)
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

        private void btn_file_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select file";
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(txt_path.Text);
            dialog.Filter = "txt files (*.*)|*.txt";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txt_path.Text = dialog.FileName;
            }
        }

        private void btn_xlsx_Click(object sender, EventArgs e)
        {
            Boolean isTxt = false;
            if (((System.Windows.Forms.Button)sender).Name.ToString() == "btn_txt")
                isTxt = true;
            if(File.Exists(txt_path.Text))
            {
                byte[] newBytes;
                newBytes = ReadFile(txt_path.Text);
                //MCSVFile m = MCSVFile.LoadByte(newBytes);
                
                try {
                    MH.MCSVFile m = new MH.MCSVFile();
                    m.LoadBinaryAsset(newBytes);
                    //Tosapp_Tool.MCSVFile m = MCSVFile.LoadStream(file);
                    Dictionary<string, string> TipsManager = new Dictionary<string, string>();

                    //m.NumRow()為所有字串的總數
                    int num = m.NumRow();
                    //return num.ToString();

                    //用來接
                    string text = string.Empty;
                    string text1 = string.Empty;
                    string text2 = string.Empty;
                    
                    DataTable result = new DataTable("Locale");

                    result.Columns.Add("編號");
                    result.Columns.Add("中文");
                    result.Columns.Add("英文");

                    //如果是txt
                    string path = "";
                    path = Path.GetDirectoryName(txt_path.Text) + "\\" + Path.GetFileNameWithoutExtension(txt_path.Text) + "_" + getTimestamp() + ".txt"; ;                    

                    if(isTxt)
                    {
                        FileStream fs = new FileStream(path, System.IO.FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
                        StreamWriter sw = new StreamWriter(fs);
                        for (int i = 0; i < num - 1; i++)
                        {
                            text = string.Empty;
                            text1 = string.Empty;
                            text2 = string.Empty;
                            if (!string.IsNullOrEmpty(m.GetString(i, 0, string.Empty)))
                            {
                                //m.GetString(i, 0, string.Empty)是字串的KEY值
                                text = m.GetString(i, 0, string.Empty);
                                //m.GetString(i, 1, string.Empty)是字串的英文描述
                                text1 = m.GetString(i, 1, string.Empty);
                                //m.GetString(i, 2, string.Empty)是字串的中文描述
                                text2 = m.GetString(i, 2, string.Empty);

                                var row = result.NewRow();
                                row["編號"] = text;
                                row["中文"] = text2;
                                row["英文"] = text1;
                                result.Rows.Add(row);
                                sw.WriteLine("<<<<" + text + ">>>>");
                                sw.WriteLine(text2);
                                sw.WriteLine(text1);
                            }
                        }
                        sw.Close();
                        sw.Dispose();
                        fs.Dispose();
                    }
                    else
                    {
                        for (int i = 0; i < num - 1; i++)
                        {
                            text = string.Empty;
                            text1 = string.Empty;
                            text2 = string.Empty;
                            if (!string.IsNullOrEmpty(m.GetString(i, 0, string.Empty)))
                            {
                                //m.GetString(i, 0, string.Empty)是字串的KEY值
                                text = m.GetString(i, 0, string.Empty);
                                //m.GetString(i, 1, string.Empty)是字串的英文描述
                                text1 = m.GetString(i, 1, string.Empty);
                                //m.GetString(i, 2, string.Empty)是字串的中文描述
                                text2 = m.GetString(i, 2, string.Empty);

                                var row = result.NewRow();
                                row["編號"] = text;
                                row["中文"] = text2;
                                row["英文"] = text1;
                                result.Rows.Add(row);                                
                            }
                        }
                    }

                    if (!isTxt)
                    {
                        int res = 0;
                        res = DataTableToExcel(result, "Locale", true);
                        if (res < 0)
                            System.Windows.Forms.MessageBox.Show("Make xlsx Error!");
                    }
                }
                catch (Exception var_8_9E)
                {
                    System.Windows.Forms.MessageBox.Show("Translation error: " + var_8_9E.ToString());
                }
            }
        }

        public int DataTableToExcel(DataTable data, string sheetName, bool isColumnWritten)
        {
            fileName = Path.GetDirectoryName(txt_path.Text) + "\\" + Path.GetFileNameWithoutExtension(txt_path.Text) + "_" + getTimestamp() + ".xlsx"; //Path.GetTempFileName();
            int i = 0;
            int j = 0;
            int count = 0;
            ISheet sheet = null;

            FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
            //if (fileName.IndexOf(".xlsx") > 0) // 2007版本
            workbook = new XSSFWorkbook();
            //else if (fileName.IndexOf(".xls") > 0) // 2003版本
            //    workbook = new HSSFWorkbook();

            try
            {
                if (workbook != null)
                {
                    sheet = workbook.CreateSheet(sheetName);
                }
                else
                {
                    return -1;
                }

                if (isColumnWritten == true) //寫入DataTable的列名
                {
                    IRow row = sheet.CreateRow(0);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Columns[j].ColumnName);
                    }
                    count = 1;
                }
                else
                {
                    count = 0;
                }

                for (i = 0; i < data.Rows.Count; ++i)
                {
                    IRow row = sheet.CreateRow(count);
                    for (j = 0; j < data.Columns.Count; ++j)
                    {
                        row.CreateCell(j).SetCellValue(data.Rows[i][j].ToString());
                    }
                    ++count;
                }
                workbook.Write(fs); //寫入到excel
                fs.Close();
                fs.Dispose();
                fs = null;

                showFile(fileName);

                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                return -1;
            }
        }

        private int getTimestamp() { return (int)((DateTime.Now.AddHours(-8) - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds); }

        private void btn_close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected void showFile(string filepath)
        {
            string file = @"C:\Windows\explorer.exe";
            string argument = @"/select, " + filepath;
            System.Diagnostics.Process.Start(file, argument);
        }

        //unsafe static void bitttt()
        //{
        //    Bitmap mask = new Bitmap(image1, image3.Width, image3.Height);
        //    Bitmap input = new Bitmap(@"RGB4.png", true);

        //    Bitmap output = new Bitmap(input.Width, input.Height, PixelFormat.Format32bppArgb);
        //    var rect = new Rectangle(0, 0, input.Width, input.Height);
        //    var bitsMask = mask.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //    var bitsInput = input.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        //    var bitsOutput = output.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
        //    unsafe
        //    {
        //        for (int y = 0; y < input.Height; y++)
        //        {
        //            byte* ptrMask = (byte*)bitsMask.Scan0 + y * bitsMask.Stride;
        //            byte* ptrInput = (byte*)bitsInput.Scan0 + y * bitsInput.Stride;
        //            byte* ptrOutput = (byte*)bitsOutput.Scan0 + y * bitsOutput.Stride;
        //            for (int x = 0; x < input.Width; x++)
        //            {
        //                ptrOutput[4 * x] = ptrInput[4 * x];           // blue
        //                ptrOutput[4 * x + 1] = ptrInput[4 * x + 1];   // green
        //                ptrOutput[4 * x + 2] = ptrInput[4 * x + 2];   // red
        //                ptrOutput[4 * x + 3] = ptrMask[4 * x];        // alpha
        //            }
        //        }
        //    }
        //    mask.UnlockBits(bitsMask);
        //    input.UnlockBits(bitsInput);
        //    output.UnlockBits(bitsOutput);

        //    output.Save(...);
        //}

        private void button1_Click(object sender, EventArgs e)  	//打开方法
        {
            try
            {
                // Retrieve the image.
                Bitmap image1 = new Bitmap(@"Alpha8.png", true);
                Bitmap image3 = new Bitmap(@"RGB4.png", true);
                Bitmap image2 = new Bitmap(image1, image3.Width, image3.Height);

                int x, y;

                // Loop through the images pixels to reset color.
                for (x = 0; x < image2.Width; x++)
                {
                    for (y = 0; y < image2.Height; y++)
                    {
                        Color pixelColor2 = image2.GetPixel(x, y);
                        Color pixelColor3 = image3.GetPixel(x, y);
                        Color newColor = Color.FromArgb(pixelColor2.A, pixelColor3.R, pixelColor3.G, pixelColor3.B);
                        image3.SetPixel(x, y, newColor);
                    }
                }
                image3.Save("output.png", System.Drawing.Imaging.ImageFormat.Png);

            }
            catch (ArgumentException)
            {
                System.Windows.Forms.MessageBox.Show("There was an error." + "Check the path to the image file.");
            }
        }

        private void btn_chs_alp_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select file";
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(txt_alp.Text);
            dialog.Filter = "png files (*.*)|*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txt_alp.Text = dialog.FileName;
            }
        }

        private void btn_chs_rgb_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select file";
            dialog.InitialDirectory = System.IO.Path.GetDirectoryName(txt_rgb.Text);
            dialog.Filter = "png files (*.*)|*.png";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                txt_rgb.Text = dialog.FileName;
            }
        }

        private void btn_merge_Click(object sender, EventArgs e)
        {
            try
            {
                if(File.Exists(System.Windows.Forms.Application.StartupPath + "\\merge.png"))
                    File.Delete(System.Windows.Forms.Application.StartupPath + "\\merge.png");

                // Retrieve the image.
                Bitmap img_alp_tmp = new Bitmap(txt_alp.Text, false);
                Bitmap img_rgb = new Bitmap(txt_rgb.Text, false);
                Bitmap img_alp = new Bitmap(img_alp_tmp, img_rgb.Width, img_rgb.Height);

                int x, y;

                // Loop through the images pixels to reset color.
                for (x = 0; x < img_rgb.Width; x++)
                {
                    for (y = 0; y < img_rgb.Height; y++)
                    {
                        Color c_rgb = img_rgb.GetPixel(x, y);
                        Color c_alp = img_alp.GetPixel(x, y);
                        Color newColor = Color.FromArgb(c_alp.A, c_rgb.R, c_rgb.G, c_rgb.B);
                        img_rgb.SetPixel(x, y, newColor);
                    }
                }
                
                img_rgb.Save(System.Windows.Forms.Application.StartupPath + "\\merge.png", System.Drawing.Imaging.ImageFormat.Png);
                img_alp_tmp.Dispose();
                img_rgb.Dispose();
                img_alp.Dispose();
                showFile(System.Windows.Forms.Application.StartupPath + "\\merge.png");
            }
            catch (ArgumentException)
            {
                System.Windows.Forms.MessageBox.Show("There was an error." + "Check the path to the image file.");
            }
        }

        /// <summary>
        /// 图像水平翻转
        /// </summary>
        /// <param name="bmp">原来图像</param>
        /// <returns></returns>
        public static Bitmap HorizontalFlip(Bitmap bmp)
        {
            try
            {
                var width = bmp.Width;
                var height = bmp.Height;
                Graphics g = Graphics.FromImage(bmp);
                Rectangle rect = new Rectangle(0, 0, width, height);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipX);
                g.DrawImage(bmp, rect);
                return bmp;
            }
            catch (Exception ex)
            {
                return bmp;
            }

        }


        /// <summary>
        /// 图像垂直翻转
        /// </summary>
        /// <param name="bit">原来图像</param>
        /// <returns></returns>
        public static Bitmap VerticalFlip(Bitmap bmp)
        {
            try
            {
                var width = bmp.Width;
                var height = bmp.Height;
                Graphics g = Graphics.FromImage(bmp);
                Rectangle rect = new Rectangle(0, 0, width, height);
                bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
                g.DrawImage(bmp, rect);
                return bmp;
            }
            catch (Exception ex)
            {
                return bmp;
            }
        }

        private void btn_cut_Click(object sender, EventArgs e)
        {
            if (!File.Exists(System.Windows.Forms.Application.StartupPath + "\\merge.png"))
                System.Windows.Forms.MessageBox.Show("Please Merge First.");
            else
            {
                
                Bitmap img_src = new Bitmap(System.Windows.Forms.Application.StartupPath + "\\merge.png",true);
                Bitmap img_flip = VerticalFlip(img_src);
                string timeStamp = ((int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds).ToString();
                //來源圖片，大小512 x 512
                int picWidth = 160;
                int picHeight = 160;
                string[] str_ary_split;
                string cut_name, src_name, pos_x, pos_y, unk_1, unk_2;
                Bitmap img_cut, img_frame;
                Graphics gpc;

                if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\CutPng\\" + timeStamp + "\\"))
                {
                    Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\CutPng\\" + timeStamp + "\\");
                }

                foreach (string line in rtb_pos.Lines)
                {
                    str_ary_split = line.Split(',');
                    cut_name = str_ary_split[0];

                    src_name = str_ary_split[1];
                    pos_x = str_ary_split[2];
                    pos_y = str_ary_split[3];
                    unk_1 = str_ary_split[4];
                    unk_2 = str_ary_split[5];

                    img_cut = new Bitmap(picWidth, picHeight, PixelFormat.Format32bppArgb);
                    img_cut.SetResolution(72.0f, 72.0f);
                    gpc = Graphics.FromImage(img_cut);
                    //建立畫板
                    gpc.DrawImage(img_flip,
                             //將被切割的圖片畫在新圖片上面，第一個參數是被切割的原圖片
                             new Rectangle(0, 0, picWidth, picHeight),
                             //指定繪製影像的位置和大小，基本上是同pic大小
                             new Rectangle(Int32.Parse(pos_x), Int32.Parse(pos_y), picWidth, picHeight),
                             //指定被切割的圖片要繪製的部分
                             GraphicsUnit.Pixel);
                    //測量單位，這邊是pixel
                    VerticalFlip(img_cut);

                    string frame_code = "", frame_xml = "";
                    frame_xml = post("http://tosapphou.somee.com/tosapphou_ws.asmx/GetAttribute", "MonsterID=" + cut_name);
                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(frame_xml);//xmlstring 是傳入 XML 格式的 string     
                    frame_code = doc.GetElementsByTagName("string")[0]?.InnerText;

                    if (frame_code == "_")
                    {
                        using (var form = new FrmFrameChooser(img_cut))
                        {
                            var result = form.ShowDialog();

                            //if (result == DialogResult.OK)
                            //{
                            string val = form.ReturnValue1;

                            if (val != "HowDoYouTurnThisOn")
                                frame_code = val;
                        }
                    }

                    if (frame_code != "_")
                        img_frame = new Bitmap(System.Windows.Forms.Application.StartupPath + "\\Frame\\" + frame_code + ".png", true);
                    else
                        img_frame = new Bitmap(160, 160);

                    if (frame_code != "_")
                    {
                        gpc.DrawImage(img_frame,
                             //將被切割的圖片畫在新圖片上面，第一個參數是被切割的原圖片
                             new Rectangle(0, 0, picWidth, picHeight),
                             //指定繪製影像的位置和大小，基本上是同pic大小
                             new Rectangle(0, 0, picWidth, picHeight),
                             //指定被切割的圖片要繪製的部分
                             GraphicsUnit.Pixel);
                    }
                    if(cb_100px.Checked)
                    {
                        Bitmap img_100px = new Bitmap(img_cut, 100, 100);
                        img_100px.SetResolution(72.0f, 72.0f);
                        img_100px.Save(System.Windows.Forms.Application.StartupPath + "\\CutPng\\" + timeStamp + "\\" + cut_name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                        img_100px.Dispose();
                    }
                    else
                        img_cut.Save(System.Windows.Forms.Application.StartupPath + "\\CutPng\\" + timeStamp + "\\" + cut_name + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    gpc.Dispose();
                    img_cut.Dispose();
                }
                img_flip.Dispose();
                img_src.Dispose();
                showFile(System.Windows.Forms.Application.StartupPath + "\\CutPng\\" + timeStamp + "\\");
            }
        }

        private string post(string url, string param)
        {
            //url = "https://zh.towerofsaviors.com/api/user/gameData?loginToken=e900e589e87e9be2a6dda26b64783bb8&apKey=872518590&A3=d41d8cd9&idfv=&mac=06%3abc%3a27%3a5f%3a7e%3ae6&geoData=0%2c0&uid=402873355&session=6362a1de107c4e9260c3b01cf0d84a4d&versionKey=ulYzGFokOZWQJBj6&language=zh_TW&platform=android&version=17.03&timestamp=1552040541&timezone=8&external=True&revision=459&iay=eb016fa&hash=0148237c94ee468326d6191ef7f9e369";
            //param = "head=3%2fJWw12BCSnYKjIupB8iGAC3PKLSwsuX7etsWzFhtnW8MVrDazkIT0rLEJ1TtnzrWTt5VcxhsM8JNEFqGx9ry5BVFuyWbwcXCfxFyZ3VOUR5bk%2bOIemQRkeB0yU8gxU8tJ09mLfj%2f8ShnQAr0jaTiuc%2fjTKqcFs%2bcn8i1VFhQBGO7E%2bSLFsTPmSvHvjiYfJzBWe%2fgvGnTV%2bY4bHKmax72%2bvqngr5eu0WUW2BA0ZeAMK1x5R9YrPR0%2f6WDD2Mz%2b%2f1I8dDfehcy7NmBvRoOWsLql9MKac17lkWL7gK81aKC%2fTo4iva%2f9gHG3P%2fJMs%2fDhoSWJKwfjZw6LuLw6Kf%2bMQIYRt%2b5Q82wFVMA2ViMH9WGAxI7K%2fZtWxib0DMwuvD8DPaNr1AKvTKf2Zy5AMRiY7fOgcw%2b120Kr%2bm2acCEUnNd9SHLdUxObzQOyz659W6v1WMipeMkYztMt69TG1sdx7NPa0SUxI%2b8Z%2fxJMuMwe3EkJXSxTF%2bnf0xKWPHHwIBY5C%2b8z%2b7raEP55ufN7Lt94gsE7arE7wjlf0vmQQ5v6oJ%2faazzSeHcUsf9RDjThoqxjbcbkuC6CIEp8sA9EhvJ1Dx51AgL3VmzpFaCYUvL0CS11LDJGbRVuEo%2fa3x%2bLZFK8r%2ftnoUKo%2fllaeAJAwfl5UGDI8vfXvlWyQiZtav1zI1p2bXTJBFzckpsHxAMXNUunrX7LQB1r4ADFqMHo1S%2bwm84b2wVcSU3hmYK9vi4m500T%2fOctYgJrMwEzs0VeCeONbCujHw6F2cR41kf75%2fbCeK%2bbtDBcaIRBkyTXIz9sptIUX5pS9HmSJxKLkfYvrX%2bkAG%2fdBaf3irThFkIIBEx2kDClGKJofCsMBa6k4Fm4G72WCYyK%2bWKaboO5jO67sQPHDM4NMmXTNgAwc%2bzhpukqt1D4VXGqcfNdX1rIpSHgtkMB%2fm8fUWk78YOjzSVSand3tEaSG03%2b9ghbHue3bfmvLq7DAWDRk47VzKOO0%2f%2fdLiS81dRHKP13arus5HJ%2f4qNg0%2f2biU%2b8FJwzWdk1CbNk2PZntiCDQY38u5Q7XOXEQGmf9EDFC82IqE05oX0h2%2boE%2bk%2fIt%2fdYrBr2XJjjg8HLDQejf3il24jDWYcIK8yBmO2t20a9a1oRMCcBB8BeZ8I11rxdS8UthDb9dcrXCYNJo4GhowbJTk%2buckC5MAubhLHGrgVwjrWXa4EcUghQ%2bOLTRPyrmtE8hTjhLI1B7yGrELkVlkGI7gmVM09dliT%2ffzc012gApSo%2fCf4EsFxfPZfouPVZhlV8Bk07h9BnL6gYQHXdg1tKPTRsOII%2bM9LZS6C6kY72LyPStRPZvbys3GeX1tEe62f1n5lecwMAhtGe73tQ62iV3FXsG%2fk0xLpDevBAcJJeJIsZl2AdNK7KNjElElN0pGQqVCE%2fapMSiRtpw3Zk5%2b7ZvLfELzmdfwdElWDJNb9ct672k9TXg3fR96bZibWq5wCLuUxZgbEhPRpeIsUS0uhSXd0clU6HTt3IfKRrUTMCGu4npcFXffgFGTBtWLMiMNaVsmYwz5VXZAw0KsToFXjN9G%2f2azMn%2bKMq4y4aajrbWQbCiTiItOa1NCBGkx8JROX7gnVfs%3d";
            byte[] bytes = Encoding.UTF8.GetBytes(param);
            HttpWebRequest httpWebRequest = WebRequest.Create(url) as HttpWebRequest;
            httpWebRequest.Method = "POST";
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Timeout = 30000;
            httpWebRequest.ContentLength = (long)bytes.Length;
            httpWebRequest.Host = httpWebRequest.RequestUri.Host; //"zh.towerofsaviors.com";
            httpWebRequest.UserAgent = "Dalvik/2.1.0 (Linux; U; Android 5.1.1; SM-G930F Build/LMY48Z)";
            httpWebRequest.Accept = "*/*";
            using (Stream requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }
            string result = "";
            using (HttpWebResponse httpWebResponse = httpWebRequest.GetResponse() as HttpWebResponse)
            {
                using (StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
                {
                    result = streamReader.ReadToEnd();
                }
            }
            return result;
        }

        private void btn_logindata_Click(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Json\\");
            }

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\login.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\login.txt");

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_login.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_login.txt");

            StreamWriter str = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\login.txt");
           


            string url = "https://zh.towerofsaviors.com/api/user/login?type=facebook&uniqueKey=14d8a07368c350485a658939e2fe2e32&deviceKey=9198d459a03488631865ec43c380d8a3&sysInfo=Android+OS+5.1.1+%2f+API-22+(LMY48Z%2feng.se.infra.20180530.161229)%7cARMv7+VFPv3+NEON+VMH%7c0%7c2022%7c512%7cAdreno+(TM)+540%7cFALSE%7cOpenGL+ES+2.0+build+1.3%402768895%7cFull%7cFALSE%7c16384%7c17.43%7c%7c%7cca%3a87%3a2d%3a64%3a4f%3ac0&session=&loginToken=200b47691b6582e37e1f0affc7659bfb&apKey=434267742&A1=d41d8cd9&A2=d41d8cd9&A3=d41d8cd9&A4=d41d8cd9&A5=d41d8cd9&A6=d41d8cd9&A7=d41d8cd9&A8=d41d8cd9&A9=d41d8cd9&A10=d41d8cd9&A11=d41d8cd9&A13=d41d8cd9&A12=d41d8cd9&idfv=&mac=ca%3a87%3a2d%3a64%3a4f%3ac0&geoData=0%2c0&versionKey=ltKWB4WRrG3hvBre&language=zh_TW&platform=android&version=17.43&timestamp=1565596485&timezone=13&tosLang=zh&revision=1048&iay=f146072&hash=12e9833bf4ea274fcfb4b2fd36b13ba7";
            string param = "head=3%2fJWw12BCSnYKjIupB8iGFHL9ZK%2fCJEn5CynIEisceTZtTPTsHQ7pWfoNnVYNyWF%2bXLPafBHFnCHK0m6wdIA7Ht0agNrz6NgMnK5y5PYTc4oVP1olBGV3sWaVoH5jTQIAhveFIuqhSR4HwoYzRFHEbMrYJedSbfd6A4SjQkfDuUVeGa0rATd9EhM%2fzPswa1bA6N71qeJvJfKH1%2f4XDhhweuFZlaz%2b5HyxVYyhrKhY3HX5rN5%2fNidJggP4mIX6cE39NFupKTv0yZYFUVs4QexK5jjjwnURcp3J5Mfxc35NTpAld4QLbSsscn%2fvMVb1FpAPC3pBA%2fqogjAjmhOkAmO99jIO2nDfoZkql9yHxbAukT4JuhMak86KhBr79%2f9XTWNmzQ7jIcYXNlb2rWbLEoUF3X3KubNZT3mhOTPNY%2bHIoi4BGR2dCrFyJxbJ5ddx5GHCAlpdMEUyHaiK7pMVhN%2b4xA9v1ugXY%2bI2CVYSomY%2byy6ax6IRCIbOe%2fRDTpVOfXzfy5FG7LLAygGaqhwhBh7lNFVNuf5rR7UKOaf%2bIm4NSlLPYeg5LvUncN%2bcZzm%2f30kClM9qUowzZogOvG1F5HFwD4UVuchrFLo2trpJYdxTI5LvziOUmmZ7GNNlUXZdEeAkM6%2bKXMiS%2fulKO9uLv9CLPIZnIXJcW%2bxKFEyJ30g9rH8QfqmJ9f1vFBEn%2f6CjN%2bzXHe%2f3hMketlsXfmV4ytwLptKIKdOeJ1Y5F32L73tQNSdz%2fH8ZZWJVqog34pRRGyckksISi0oDzORe1zdt1FLyaV3w%2bN%2bovS3%2fr%2bKSPFBVhQMBAXn%2fFICvwoRsBWdOjDwRK7ZAkeohnmzx49%2bWvrjAyvcwOL3hfWA8q1ltCH3hNr3E6Sl%2fRxO6GOUOkO85yk5KK6BSkOfxshOJDsTMiS%2bQYmV6d7aI9Wr4ORjHofy8Qb10WqLhu5aizJx5Ls0mks2ANUzfYyBgEAYXCI1j8oUqSCGq2BEKH8Dxj003a5PV4FGvBR3kuc9mmvTUsasYz4lp72XUhXUxioymsMyNlIrsnYLlzIR%2bH0MxjU4QINuthhtx%2bmR4rJpA3lv%2bYYp4BjS5vmjLbrOWDNuRYMBLBKhngv4%2bzHkPwPrtYNykM6ScnEfHz4DEVTiATjIZUJdYlPAztQrYuw13dS3clod8wp3ewIlrN1DG6%2fnRnN1H3j2H2s3qNTYi%2bv6R4oZd9ezw3Ysd99TH2BEtrDpQnbZmpHT%2bV7kQDOHXz9lVsNo8%2bjo745VjZ8NRzfxKVvaLWWUnw324hRU24WxdYwhvLPczpTFAyqGJylm1UgB87mar32OdpmXDNnE9T6omCioeCdU6OSwAxlf1XH2Gbyavu4w9vkv5tqoBkr%2flt%2bAe9tI%2bj0ZwCC6JIs3kKuJxSfBB6wJH0PEXkhtpS4GZZv6eyA9jhwKDXSyVHZS%2bxEgQydYHDlI4s44n0pyRTPkSkNozn4Ki5mAbwH6gB4jKu%2bevq26Q20rObt7tHUjOwxZqMbk5qXPvEvpvn4kDBwN3PEWgzJeDrrhDgiua5kH6Hg%3d";
            string WriteWord = post(url, param);
            str.Write(WriteWord);
            str.Close();

            File.Copy(System.Windows.Forms.Application.StartupPath + "\\login.txt", System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_login.txt");

            //string url = "https://zh.towerofsaviors.com/api/user/login?type=facebook&uniqueKey=14d8a07368c350485a658939e2fe2e32&deviceKey=2fa551dd6a83c64e2bfc802906536fea&sysInfo=Android+OS+5.1.1+%2f+API-22+(LMY48Z%2feng.se.infra.20180530.161229)%7cIntel+x86+SSE3%7c2%7c2022%7c512%7cAdreno+(TM)+540%7cFALSE%7cOpenGL+ES+2.0+build+1.3%402768895%7cFull%7cFALSE%7c16384%7c17.03%7c%7c%7c06%3abc%3a27%3a5f%3a7e%3ae6&session=&loginToken=e900e589e87e9be2a6dda26b64783bb8&apKey=371058341&A1=d41d8cd9&A2=d41d8cd9&A3=d41d8cd9&A4=d41d8cd9&A5=d41d8cd9&A6=d41d8cd9&A7=d41d8cd9&A8=d41d8cd9&A9=d41d8cd9&A10=d41d8cd9&A11=d41d8cd9&A13=d41d8cd9&A12=d41d8cd9&idfv=&mac=06%3abc%3a27%3a5f%3a7e%3ae6&geoData=0%2c0&versionKey=ulYzGFokOZWQJBj6&language=zh_TW&platform=android&version=17.03&timestamp=1552040533&timezone=8&external=True&revision=459&iay=fefd671&hash=5a75b05a29683198d1f3fc76b8921d35";
            //string param = "head=3%2fJWw12BCSnYKjIupB8iGAC3PKLSwsuX7etsWzFhtnW8MVrDazkIT0rLEJ1TtnzrWTt5VcxhsM8JNEFqGx9ry5BVFuyWbwcXCfxFyZ3VOUR5bk%2bOIemQRkeB0yU8gxU8tJ09mLfj%2f8ShnQAr0jaTiuc%2fjTKqcFs%2bcn8i1VFhQBGO7E%2bSLFsTPmSvHvjiYfJzBWe%2fgvGnTV%2bY4bHKmax72%2bvqngr5eu0WUW2BA0ZeAMK1x5R9YrPR0%2f6WDD2Mz%2b%2f1I8dDfehcy7NmBvRoOWsLql9MKac17lkWL7gK81aKC%2fTo4iva%2f9gHG3P%2fJMs%2fDhoSWJKwfjZw6LuLw6Kf%2bMQIYc6zbM0EFCXPAUXnTCbLWtLIKjjBcy%2fWWNVxQXu3mSaGuKMFS01fBDTzGBMBt6c7KX3yvD%2bkWzGLXS7h17rQHOr9T6%2fa4TNWFx1ViVaBe2veu2D7HrkjVTPoA%2fk52Ygiu1wLpztVGVbwlh9VwnsHe7bFiWWx6ekxrJwb%2bEd1KgBN4uYMnLBSKUdmfQdnWok2%2bpRxqjs9jNzhMfhIxlf7vDGU4wsMOmX1tC9V36whgz5T7KeuousFQGlm%2b6d7mLvSvF8JSWv6SFg80kYrzg7XIpCUpmQDoQBZRMncOKCArl60mrsACXTvuTwDDJau7X3aSnygqbnBHN8IoNffgZZTwh%2fHV2M5S9CbA2%2biGyGTvkjud9yuiARh%2f5SRU%2b9zD%2bSo%2bHE5Pefgb0Lei7MrRffYeWgCF0YDBXJIvr86aZY2i9MKIxHLVKK38fXCGqP6ctYp9gohb3Tn%2bgVDNGrfiJM6%2brz5pS9HmSJxKLkfYvrX%2bkAGmsx82gzMon01FHIcHd%2bpGjQVznpna8c2JaQuncyBnLSYyK%2bWKaboO2mBLhgPKPUgpk88lsg6apyVcXZ9cVUXjlpaRO%2bIGPA9vGLl98QnZbd%2fbDklKsRgUlVrd26FXMQe0Qtvcp5jzMoZv3aj1DEPkhrqqMrmbnF8ymxRuIXaQTFdRHKP13aruo1PvQsPpfDJ%2bh%2fCb6fiuqkKsCwUt%2fvJsC3TuWKCykmHRydUvh8S8dNsvlj1jKs1aUhQD6NQ3ss%2bp72XUhXUxiqEADHymTUa53NrJqUXzW2ygdk00EoEk4jl10xtvDIt3COMr57t8qKDr8qcH4VImc39uMPb4vEMrWfXk5Us7ZPHBK8JM24HqXfBbi%2bB%2bBJlQve2I0b7mw1oy%2bms33ua4lrKUznP%2fZ8rI18rtgfIIy%2bfgmffZf04z5jmGPuBGOBlhbfyoxXaUmpXnFgfGNcgcEDSOs5aQLwde%2bFfdiSgKrC7a62bzyYYaqIEezCZpDTyLBa5AADfnamSyjP%2bUENRzxMIxqlJncADBgTs0li9X1Ll1s2MUcX0v9Fv3AvM45He%2fXdmz5qT1%2f3F1PhhoYwUr79mnOtsaFnS9o6O4FtwZg42eifVQ8tJDSYnrhV8e0e6Qu7oqCbBJ1FovBKfYoQqylCmIUwG4h2Esq1O4afmGqusFOK9VwPy%2b0CBGsSHH5y%2bkUclXBGahJHC";            
            //string WriteWord = post(url, param);

            StreamWriter stageList = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\stageList.txt");
            StreamWriter floorList = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\floorList.txt");
            StreamWriter floorStars = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\floorStars.txt");
            StreamWriter floorStarRewards = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\floorStarRewards.txt");
            StreamWriter monsterSkins = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\monsterSkins.txt");

            JObject j_login = JObject.Parse(WriteWord);

            string tbl_title;

            tbl_title = "";
            if (j_login["data"]["stageList"] != null)
            {
                if(j_login["data"]["stageList"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["stageList"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    stageList.Write(tbl_title);
                    foreach(string ln in j_login["data"]["stageList"])
                        stageList.Write("\r\n"+ln.Replace("\r\n", "char(10)"));
                }
            }
            stageList.Close();

            tbl_title = "";
            if (j_login["data"]["floorList"] != null)
            {
                if (j_login["data"]["floorList"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["floorList"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    floorList.Write(tbl_title);
                    foreach (string ln in j_login["data"]["floorList"])
                        floorList.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            floorList.Close();

            tbl_title = "";
            if (j_login["data"]["floorStars"] != null)
            {
                if (j_login["data"]["floorStars"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["floorStars"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    floorStars.Write(tbl_title);
                    foreach (string ln in j_login["data"]["floorStars"])
                        floorStars.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            floorStars.Close();

            tbl_title = "";
            if (j_login["data"]["floorStarRewards"] != null)
            {
                if (j_login["data"]["floorStarRewards"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["floorStarRewards"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    floorStarRewards.Write(tbl_title);
                    foreach (string ln in j_login["data"]["floorStarRewards"])
                        floorStarRewards.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            floorStarRewards.Close();

            tbl_title = "";
            if (j_login["data"]["monsterSkins"] != null)
            {
                if (j_login["data"]["monsterSkins"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["monsterSkins"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    monsterSkins.Write(tbl_title);
                    foreach (string ln in j_login["data"]["monsterSkins"])
                        monsterSkins.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            monsterSkins.Close();

            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "stageList.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "floorList.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "floorStars.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "floorStarRewards.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "monsterSkins.txt");

            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "stageList.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "floorList.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "floorStars.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "floorStarRewards.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "monsterSkins.txt");

            StreamWriter stgDtlHtml = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\StageDetailHtml.html");
            stgDtlHtml.Write(getStageDetailAll());
            stgDtlHtml.Close();
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "StageDetailHtml.html");

            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "StageDetailHtml.html");
        }

        protected void uploadFtp(string url, string acc, string pwd, string filename)
        {
            FtpWebRequest request = (FtpWebRequest)WebRequest.Create(url + filename);
            request.Method = WebRequestMethods.Ftp.UploadFile;

            // FTP的帳密.
            request.Credentials = new NetworkCredential(acc, pwd);

            // 要上傳的檔案
            StreamReader sourceStream = new StreamReader(System.Windows.Forms.Application.StartupPath + "\\" + filename);
            byte[] fileContents = Encoding.UTF8.GetBytes(sourceStream.ReadToEnd());
            sourceStream.Close();
            request.ContentLength = fileContents.Length;

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(fileContents, 0, fileContents.Length);
            requestStream.Close();

            FtpWebResponse response = (FtpWebResponse)request.GetResponse();

            Console.WriteLine("Upload File Complete, status {0}", response.StatusDescription);

            response.Close();
        }

        private void btn_gamedata_Click(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Json\\");
            }

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\gamedata.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\gamedata.txt");

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_gamedata.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_gamedata.txt");

            StreamWriter str = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\gamedata.txt");
            string url = "https://zh.towerofsaviors.com/api/user/gameData?loginToken=e900e589e87e9be2a6dda26b64783bb8&apKey=92799478&A3=c5bba60a&idfv=&mac=06%3abc%3a27%3a5f%3a7e%3ae6&geoData=0%2c0&uid=402873355&session=6362a1de107c4e921183939cb6a32511&versionKey=ulYzGFokOZWQJBj6&language=zh_TW&platform=android&version=17.03&timestamp=1552153113&timezone=8&external=True&revision=459&iay=9d6ec29&hash=fe3d339064c5a7958a5f582624907bbd";
            string param = "head=3%2fJWw12BCSnYKjIupB8iGAC3PKLSwsuX7etsWzFhtnW8MVrDazkIT0rLEJ1TtnzrWTt5VcxhsM8JNEFqGx9ry5BVFuyWbwcXCfxFyZ3VOUR5bk%2bOIemQRkeB0yU8gxU8tJ09mLfj%2f8ShnQAr0jaTiuc%2fjTKqcFs%2bcn8i1VFhQBGO7E%2bSLFsTPmSvHvjiYfJzBWe%2fgvGnTV%2bY4bHKmax72%2bvqngr5eu0WUW2BA0ZeAMK1x5R9YrPR0%2f6WDD2Mz%2b%2f1I8dDfehcy7NmBvRoOWsLql9MKac17lkWL7gK81aKC%2fTo4iva%2f9gHG3P%2fJMs%2fDhoSWJKwfjZw6LuLw6Kf%2bMQIYRt%2b5Q82wFVMA2ViMH9WGAxI7K%2fZtWxib0DMwuvD8DPaNr1AKvTKf2Zy5AMRiY7fOgcw%2b120Kr%2bm2acCEUnNd9SHLdUxObzQOyz659W6v1WMipeMkYztMt69TG1sdx7NPa0SUxI%2b8Z%2fxJMuMwe3EkJXSxTF%2bnf0xKWPHHwIBY5C%2b8z%2b7raEP55ufN7Lt94gsE7arE7wjlf0vmQQ5v6oJ%2faazzSeHcUsf9RDjThoqxjbcbkuC6CIEp8sA9EhvJ1Dx51AgL3VmzpFaCYUvL0CS11LDJGbRVuEo%2fa3x%2bLZFK8r%2ftnoUKo%2fllaeAJAwfl5UGDI8vfXvlWyQiZtav1zI1p2bXTJBFzckpsHxAMXNUunrX7LQB1r4ADFqMHo1S%2bwm84b2wVcSU3hmYK9vi4m500T%2fOctYgJrMwEzs0VeCeONbCujHw6F2cR41kf75%2fbCeK%2bbtDBcaIRBkyTXIz9sptIUX5pS9HmSJxKLkfYvrX%2bkAGqr8lqNzUFeHASEGToXjyN7IXBG7rvZXslgkkZnQtNTiYyK%2bWKaboO3t%2bQX11wCwRMJoWksiQHuevMRwZCTZuik%2ffVV3ZbeTe7xpd%2btnfAWVETwL04tSX8o4%2fnWNLmQXm1jqbE3d%2fhUBSNJUu%2bE%2br5AvK%2bZhuuRn4DycTDyJ1WsddRHKP13aruo1PvQsPpfDJ%2bh%2fCb6fiuqmIA9wXoX6Si9nsw4X378Vnt5ORvOyEukvmfQrsP8nLcUhQD6NQ3ss%2bp72XUhXUxipqaD1A7ZObkxbBkfLtCi1pixm5G%2bzX9TtwmDEYndGdcaBM2%2b7LPA7MK1h%2fQnnSWTQeWDooM7gz4letLk1%2fv%2ffsb5ulbyVpEIpsaVHDJTaXK%2fe2I0b7mw1o80rUWcLJKBcHRRf6LWTxLu7zyfMDqv4DGrCkkvDRWT3mGPuBGOBlha5Hh9JgeE1wXy%2bLSPR2i0DIGgxxbF0LZT%2fVna%2f8cDnQ54eBibMlw4YEezCZpDTyLO0XmVK8CfevP76mkeyqR3%2fcUaVot1Exh8MEsU7D%2f9xu6tlbdnErFJy8e57ePu1Bh7WVZnlbmbrm0UMFUYuUtWL%2bO7U9tMk9P3nWFx8LJ1FwaVuIx01cjQ8872Fjf6vSp3HdxLlLiTh7p9iscruogVxitZZOv09X5a1O4afmGquskokIAlFG1w1nF3O8HSJE84EaxIcfnL6R24rxUrRkM06mzLFvjB6mofjQ646%2bngY6Yi8pUtrKnpXdxrgQO6Hrlw%3d%3d";
            string WriteWord = post(url, param);
            str.Write(WriteWord);
            str.Close();

            File.Copy(System.Windows.Forms.Application.StartupPath + "\\gamedata.txt", System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_gamedata.txt");
        }

        private void btn_dairylog_Click(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Json\\");
            }

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\dairy.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\dairy.txt");

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_dairy.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_dairy.txt");

            StreamWriter str = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\dairy.txt");
            string url = "https://zh.towerofsaviors.com/api/user/dailyCheckinV13?uid=402873355&session=6362a1de107c4e9260c3b01cf0d84a4d&versionKey=ulYzGFokOZWQJBj6&language=zh_TW&platform=android&version=17.03&timestamp=1552040562&timezone=8&external=True&revision=459&iay=d653e2d&hash=a8d7a76b5a6b230e7b74cf6d8c610da6";
            string param = "head=KRPuG2XO4QC1VmmtgNRHsYLxth9kG4b0Mev%2bhns06mPsy1bv0B0r%2fSOw6wHbyd8OQD3yBIE6dwN684ffOZI70fEht%2fj%2bxfUwLTuvP6nwUByhIsf9Y7R%2bRyBw9TptSrgiv1VrYOq40OXUJlqWhHhrnRVu2zsewS%2fOqTd4uqTX1LVvJV1xHPDivxV55t5wTuwCBzdoUVo0944H06OfW4vfOxrs37Dr%2ffldPhRW5yGsUuja2uklh3FMjku%2fOI5SaZnsY02VRdl0R4CQzr4pcyJL%2b6Uo724u%2f0Is8hmchclxb7EoUTInfSD2sfxB%2bqYn1%2fW8UESf%2foKM37Ncd7%2feEyR62Wxd%2bZXjK3Aum0ogp054nVjkXfYvve1A1J3P8fxllYlWRZ5D0UHSohxqtviTIgqGnX2ALSSv6b7MpXfD436i9Lf%2bv4pI8UFWFJ4miTLUvrppVx348XT59bHPx5IS0dWx2b3Ui1CicBAJgcwCCq5AjLmpJzgndwZVhuxla5pYBZrvuFahyjAY1qGK7g2byHABM2RCf0Bed4rv6mtcB0T18vQa0twhfB2tq9MYet3BLLvOpODCt1purvmeV7RFQ3JPf%2fBL6jBXyuswOMyoOwZA1Ygt6HQs2vUwSsZdExx32LpePdbSdS5bThRSYEsFhEe6hCqmt9mdyhacpTlDeQcrViRS72l5e14gbTuZcnQP7URthhyzv7hJfyX9cEmyIEmmFe30SSXUmVDl5fd0B%2fLRB9Fg5qFeFgoApCsLr3u93ibw5ZeW%2f8rHQlaQ488tSJL2TVsx3uttl0Okqevwq%2bLzqUk9BcfszgCf8TLchk%2ff%2bU%2fCM7DBt4Z%2fnr%2b4PAAVwWUFciA7xus5LRYRhod7HLser91M%2b7cEdsCaTb9TJpl0pw4E4JGcaB%2bTHV7p0NRnOL3FchkZNH%2b6NvQu%2frIGFkzzxfKvAQOJ5tqTeTH%2bc6eRENkIIk3YSQ7UK53thi3Th%2bxlgDxlE53LsZ7eOKmXvmxesUM007eqozALGNockWiaUSLnh2sQMhwS0XCV5KVVrVu%2fvT8q70ATs1Kea8I6DHCXh8bK27Ml39aJkWjmCvm5s9eNG2UxeRO4oG%2b25mkqHmIy9ZmI%2fAM4UCbwkaA51W%2f9vQk3y%2fpsFFtN5%2fL7gXdqj6i1qnsyMduTXZnNaiHza7ReY9CR2sc0afcnjjsl4gVg03vnxxE3eugDF21bJ8ODQLVOVainSl0raJ04l%2f0q0Clc3bGoh1yRR7dOQrRUuCaEwJFWcDckLnLM4C8I0pEl%2fT2R";
            string WriteWord = post(url, param);
            str.Write(WriteWord);
            str.Close();

            File.Copy(System.Windows.Forms.Application.StartupPath + "\\dairy.txt", System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_dairy.txt");
        }

        private void btn_data_switch_Click(object sender, EventArgs e)
        {
            tm_login_packet.Enabled = !tm_login_packet.Enabled;
            ((System.Windows.Forms.Button)sender).ForeColor = tm_login_packet.Enabled ? System.Drawing.Color.Red: System.Drawing.Color.Black;
        }

        private void tm_login_packet_Tick(object sender, EventArgs e)
        {
            string time = DateTime.Now.ToString("HHmmss");
            if(time == "000000")
            {
                btn_logindata.PerformClick();
            }
            if (time == "190000")
                btn_logindata_en.PerformClick();
        }

        private void btn_logindata_en_Click(object sender, EventArgs e)
        {
            string date = DateTime.Now.ToString("yyyyMMdd");

            if (!Directory.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\"))
            {
                Directory.CreateDirectory(System.Windows.Forms.Application.StartupPath + "\\Json\\");
            }

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\login_en.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\login_en.txt");

            if (File.Exists(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_login_en.txt"))
                File.Delete(System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_login_en.txt");

            StreamWriter str = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\login_en.txt");
            string url = "https://tosen.towerofsaviors.com/api/user/login?type=facebook&uniqueKey=9b33c86f46d7a20a59a860c00bf997e5&deviceKey=9198d459a03488631865ec43c380d8a3&sysInfo=Android+OS+5.1.1+%2f+API-22+(LMY48Z%2feng.se.infra.20180530.161229)%7cARMv7+VFPv3+NEON+VMH%7c0%7c2022%7c512%7cAdreno+(TM)+540%7cFALSE%7cOpenGL+ES+2.0+build+1.3%402768895%7cFull%7cFALSE%7c16384%7c17.43%7c%7c%7cca%3a87%3a2d%3a64%3a4f%3ac0&session=&loginToken=57d51546df5d121385d80466f4f0cb3f&apKey=553379154&A1=d41d8cd9&A2=d41d8cd9&A3=d41d8cd9&A4=d41d8cd9&A5=d41d8cd9&A6=d41d8cd9&A7=d41d8cd9&A8=d41d8cd9&A9=d41d8cd9&A10=d41d8cd9&A11=d41d8cd9&A13=d41d8cd9&A12=d41d8cd9&idfv=&mac=ca%3a87%3a2d%3a64%3a4f%3ac0&geoData=0%2c0&versionKey=ltKWB4WRrG3hvBre&language=zh_TW&platform=android&version=17.43&timestamp=1565596751&timezone=13&tosLang=zh&revision=1048&iay=c1f858d&hash=9d65f8310dab61a5418e6727be3c8e04";
            string param = "head=3%2fJWw12BCSnYKjIupB8iGPtOD5%2fYZGMZIPNolHHGB1tYCeb49ID7ZgAn2boLM3zqmHaUo1Ki7L2%2b%2fTBWRDLrsNYZrhHTjH76dVlHJjVm9oCVM1BmRmADRa%2b%2bWFR5pnOA8iNmamUsO5lD4V0Cjn8k%2f4PEMDNscAAT8cd%2bqwVM6s9H%2fUZHLggi84RqUeTjqsTAOsCXqD005rQH7lZYEqT0luZ%2bGHwlG4ZyGL7pbUVXE6tPRCc8mnf%2bpSwMthDabI%2bS20IYhMevBguUW5RfcyrrwZcx3nlW2TGCf76uJ3CazyciRuhYel%2fewC0q49Es%2btUR5E501onXeJu2%2bI6lSwf%2b1Mec39zQF%2fG1krccLWtp%2fe9Wf2EiTxVGaDhXdbvOn1kjg87VSCHOUPrPZzy7hELwMIz6N9W2ZWIOGIGbt2iCbgNbvzH3fEDMyBb6stYf7ugai4oMPo1ryiB3PCiX6JnwsjC1Q07qkZVLipeMkYztMt69TG1sdx7NPa0SUxI%2b8Z%2fxJMuMwe3EkJXSxTF%2bnf0xKWPHHwIBY5C%2b8z%2b7raEP55ufN7Lt94gsE6%2fbT0vmHa5OU61kpJnNPFLsMPuuAHU4EDuL%2fiwoJeLDXwlJa%2fpIWDzSRivODtcikJSmZAOhAFlEydw4oICuXrSauwAJdO%2b5PAMMlq7tfdpKfKCpucEc3wig19%2bBllPCH8dXYzlL0JsDb6IbIZO%2bSO533K6IBGH%2flJFT73MP5Kj4cTk95%2bBvQt6LsytF99h5aAIXRgMFcki%2bM%2bftOOzCqGZGPUsww0vwxDJGgK8aTYrYCiFvdOf6BUM0at%2bIkzr6vPmlL0eZInEouR9i%2btf6QAYpOEXozqAGeBQlgaDFkyXp41LvT5ziCd6lV%2f5vOq5YFZjIr5Yppug7vkO1qUycKr1C67SMkOoU9wZ%2bDhOdyZGtfNruBDbxIK7%2fqtEb6gHk2vvlP6Et07hFWbsO1eWOWytIJ71%2bqn%2bYpa3WtYmbsmGk4w%2b6%2fRBuNOxpZKiBqQu5zpErP7QiHPsvjU%2b9Cw%2bl8Mn6H8Jvp%2bK6qdP%2btem1i09j7YC%2bQceoT8duoW29nhu0B1AZnRe4UbZ%2bSFAPo1Deyz6nvZdSFdTGKh9%2fqWKD%2byP0THA2nIcqKauHyqOKf0BKnZxo16OWkAZjpubiiprIO2edSiTzxNwMVGy%2fvGA08sUUSq5Y69u5HGWrHSiM75kqxy3jczyryt%2b097YjRvubDWgoRQzv8ifQZL%2bXPnjN2feNuWUu6FTE830apu1ZsxAboeYY%2b4EY4GWFXHb%2fqkO7TgIXNFZf2YEs0gQWxU89jZ4G%2fRFE6hBx8oi8jVnbfspu9AR7MJmkNPIsuxh07AFIrUlNGlovgqdRnAb6uHQn70myPB0GZdsEWPDPrL5Z3i%2by3ohUC3DMUqwOW5xNjWgCk7aGXuQfLZbGYdorwPEkda38rwr%2fg23nX269wzJrVfT%2fmxj%2fRX6Qopz7uY115b%2f%2bSr67hVjoiyhiUOF1N3IX3kkirU7hp%2bYaq6wU4r1XA%2fL7QIEaxIcfnL6RRyVcEZqEkcI%3d";
            string WriteWord = post(url, param);
            str.Write(WriteWord);
            str.Close();

            File.Copy(System.Windows.Forms.Application.StartupPath + "\\login_en.txt", System.Windows.Forms.Application.StartupPath + "\\Json\\" + date + "_login_en.txt");

            StreamWriter stageList_en = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\stageList_en.txt");
            StreamWriter floorList_en = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\floorList_en.txt");
            StreamWriter floorStars_en = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\floorStars_en.txt");
            StreamWriter floorStarRewards_en = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\floorStarRewards_en.txt");

            JObject j_login = JObject.Parse(WriteWord);

            string tbl_title;

            tbl_title = "";
            if (j_login["data"]["stageList"] != null)
            {
                if (j_login["data"]["stageList"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["stageList"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    stageList_en.Write(tbl_title);
                    foreach (string ln in j_login["data"]["stageList"])
                        stageList_en.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            stageList_en.Close();

            tbl_title = "";
            if (j_login["data"]["floorList"] != null)
            {
                if (j_login["data"]["floorList"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["floorList"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    floorList_en.Write(tbl_title);
                    foreach (string ln in j_login["data"]["floorList"])
                        floorList_en.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            floorList_en.Close();

            tbl_title = "";
            if (j_login["data"]["floorStars"] != null)
            {
                if (j_login["data"]["floorStars"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["floorStars"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    floorStars_en.Write(tbl_title);
                    foreach (string ln in j_login["data"]["floorStars"])
                        floorStars_en.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            floorStars_en.Close();

            tbl_title = "";
            if (j_login["data"]["floorStarRewards"] != null)
            {
                if (j_login["data"]["floorStarRewards"].Count() > 0)
                {
                    int tbl_col_cnt = (j_login["data"]["floorStarRewards"][0].ToString().Split('|')).Count();
                    for (int i = 0; i < tbl_col_cnt; i++)
                        tbl_title += "col_" + (i + 1).ToString() + "|";
                    tbl_title = tbl_title.Substring(0, tbl_title.Length - 1);

                    floorStarRewards_en.Write(tbl_title);
                    foreach (string ln in j_login["data"]["floorStarRewards"])
                        floorStarRewards_en.Write("\r\n" + ln.Replace("\r\n", "char(10)"));
                }
            }
            floorStarRewards_en.Close();
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "stageList_en.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "floorList_en.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "floorStars_en.txt");
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "floorStarRewards_en.txt");

            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "stageList_en.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "floorList_en.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "floorStars_en.txt");
            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "floorStarRewards_en.txt");
            
            StreamWriter stgDtlHtmlEn = new StreamWriter(System.Windows.Forms.Application.StartupPath + "\\StageDetailHtmlEn.html");
            stgDtlHtmlEn.Write(getStageDetailAllEn());
            stgDtlHtmlEn.Close();
            uploadFtp("ftp://asdfghjklu2000@tosapphou.somee.com/www.TosappHou.somee.com/", "asdfghjklu2000", "Fn760508", "StageDetailHtmlEn.html");

            uploadFtp("ftp://waws-prod-dm1-115.ftp.azurewebsites.windows.net/site/wwwroot/", "TosappTool\\$TosappTool", "wK2aHNo3u3vMLbhhGKPlwxHl0fNMAnx6ji0sN5nFYf0oMeeL89fLdhmBAu7H", "StageDetailHtmlEn.html");
        }

        protected void loadLocaleCsv()
        {
            MH.MCSVFile m = new MH.MCSVFile();
            m.LoadBinaryAsset(ReadFile("LOCALE.csv.txt"));

            Dictionary<string, string> TipsManager = new Dictionary<string, string>();

            //m.NumRow()為所有字串的總數
            int num = m.NumRow();
            //宣告一個陣列用來接MCSVFile破解出來的字串
            string[,] dict = new string[num, 3];

            //用來接
            string text = string.Empty;
            string text1 = string.Empty;
            string text2 = string.Empty;

            int num2 = 0;
            for (int i = 0; i < num - 1; i++)
            {
                text = string.Empty;
                text1 = string.Empty;
                text2 = string.Empty;
                if (!string.IsNullOrEmpty(m.GetString(i, 0, string.Empty)))
                {
                    //m.GetString(i, 0, string.Empty)是字串的KEY值
                    text = m.GetString(i, 0, string.Empty);
                    //m.GetString(i, 0, string.Empty)是字串的英文描述
                    text1 = m.GetString(i, 1, string.Empty);
                    //m.GetString(i, 0, string.Empty)是字串的中文描述
                    text2 = m.GetString(i, 2, string.Empty);
                    try
                    {
                        //寫到陣列去
                        dict[i, 0] = text;
                        dict[i, 1] = text1;
                        dict[i, 2] = text2;

                        dict_locale_csv.Add(text, text2);
                        dict_locale_csv_eng.Add(text, text1);
                    }
                    catch (Exception var_8_9E)
                    {
                        //Watchdog.LogError("[SimpleLocale] (Duplicated Translation Label) Key: " + text + " Msg: " + text2);
                    }
                    string[] array = text.Split(new char[] { '_' });
                    if (array.Length == 2 && text.StartsWith("TIP_") && int.TryParse(array[1], out num2))
                    {
                        TipsManager.Add(text, text2);
                    }
                }
            }
        }

        public string getStageDetailAll()
        {
            SimpleLocale.dict = dict_locale_csv;
            string HtmlStr = "";
            DataTable dt_stageList, dt_floorList, dt_floorStars, dt_floorStarRewards;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            try
            {

                dt_stageList = TxtConvertToDataTable("stageList.txt", "stageList", "|");
                dt_floorList = TxtConvertToDataTable("floorList.txt", "stageList", "|");
                dt_floorStars = TxtConvertToDataTable("floorStars.txt", "stageList", "|");
                dt_floorStarRewards = TxtConvertToDataTable("floorStarRewards.txt", "stageList", "|");
                
                var qry_locale = from dr in dict_locale_csv
                                 where dr.Key.StartsWith("ZONE_") && dr.Value.Trim() != ""
                                 select dr;
                foreach (KeyValuePair<string, string> kp in qry_locale)
                {
                    HtmlStr += "<fieldset>";
                    HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_zone_" + kp.Key.Replace("ZONE_", "") + "')\">";
                    HtmlStr += "<font color=\"4CAF50\" size=\"5\"><b>" + kp.Value + "</b></font>";
                    HtmlStr += "<span style='float:right; display:block;' id='div_zone_" + kp.Key.Replace("ZONE_", "") + "_1'>∨&nbsp</span>";
                    HtmlStr += "<span style='float:right; display:none;' id='div_zone_" + kp.Key.Replace("ZONE_", "") + "_2'>∧&nbsp</span>";
                    HtmlStr += "</legend>";
                    HtmlStr += "<div style='display:none;' id='div_zone_" + kp.Key.Replace("ZONE_", "") + "'>";

                    
                    var qry_stage = from dr in dt_stageList.AsEnumerable()
                                    where dr.Field<string>("col_4") == kp.Key.Replace("ZONE_", "")
                                    select dr;


                    foreach (DataRow dr_stage in qry_stage)
                    {
                        //Context.Response.Output.Write(dr["col_1"].ToString());
                        HtmlStr += "<fieldset>";
                        HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_part_" + dr_stage["col_1"].ToString() + "')\">";
                        HtmlStr += "<font color=\"3366FF\" size=\"4\"><b>" + dr_stage["col_10"].ToString() + "</b></font>";

                        HtmlStr += dr_stage["col_8"].ToString() == "" ? "" : ("&nbsp(開放時間： " + startTime.AddSeconds(Int32.Parse(dr_stage["col_8"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " ~ " + startTime.AddSeconds(Int32.Parse(dr_stage["col_9"].ToString()) - 1).ToString("yyyy/MM/dd HH:mm:ss") + " )");
                        HtmlStr += "<span style='float:right; display:none;' id='div_part_" + dr_stage["col_1"].ToString() + "_1'>✚&nbsp</span>";
                        HtmlStr += "<span style='float:right; display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "_2'>━&nbsp</span>";
                        HtmlStr += "</legend>";
                        HtmlStr += "<div style='display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "'>";
                        if (dr_stage["col_16"].ToString() != "")
                            HtmlStr += "【副本說明】<br>" + dr_stage["col_16"].ToString().Replace("char(10)","<br>") + "<br>";
                        HtmlStr += "→<a href=\"https://tos.fandom.com/zh/wiki/" + dr_stage["col_10"].ToString().Replace(' ', '_') + "\" target=\"blank\">維基關卡資訊</a>";
                        HtmlStr += "<hr>";


                        var qry_floor = from c in dt_floorList.AsEnumerable()
                                        join o in dt_floorStars.AsEnumerable() on c.Field<string>("col_1") equals o.Field<string>("col_1") into ps
                                        from o in ps.DefaultIfEmpty()
                                        where c.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                        select new
                                        {
                                            floorId = c.Field<string>("col_1"),
                                            floorIcon = c.Field<string>("col_4"),
                                            floorName = c.Field<string>("col_8"),
                                            floorStam = c.Field<string>("col_5"),
                                            floorSpirit = c.Field<string>("col_29"),
                                            floorRound = c.Field<string>("col_6"),
                                            floorStar1 = o == null ? "" : o.Field<string>("col_2"),
                                            floorStar2 = o == null ? "" : o.Field<string>("col_3"),
                                            floorStar3 = o == null ? "" : o.Field<string>("col_4"),
                                            floorLimit = c.Field<string>("col_27"),
                                            floorDrop = c.Field<string>("col_36")
                                        };

                        foreach (var dr_floor in qry_floor)
                        {
                            HtmlStr += "【<a href=\"http://www.tosapp.tw/tos/no." + dr_floor.floorIcon + ".html\" target=\"blank\">" + dr_floor.floorIcon + "</a>】";
                            HtmlStr += "<img width=\"40\" height=\"40\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dr_floor.floorIcon + ".png\">";
                            HtmlStr += "&nbsp";
                            HtmlStr += dr_floor.floorName;
                            HtmlStr += "<br>";
                            HtmlStr += "　體力： " + (dr_floor.floorStam != "0" ? dr_floor.floorStam : (dr_floor.floorSpirit != "0" ? (dr_floor.floorSpirit + " 戰靈") : "0"));
                            HtmlStr += "；戰鬥： " + dr_floor.floorRound;

                            if (dr_floor.floorLimit != "")
                            {
                                Floor f = new Floor();
                                f.limitation = dr_floor.floorLimit;
                                f.UpdateLimitations();
                                string mes = "";
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupA)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupB)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupC)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                HtmlStr += "<br>【關卡限制】" + mes;
                            }
                            if (dr_floor.floorDrop != "")
                            {
                                string dropList = "";
                                if (dr_floor.floorDrop.IndexOf(',') > 0)
                                    dropList = dr_floor.floorDrop.Split(',')[0];
                                else
                                    dropList = dr_floor.floorDrop;
                                HtmlStr += "<br>";
                                HtmlStr += "　MayDrop： " + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "(★)");
                                HtmlStr += "<br>";
                                HtmlStr += "　<img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "").Replace(":", ".png\"><img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/") + ".png\">";
                            }
                            if (dr_floor.floorStar1 != "")
                            {
                                HtmlStr += "<br>★ 成就";
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar1;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar2;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar3;
                            }
                            HtmlStr += "<hr>";
                        }

                        var qry_starReward = from dr in dt_floorStarRewards.AsEnumerable()
                                             where dr.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                             select dr;
                        if (qry_starReward.Count() > 0)
                            HtmlStr += "<br>✪ 成就獎勵";
                        foreach (DataRow dr_star in qry_starReward)
                        {
                            HtmlStr += "<br>";
                            HtmlStr += dr_star["col_3"].ToString();
                        }
                        HtmlStr += "<br>";
                        HtmlStr += "</div></fieldset>";
                    }
                    HtmlStr += "</div></fieldset>";
                }
                string[] addZone = txt_zone.Text.Split(',');
                foreach (string zonenumber in addZone)
                {
                    HtmlStr += "<fieldset>";
                    HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_zone_" + zonenumber + "')\">";
                    HtmlStr += "<font color=\"4CAF50\" size=\"5\"><b>ZONE_" + zonenumber + "</b></font>";
                    HtmlStr += "<span style='float:right; display:block;' id='div_zone_" + zonenumber + "_1'>∨&nbsp</span>";
                    HtmlStr += "<span style='float:right; display:none;' id='div_zone_" + zonenumber + "_2'>∧&nbsp</span>";
                    HtmlStr += "</legend>";
                    HtmlStr += "<div style='display:none;' id='div_zone_" + zonenumber + "'>";


                    var qry_stage = from dr in dt_stageList.AsEnumerable()
                                    where dr.Field<string>("col_4") == zonenumber
                                    select dr;


                    foreach (DataRow dr_stage in qry_stage)
                    {
                        //Context.Response.Output.Write(dr["col_1"].ToString());
                        HtmlStr += "<fieldset>";
                        HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_part_" + dr_stage["col_1"].ToString() + "')\">";
                        HtmlStr += "<font color=\"3366FF\" size=\"4\"><b>" + dr_stage["col_10"].ToString() + "</b></font>";

                        HtmlStr += dr_stage["col_8"].ToString() == "" ? "" : ("&nbsp(開放時間： " + startTime.AddSeconds(Int32.Parse(dr_stage["col_8"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " ~ " + startTime.AddSeconds(Int32.Parse(dr_stage["col_9"].ToString()) - 1).ToString("yyyy/MM/dd HH:mm:ss") + " )");
                        HtmlStr += "<span style='float:right; display:none;' id='div_part_" + dr_stage["col_1"].ToString() + "_1'>✚&nbsp</span>";
                        HtmlStr += "<span style='float:right; display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "_2'>━&nbsp</span>";
                        HtmlStr += "</legend>";
                        HtmlStr += "<div style='display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "'>";
                        if (dr_stage["col_16"].ToString() != "")
                            HtmlStr += "【副本說明】<br>" + dr_stage["col_16"].ToString().Replace("char(10)", "<br>") + "<br>";
                        HtmlStr += "→<a href=\"https://tos.fandom.com/zh/wiki/" + dr_stage["col_10"].ToString().Replace(' ', '_') + "\" target=\"blank\">維基關卡資訊</a>";
                        HtmlStr += "<hr>";

                        var qry_floor = from c in dt_floorList.AsEnumerable()
                                        join o in dt_floorStars.AsEnumerable() on c.Field<string>("col_1") equals o.Field<string>("col_1") into ps
                                        from o in ps.DefaultIfEmpty()
                                        where c.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                        select new
                                        {
                                            floorId = c.Field<string>("col_1"),
                                            floorIcon = c.Field<string>("col_4"),
                                            floorName = c.Field<string>("col_8"),
                                            floorStam = c.Field<string>("col_5"),
                                            floorSpirit = c.Field<string>("col_29"),
                                            floorRound = c.Field<string>("col_6"),
                                            floorStar1 = o == null ? "" : o.Field<string>("col_2"),
                                            floorStar2 = o == null ? "" : o.Field<string>("col_3"),
                                            floorStar3 = o == null ? "" : o.Field<string>("col_4"),
                                            floorLimit = c.Field<string>("col_27"),
                                            floorDrop = c.Field<string>("col_36")
                                        };

                        foreach (var dr_floor in qry_floor)
                        {
                            HtmlStr += "【<a href=\"http://www.tosapp.tw/tos/no." + dr_floor.floorIcon + ".html\" target=\"blank\">" + dr_floor.floorIcon + "</a>】";
                            HtmlStr += "<img width=\"40\" height=\"40\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dr_floor.floorIcon + ".png\">";
                            HtmlStr += "&nbsp";
                            HtmlStr += dr_floor.floorName;
                            HtmlStr += "<br>";
                            HtmlStr += "　體力： " + (dr_floor.floorStam != "0" ? dr_floor.floorStam : (dr_floor.floorSpirit != "0" ? (dr_floor.floorSpirit + " 戰靈") : "0"));
                            HtmlStr += "；戰鬥： " + dr_floor.floorRound;

                            if (dr_floor.floorLimit != "")
                            {
                                Floor f = new Floor();
                                f.limitation = dr_floor.floorLimit;
                                f.UpdateLimitations();
                                string mes = "";
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupA)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupB)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupC)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                HtmlStr += "<br>【關卡限制】" + mes;
                            }

                            if (dr_floor.floorDrop != "")
                            {
                                string dropList = "";
                                if (dr_floor.floorDrop.IndexOf(',') > 0)
                                    dropList = dr_floor.floorDrop.Split(',')[0];
                                else
                                    dropList = dr_floor.floorDrop;
                                HtmlStr += "<br>";
                                HtmlStr += "　MayDrop： " + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "(★)");
                                HtmlStr += "<br>";
                                HtmlStr += "　<img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "").Replace(":", ".png\"><img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/") + ".png\">";
                            }
                            if (dr_floor.floorStar1 != "")
                            {
                                HtmlStr += "<br>★ 成就";
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar1;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar2;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar3;
                            }
                            HtmlStr += "<hr>";
                        }

                        var qry_starReward = from dr in dt_floorStarRewards.AsEnumerable()
                                             where dr.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                             select dr;
                        if (qry_starReward.Count() > 0)
                            HtmlStr += "<br>✪ 成就獎勵";
                        foreach (DataRow dr_star in qry_starReward)
                        {
                            HtmlStr += "<br>";
                            HtmlStr += dr_star["col_3"].ToString();
                        }
                        HtmlStr += "<br>";
                        HtmlStr += "</div></fieldset>";
                    }
                    HtmlStr += "</div></fieldset>";
                }
            }
            catch (Exception e)
            {
                HtmlStr = "Exception: " + e.Message.ToString();
            }
            HtmlStr += "<br><br>"; //以免被somee的footer給遮蔽
            return HtmlStr;
        }

        public string getStageDetailAllEn()
        {
            SimpleLocale.dict = dict_locale_csv_eng;
            string HtmlStr = "";
            DataTable dt_stageList, dt_floorList, dt_floorStars, dt_floorStarRewards;
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1)); // 当地时区
            try
            {

                dt_stageList = TxtConvertToDataTable("stageList_en.txt", "stageList", "|");
                dt_floorList = TxtConvertToDataTable("floorList_en.txt", "stageList", "|");
                dt_floorStars = TxtConvertToDataTable("floorStars_en.txt", "stageList", "|");
                dt_floorStarRewards = TxtConvertToDataTable("floorStarRewards_en.txt", "stageList", "|");
                
                var qry_locale = from dr in dict_locale_csv
                                 where dr.Key.StartsWith("ZONE_") && dr.Value.Trim() != ""
                                 select dr;
                foreach (KeyValuePair<string, string> kp in qry_locale)
                {
                    HtmlStr += "<fieldset>";
                    HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_zone_" + kp.Key.Replace("ZONE_", "") + "')\">";
                    HtmlStr += "<font color=\"4CAF50\" size=\"5\"><b>" + kp.Value + "</b></font>";
                    HtmlStr += "<span style='float:right; display:block;' id='div_zone_" + kp.Key.Replace("ZONE_", "") + "_1'>∨&nbsp</span>";
                    HtmlStr += "<span style='float:right; display:none;' id='div_zone_" + kp.Key.Replace("ZONE_", "") + "_2'>∧&nbsp</span>";
                    HtmlStr += "</legend>";
                    HtmlStr += "<div style='display:none;' id='div_zone_" + kp.Key.Replace("ZONE_", "") + "'>";
                    
                    var qry_stage = from dr in dt_stageList.AsEnumerable()
                                    where dr.Field<string>("col_4") == kp.Key.Replace("ZONE_", "")
                                    select dr;


                    foreach (DataRow dr_stage in qry_stage)
                    {
                        //Context.Response.Output.Write(dr["col_1"].ToString());
                        HtmlStr += "<fieldset>";
                        HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_part_" + dr_stage["col_1"].ToString() + "')\">";
                        HtmlStr += "<font color=\"3366FF\" size=\"4\"><b>" + dr_stage["col_10"].ToString() + "</b></font>";

                        HtmlStr += dr_stage["col_8"].ToString() == "" ? "" : ("&nbsp(開放時間： " + startTime.AddSeconds(Int32.Parse(dr_stage["col_8"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " ~ " + startTime.AddSeconds(Int32.Parse(dr_stage["col_9"].ToString()) - 1).ToString("yyyy/MM/dd HH:mm:ss") + " )");
                        HtmlStr += "<span style='float:right; display:none;' id='div_part_" + dr_stage["col_1"].ToString() + "_1'>✚&nbsp</span>";
                        HtmlStr += "<span style='float:right; display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "_2'>━&nbsp</span>";
                        HtmlStr += "</legend>";
                        HtmlStr += "<div style='display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "'>";
                        if (dr_stage["col_16"].ToString() != "")
                            HtmlStr += "【副本說明】<br>" + dr_stage["col_16"].ToString().Replace("char(10)", "<br>") + "<br>";
                        HtmlStr += "→<a href=\"https://towerofsaviors.fandom.com/wiki/" + dr_stage["col_10"].ToString().Replace(' ', '_') + "\" target=\"blank\">美版維基關卡資訊</a><br>";
                        HtmlStr += "<hr>";

                        var qry_floor = from c in dt_floorList.AsEnumerable()
                                        join o in dt_floorStars.AsEnumerable() on c.Field<string>("col_1") equals o.Field<string>("col_1") into ps
                                        from o in ps.DefaultIfEmpty()
                                        where c.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                        select new
                                        {
                                            floorId = c.Field<string>("col_1"),
                                            floorIcon = c.Field<string>("col_4"),
                                            floorName = c.Field<string>("col_8"),
                                            floorStam = c.Field<string>("col_5"),
                                            floorSpirit = c.Field<string>("col_29"),
                                            floorRound = c.Field<string>("col_6"),
                                            floorStar1 = o == null ? "" : o.Field<string>("col_2"),
                                            floorStar2 = o == null ? "" : o.Field<string>("col_3"),
                                            floorStar3 = o == null ? "" : o.Field<string>("col_4"),
                                            floorLimit = c.Field<string>("col_27"),
                                            floorDrop = c.Field<string>("col_36")
                                        };

                        foreach (var dr_floor in qry_floor)
                        {
                            HtmlStr += "【<a href=\"http://www.tosapp.tw/tos/no." + dr_floor.floorIcon + ".html\" target=\"blank\">" + dr_floor.floorIcon + "</a>】";
                            HtmlStr += "<img width=\"40\" height=\"40\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dr_floor.floorIcon + ".png\">";
                            HtmlStr += "&nbsp";
                            HtmlStr += dr_floor.floorName;
                            HtmlStr += "<br>";
                            HtmlStr += "　體力： " + (dr_floor.floorStam != "0" ? dr_floor.floorStam : (dr_floor.floorSpirit != "0" ? (dr_floor.floorSpirit + " 戰靈") : "0"));
                            HtmlStr += "；戰鬥： " + dr_floor.floorRound;

                            if (dr_floor.floorLimit != "")
                            {
                                Floor f = new Floor();
                                f.limitation = dr_floor.floorLimit;
                                f.UpdateLimitations();
                                string mes = "";
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupA)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupB)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupC)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                HtmlStr += "<br>【關卡限制】" + mes;
                            }

                            if (dr_floor.floorDrop != "")
                            {
                                string dropList = "";
                                if (dr_floor.floorDrop.IndexOf(',') > 0)
                                    dropList = dr_floor.floorDrop.Split(',')[0];
                                else
                                    dropList = dr_floor.floorDrop;
                                HtmlStr += "<br>";
                                HtmlStr += "　MayDrop： " + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "(★)");
                                HtmlStr += "<br>";
                                HtmlStr += "　<img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "").Replace(":", ".png\"><img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/") + ".png\">";
                            }
                            if (dr_floor.floorStar1 != "")
                            {
                                HtmlStr += "<br>★ 成就";
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar1;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar2;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar3;
                            }
                            HtmlStr += "<hr>";
                        }

                        var qry_starReward = from dr in dt_floorStarRewards.AsEnumerable()
                                             where dr.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                             select dr;
                        if (qry_starReward.Count() > 0)
                            HtmlStr += "<br>✪ 成就獎勵";
                        foreach (DataRow dr_star in qry_starReward)
                        {
                            HtmlStr += "<br>";
                            HtmlStr += dr_star["col_3"].ToString();
                        }
                        HtmlStr += "<br>";
                        HtmlStr += "</div></fieldset>";
                    }
                    HtmlStr += "</div></fieldset>";
                }
                string[] addZone = txt_zone.Text.Split(',');
                foreach (string zonenumber in addZone)
                {
                    HtmlStr += "<fieldset>";
                    HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_zone_" + zonenumber + "')\">";
                    HtmlStr += "<font color=\"4CAF50\" size=\"5\"><b>ZONE_" + zonenumber + "</b></font>";
                    HtmlStr += "<span style='float:right; display:block;' id='div_zone_" + zonenumber + "_1'>∨&nbsp</span>";
                    HtmlStr += "<span style='float:right; display:none;' id='div_zone_" + zonenumber + "_2'>∧&nbsp</span>";
                    HtmlStr += "</legend>";
                    HtmlStr += "<div style='display:none;' id='div_zone_" + zonenumber + "'>";


                    var qry_stage = from dr in dt_stageList.AsEnumerable()
                                    where dr.Field<string>("col_4") == zonenumber
                                    select dr;


                    foreach (DataRow dr_stage in qry_stage)
                    {
                        //Context.Response.Output.Write(dr["col_1"].ToString());
                        HtmlStr += "<fieldset>";
                        HtmlStr += "<legend style=\"cursor: pointer\" onclick=\"isHidden('div_part_" + dr_stage["col_1"].ToString() + "')\">";
                        HtmlStr += "<font color=\"3366FF\" size=\"4\"><b>" + dr_stage["col_10"].ToString() + "</b></font>";

                        HtmlStr += dr_stage["col_8"].ToString() == "" ? "" : ("&nbsp(開放時間： " + startTime.AddSeconds(Int32.Parse(dr_stage["col_8"].ToString())).ToString("yyyy/MM/dd HH:mm:ss") + " ~ " + startTime.AddSeconds(Int32.Parse(dr_stage["col_9"].ToString()) - 1).ToString("yyyy/MM/dd HH:mm:ss") + " )");
                        HtmlStr += "<span style='float:right; display:none;' id='div_part_" + dr_stage["col_1"].ToString() + "_1'>✚&nbsp</span>";
                        HtmlStr += "<span style='float:right; display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "_2'>━&nbsp</span>";
                        HtmlStr += "</legend>";
                        HtmlStr += "<div style='display:block;' id='div_part_" + dr_stage["col_1"].ToString() + "'>";
                        if (dr_stage["col_16"].ToString() != "")
                            HtmlStr += "【副本說明】<br>" + dr_stage["col_16"].ToString().Replace("char(10)", "<br>") + "<br>";
                        HtmlStr += "→<a href=\"https://towerofsaviors.fandom.com/wiki/" + dr_stage["col_10"].ToString().Replace(' ', '_') + "\" target=\"blank\">美版維基關卡資訊</a><br>";
                        HtmlStr += "<hr>";

                        var qry_floor = from c in dt_floorList.AsEnumerable()
                                        join o in dt_floorStars.AsEnumerable() on c.Field<string>("col_1") equals o.Field<string>("col_1") into ps
                                        from o in ps.DefaultIfEmpty()
                                        where c.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                        select new
                                        {
                                            floorId = c.Field<string>("col_1"),
                                            floorIcon = c.Field<string>("col_4"),
                                            floorName = c.Field<string>("col_8"),
                                            floorStam = c.Field<string>("col_5"),
                                            floorSpirit = c.Field<string>("col_29"),
                                            floorRound = c.Field<string>("col_6"),
                                            floorStar1 = o == null ? "" : o.Field<string>("col_2"),
                                            floorStar2 = o == null ? "" : o.Field<string>("col_3"),
                                            floorStar3 = o == null ? "" : o.Field<string>("col_4"),
                                            floorLimit = c.Field<string>("col_27"),
                                            floorDrop = c.Field<string>("col_36")
                                        };

                        foreach (var dr_floor in qry_floor)
                        {
                            HtmlStr += "【<a href=\"http://www.tosapp.tw/tos/no." + dr_floor.floorIcon + ".html\" target=\"blank\">" + dr_floor.floorIcon + "</a>】";
                            HtmlStr += "<img width=\"40\" height=\"40\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dr_floor.floorIcon + ".png\">";
                            HtmlStr += "&nbsp";
                            HtmlStr += dr_floor.floorName;
                            HtmlStr += "<br>";
                            HtmlStr += "　體力： " + (dr_floor.floorStam != "0" ? dr_floor.floorStam : (dr_floor.floorSpirit != "0" ? (dr_floor.floorSpirit + " 戰靈") : "0"));
                            HtmlStr += "；戰鬥： " + dr_floor.floorRound;

                            if (dr_floor.floorLimit != "")
                            {
                                Floor f = new Floor();
                                f.limitation = dr_floor.floorLimit;
                                f.UpdateLimitations();
                                string mes = "";
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupA)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupB)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                foreach (KeyValuePair<Floor.Limitation.Type, Floor.Limitation> kvp in f.limitationGroupC)
                                {
                                    mes += "<br>" + kvp.Value.description;
                                }
                                HtmlStr += "<br>【關卡限制】" + mes;
                            }
                            if (dr_floor.floorDrop != "")
                            {
                                string dropList = "";
                                if (dr_floor.floorDrop.IndexOf(',') > 0)
                                    dropList = dr_floor.floorDrop.Split(',')[0];
                                else
                                    dropList = dr_floor.floorDrop;
                                HtmlStr += "<br>";
                                HtmlStr += "　MayDrop： " + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "(★)");
                                HtmlStr += "<br>";
                                HtmlStr += "　<img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/" + dropList.Replace("3@", "").Replace("#0", "").Replace("#1", "").Replace(":", ".png\"><img width=\"20\" height=\"20\" src=\"https://asdfghjklu2000.github.io/Tosapp_Tool/px100/") + ".png\">";
                            }
                            if (dr_floor.floorStar1 != "")
                            {
                                HtmlStr += "<br>★ 成就";
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar1;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar2;
                                HtmlStr += "<br>☆ ";
                                HtmlStr += dr_floor.floorStar3;
                            }
                            HtmlStr += "<hr>";
                        }

                        var qry_starReward = from dr in dt_floorStarRewards.AsEnumerable()
                                             where dr.Field<string>("col_2") == dr_stage["col_1"].ToString()
                                             select dr;
                        if (qry_starReward.Count() > 0)
                            HtmlStr += "<br>✪ 成就獎勵";
                        foreach (DataRow dr_star in qry_starReward)
                        {
                            HtmlStr += "<br>";
                            HtmlStr += dr_star["col_3"].ToString();
                        }
                        HtmlStr += "<br>";
                        HtmlStr += "</div></fieldset>";
                    }
                    HtmlStr += "</div></fieldset>";
                }
            }
            catch (Exception e)
            {
                HtmlStr = "Exception: " + e.Message.ToString();
            }
            HtmlStr += "<br><br>"; //以免被somee的footer給遮蔽
            return HtmlStr;
        }
    }
}
