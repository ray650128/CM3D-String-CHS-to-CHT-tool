using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StringConvertor
{
    public partial class Form1 : Form
    {
        string mScanDir = Application.StartupPath + "\\String";
        string mScanType = "*.txt";
        string mFullPath;
        string[] txtFiles;
        string txtConv;
        string[] split;

        List<string> list = new List<string>();

        private const int LocaleSystemDefault = 0x0800;
        private const int LcmapSimplifiedChinese = 0x02000000;
        private const int LcmapTraditionalChinese = 0x04000000;

        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int LCMapString(int locale, int dwMapFlags, string lpSrcStr, int cchSrc,
                                      [Out] string lpDestStr, int cchDest);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ReloadFileList();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox2.DataSource = null;
            list.Clear();

            mFullPath = mScanDir + "\\" + txtFiles[listBox1.SelectedIndex];

            StreamReader sr = new StreamReader(mFullPath, System.Text.Encoding.UTF8);
            while (sr.Peek() > -1)
            {
                //listBox2.Items.Add(sr.ReadLine());
                list.Add(sr.ReadLine());
            }
            sr.Close();

            listBox2.DataSource = list;
            listBox2.SelectedIndex = 0;
        }

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > 0)
            {
                String tempText = listBox2.SelectedItem.ToString();
                split = tempText.Split(new Char[] { '\t' });
                txtOrig.Text = split[0];
                txtTrans.Text = split[1];
            }
        }

        private void btnTrans_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex > 0)
            {
                txtConv = ToTraditional(txtTrans.Text);
                txtTrans.Text = txtConv;
                list[listBox2.SelectedIndex] = split[0] + '\t' + txtConv;
                listBox2.DataSource = null;
                listBox2.DataSource = list;

                if (listBox2.SelectedIndex >= 0 && listBox2.SelectedIndex < listBox2.Items.Count - 1)
                {
                    listBox2.SelectedIndex++;
                }
            }
        }

        private void btnAllConv_Click(object sender, EventArgs e)
        {
            string[] temp;
            for(int i = 1; i < list.Count; i++)
            {
                temp = list[i].Split(new Char[] { '\t' });
                temp[1] = ToTraditional(temp[1]);
                list[i] = temp[0] + '\t' + temp[1];
            }
            listBox2.DataSource = null;
            listBox2.DataSource = list;
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex <= listBox2.Items.Count && listBox2.SelectedIndex > 0)
            {
                listBox2.SelectedIndex--;
            }
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex >= 0 && listBox2.SelectedIndex < listBox2.Items.Count - 1)
            {
                listBox2.SelectedIndex++;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ReloadFileList();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            FileStream fs = new FileStream(mFullPath, FileMode.Create);

            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.UTF8);

            //開始寫入
            foreach (string line in list)
                sw.WriteLine(line);

            //清空緩衝區
            sw.Flush();
            //關閉流
            sw.Close();
            fs.Close();
        }

        public static string ToTraditional(string argSource)
        {
            var t = new String(' ', argSource.Length);
            LCMapString(LocaleSystemDefault, LcmapTraditionalChinese, argSource, argSource.Length, t, argSource.Length);
            return t;
        }

        private void ReloadFileList()
        {
            txtFiles = System.IO.Directory.GetFiles(mScanDir, @mScanType, System.IO.SearchOption.TopDirectoryOnly); // System.IO.SearchOption.AllDirectories
            for (int i = 0; i < txtFiles.Length; i++)
            {
                txtFiles[i] = txtFiles[i].Replace(mScanDir + "\\", "");
            }
            this.listBox1.Items.Clear();
            this.listBox1.Items.AddRange(txtFiles);
        }
    }
}
