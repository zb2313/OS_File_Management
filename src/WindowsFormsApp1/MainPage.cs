using Microsoft.VisualBasic;
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
using System.Runtime.Serialization.Formatters.Binary;



namespace WindowsFormsApp1
{
    public partial class MainPage : Form
    {
        public FCB root; // 根目录
        public FCB currentContent; // 当前目录
        public static BlankSpace blankSpace; // 全局位图
        public FCBIndex fcbIndex;
        public List<FCB> fcbList = new List<FCB>();
       
        private readonly Stack<FCB> pastStack = new Stack<FCB>(); // 路径栈
        private readonly Stack<FCB> furtStack = new Stack<FCB>(); // 路径栈
        public MainPage()
        {
            InitializeComponent();
            try
            {
                Deserialize();
            }
            catch (FileNotFoundException)
            {
                root = new FCB("我的文件", FCB.fileType.folder);
                fcbIndex = new FCBIndex();
                blankSpace = new BlankSpace();
            }
            currentContent = root;
            InitializeView();
            UpdateView();
        }
        public void Deserialize()
        {
            FileStream fileStream;
            BinaryFormatter b = new BinaryFormatter();
            string dir = Directory.GetCurrentDirectory();

            fileStream = new FileStream(Path.Combine(dir, "root.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            root = b.Deserialize(fileStream) as FCB;
            fileStream.Close();

            fileStream = new FileStream(Path.Combine(dir, "fcbIndex.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            fcbIndex = b.Deserialize(fileStream) as FCBIndex;
            fileStream.Close();

            fileStream = new FileStream(Path.Combine(dir, "blankSpace.dat"), FileMode.Open, FileAccess.Read, FileShare.Read);
            blankSpace = b.Deserialize(fileStream) as BlankSpace;
            fileStream.Close();
        }
        public void InitializeView()
        {
            listView1.Items.Clear();
            treeView1.Nodes.Clear();
            treeView1.Nodes.Add(new TreeNode("我的文件"));
            toolStripStatusLabel1.Text = "系统总空间：" + (BlankSpace.capacity * MemorySpace.memorySize / 1024).ToString() + "KB";
        }
        public void UpdateView()
        {
            comboBox1.Text = currentContent.getPath();

            UpdateTreeView();
            UpdateListView(currentContent);

            int used = blankSpace.usedSpace* MemorySpace.memorySize;
            string usedString = (used > 1024 ? (used / 1024).ToString() + "KB" : used.ToString() + "B");
            toolStripStatusLabel2.Text = "已用空间：" + usedString;

            int rest = (BlankSpace.capacity - blankSpace.usedSpace) * MemorySpace.memorySize;
            string restString = (rest > 1024 ? (rest / 1024).ToString() + "KB" : rest.ToString() + "B");
            toolStripStatusLabel3.Text = "剩余空间：" + restString;
        }
        public void UpdateTreeView()
        {
            treeView1.Nodes.Clear();
            TreeNode rootNode = new TreeNode("我的文件");
            rootNode.Tag = root;
            NodeDFS(rootNode, root);
            rootNode.Expand();
            treeView1.Nodes.Add(rootNode);
        }

        // 更新右边的列表视图
        public void UpdateListView(FCB fcb)
        {
            listView1.Items.Clear();
            FCB child = fcb.child;
            while (child != null)
            {
                ListViewItem listViewItem = new ListViewItem(new string[] {
                        child.fileName,
                        child.fileSize.ToString() + " 字节",
                        (child.type == FCB.fileType.txt ? "文本文件" : "文件夹"),
                        child.editTime.ToString()
                    });
                listViewItem.Tag = child;
                listViewItem.ImageIndex = (child.type == FCB.fileType.folder ? 0 : 1);
                listView1.Items.Add(listViewItem);
                child = child.nextBro;
            }
        }
        // DFS所有目录节点
        private void NodeDFS(TreeNode treeNode, FCB fcb)
        {
            FCB child = fcb.child;
            while (child != null)
            {
                if (child.type == FCB.fileType.folder)
                {
                    TreeNode newNode = new TreeNode(child.fileName, 0, 0);
                    newNode.Tag = child;
                    NodeDFS(newNode, child);
                    newNode.Expand();
                    treeNode.Nodes.Add(newNode);
                }
                else
                {
                    TreeNode newNode = new TreeNode(child.fileName, 1, 1);
                    newNode.Tag = child;
                    treeNode.Nodes.Add(newNode);
                }
                child = child.nextBro;
            }
        }

        // 检查文件是否有重名, 如果有的话返回添加序号后的文件名
        private string NameCheck(string s)
        {
            string[] nameExt = s.Split('.');

            string name = nameExt[0]; // "新建文件夹(1)" "新建文本文件(1)"
            string ext = nameExt.ElementAtOrDefault(1); // "txt"

            int counter = 0;
            FCB child = currentContent.child;
            while (child != null)
            {
                int lastP = child.fileName.LastIndexOf('(');
                if (lastP == -1) lastP = child.fileName.Length;
                string childBaseName = child.fileName.Substring(0, lastP); // "新建文件夹(1)" "新建文本文件(1)"
                if (childBaseName != child.fileName && childBaseName == name)
                {
                    counter++;
                }
                else if (child.fileName == s)
                {
                    counter++;
                }
                child = child.nextBro;
            }
            if (counter > 0)
            {
                name += "(" + counter.ToString() + ")";
            }
            if (ext != null)
            {
                name += "." + ext;
            }
            return name;
        }
        public void MainWindows_Closing(object sender, EventArgs e)
        {
            Serialize();
        }
        public void Serialize()
        {
            FileStream fileStream;
            BinaryFormatter b = new BinaryFormatter();
            string dir = Directory.GetCurrentDirectory();

            fileStream = new FileStream(Path.Combine(dir, "root.dat"), FileMode.Create);
            b.Serialize(fileStream, root);
            fileStream.Close();

            fileStream = new FileStream(Path.Combine(dir, "fcbIndex.dat"), FileMode.Create);
            b.Serialize(fileStream, fcbIndex);
            fileStream.Close();

            fileStream = new FileStream(Path.Combine(dir, "blankSpace.dat"), FileMode.Create);
            b.Serialize(fileStream, blankSpace);
            fileStream.Close();
        }
        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Code by 许之博 (1854062)", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void 打开ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem currentItem;
            if (listView1.SelectedItems.Count != 0)
            {
                currentItem = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请首先选择一个文件或文件夹！");
                return;
            }

            FCB fcb = (FCB)currentItem.Tag;
            fcb.editTime = DateTime.Now;
            Open(fcb);
        }
        private void Open(FCB fcb, bool isBack = false)
        {
            switch (fcb.type)
            {
                case FCB.fileType.folder:
                    (isBack ? furtStack : pastStack).Push(currentContent);
                    currentContent = fcb;
                    comboBox1.Text = currentContent.getPath();
                    UpdateListView(fcb);
                    break;
                case FCB.fileType.txt:
                    FileEditor fileEditor = new FileEditor(fcb);
                    fileEditor.FormClosed += (s, e) => { this.UpdateView(); };
                    fileEditor.Show();
                    break;
                default:
                    break;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (furtStack.Count > 0)
            {
                Open(furtStack.Pop());
                UpdateView();
            }
        }

        private void 剪切ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fcbList.Clear();
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请首先选择一个文件或文件夹！");
                return;
            }
            foreach (ListViewItem currentItem in listView1.SelectedItems)
            {
                FCB fcb = (FCB)currentItem.Tag;
                fcbList.Add(new FCB(fcb));

                if (fcb.blocks != null)
                {
                    List<int> indexs = fcb.blocks.getSpaceList();
                    blankSpace.FreeSpace(indexs);
                }

                fcb.Delete();
                fcbIndex.Delete(fcb);
            }

            UpdateView();
        }

        private void 复制ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fcbList.Clear();
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请首先选择一个文件或文件夹！");
                return;
            }
            foreach (ListViewItem currentItem in listView1.SelectedItems)
            {
                FCB fcb = (FCB)currentItem.Tag;
                fcbList.Add(new FCB(fcb));
            }

            UpdateView();
        }

