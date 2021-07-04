using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class FileEditor : Form
    {
        private FCB fCB;
        private BlankSpace blankSpace = MainPage.blankSpace;
        private bool dirt = false;
        public FileEditor()
        {
            InitializeComponent();
        }
        public FileEditor(FCB fcb)
        {
            InitializeComponent();
            this.fCB = fcb;
            this.Text = fcb.fileName;
            showTexts();
        }
        private void showTexts()
        {
            List<int> indexs = fCB.blocks.getSpaceList();
            string content = "";
            foreach (int i in indexs)
            {
                content += blankSpace.getDataFromMemory(i);
            }
            richTextBox1.Text = content;
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {

        }

        private void 操作ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }



        private void File_Load(object sender, EventArgs e)
        {

        }
        private void saveToMemory()
        {
            string content = richTextBox1.Text;
            fCB.fileSize = content.Length;

            // 先清空原来的所有数据块
            List<int> indexs = fCB.blocks.getSpaceList();
            blankSpace.FreeSpace(indexs);

            // 重新写入
            fCB.blocks = blankSpace.CreateSpaces(content);
        }
        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveToMemory();
            dirt = false;
            this.Text = fCB.fileName;
        }

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Code by 许之博 (1854062)", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 退出ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Editor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (dirt && MessageBox.Show("是否保存文件？", "提示", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                saveToMemory();
            }
        }
    }
}
