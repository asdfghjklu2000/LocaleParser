using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LocaleParser
{
    public partial class FrmFrameChooser : Form
    {
        public string ReturnValue1 { get; set; }
        public Bitmap pic;
        public FrmFrameChooser()
        {
            this.ReturnValue1 = "HowDoYouTurnThisOn";
            InitializeComponent();
        }

        public FrmFrameChooser(Bitmap _pic)
        {
            this.ReturnValue1 = "HowDoYouTurnThisOn";
            pic = _pic;
            InitializeComponent();
        }

        private Bitmap MergeIcon(Bitmap b_Frame,Bitmap b_Icon)
        {
            //來源圖片，大小512 x 512
            int picWidth = 160;
            int picHeight = 160;
            
            Bitmap img_cut, img_50px;
            Graphics gpc;

            img_cut = new Bitmap(picWidth, picHeight, PixelFormat.Format32bppArgb);
            img_cut.SetResolution(72.0f, 72.0f);
            gpc = Graphics.FromImage(img_cut);
            //建立畫板
            gpc.DrawImage(b_Icon,
                     //將被切割的圖片畫在新圖片上面，第一個參數是被切割的原圖片
                     new Rectangle(0, 0, picWidth, picHeight),
                     //指定繪製影像的位置和大小，基本上是同pic大小
                     new Rectangle(0, 0, picWidth, picHeight),
                     //指定被切割的圖片要繪製的部分
                     GraphicsUnit.Pixel);
            //測量單位，這邊是pixel            
            
            gpc.DrawImage(b_Frame,
                    //將被切割的圖片畫在新圖片上面，第一個參數是被切割的原圖片
                    new Rectangle(0, 0, picWidth, picHeight),
                    //指定繪製影像的位置和大小，基本上是同pic大小
                    new Rectangle(0, 0, picWidth, picHeight),
                    //指定被切割的圖片要繪製的部分
                    GraphicsUnit.Pixel);

            img_50px = new Bitmap(img_cut, 50, 50);
            img_50px.SetResolution(72.0f, 72.0f);
            //img_50px.Save(System.Windows.Forms.Application.StartupPath + "\\CutPng\\" + timeStamp + "\\" + cut_name + ".png", System.Drawing.Imaging.ImageFormat.Png);
            //img_50px.Dispose();
            
            gpc.Dispose();
            img_cut.Dispose();
            return img_50px;
        }

        private void FrmFrameChooser_Load(object sender, EventArgs e)
        {
            DirectoryInfo d = new DirectoryInfo(Application.StartupPath + "\\Frame\\");//Getting Frame Dir
            FileInfo[] Files = d.GetFiles("*.png"); //Getting Png files
            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(50, 50);

            foreach (FileInfo file in Files)
            {
                string key = "";
                key = file.Name.Replace(".png", "");
                // create image list and fill it 
                Bitmap image = new Bitmap(Application.StartupPath + "\\Frame\\" + file.Name);
                
                imageList.Images.Add(key, MergeIcon(image,pic));                
                // tell your ListView to use the new image list
                listView1.LargeImageList = imageList;
                // add an item
                ListViewItem listViewItem = listView1.Items.Add(key);
                // and tell the item which image to use
                listViewItem.ImageKey = key;
                //str = str + ", " + file.Name;
            }

            //// create image list and fill it 
            //Bitmap image = new Bitmap(Application.StartupPath + "\\Frame\\0_1.png");
            //ImageList imageList = new ImageList();
            //imageList.Images.Add("itemImageKey", image);
            //imageList.ImageSize = new Size(160, 160);
            //// tell your ListView to use the new image list
            //listView1.LargeImageList = imageList;
            //// add an item
            //ListViewItem listViewItem = listView1.Items.Add("Item with image");
            //// and tell the item which image to use
            //listViewItem.ImageKey = "itemImageKey";
        }

        private void btn_choose_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ListViewItem listViewItem = listView1.SelectedItems[0];
                //System.Windows.Forms.MessageBox.Show(listViewItem.ImageKey);
                this.ReturnValue1 = listViewItem.ImageKey;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.ReturnValue1 = "HowDoYouTurnThisOn";
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }
    }
}