        private void 粘贴ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (FCB fcb in fcbList)
            {
                FCB newFCB = new FCB(fcb);
                newFCB.fileName = NameCheck(newFCB.fileName);
                newFCB.editTime = DateTime.Now;
                currentContent.addChild(newFCB);
            }
            UpdateView();
        }

        private void 保存ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Serialize();
        }

        private void 文件ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            string file_name = NameCheck("新建文本文件.txt");

            // 创建一个FCB
            FCB fcb = new FCB(file_name, FCB.fileType.txt);
            currentContent.addChild(fcb);

            // 将FCB加入到FCB表中
            fcbIndex.Add(fcb);

            UpdateView();
        }

        private void 文件夹ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string file_name = NameCheck("新建文件夹");

            // 创建一个FCB
            FCB fcb = new FCB(file_name, FCB.fileType.folder);
            currentContent.addChild(fcb);

            // 将FCB加入到FCB表中
            fcbIndex.Add(fcb);

            UpdateView();
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count == 0)
            {
                MessageBox.Show("请首先选择一个文件或文件夹！");
                return;
            }
            foreach (ListViewItem currentItem in listView1.SelectedItems)
            {
                FCB fcb = (FCB)currentItem.Tag;

                if (fcb.blocks != null)
                {
                    List<int> indexs = fcb.blocks.getSpaceList();
                    blankSpace.FreeSpace(indexs);
                }

                fcb.Delete();
                fcbIndex.Delete(fcb);
            }

            UpdateView();
        }

        private void 重命名ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem currentItem;
            if (listView1.SelectedItems.Count != 0)
            {
                currentItem = listView1.SelectedItems[0];
            }
            else
            {
                MessageBox.Show("请首先选择一个文件或文件夹！");
                return;
            }

            FCB fcb = (FCB)currentItem.Tag;

            String newfilename = Interaction.InputBox("请输入文件名", "填写文件名");
            if (newfilename != "")
            {
                fcb.fileName = newfilename;
                UpdateView();
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (pastStack.Count > 0)
            {
                Open(pastStack.Pop(), true);
                UpdateView();
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (currentContent.parent == null)
                return;
            Open(currentContent.parent);
            UpdateView();
        }

        private void 格式化ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            currentContent = root = new FCB("我的文件", FCB.fileType.folder);
            blankSpace = new BlankSpace();
            fcbIndex = new FCBIndex();
            UpdateView();
        }

        private void 帮助ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("可以参见项目文档中1.2操作说明", "帮助", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}
