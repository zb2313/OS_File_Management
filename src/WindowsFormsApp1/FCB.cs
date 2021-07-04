using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    public class FCB//文件的目录结构
    {
        //文件类型
        //文件ID
        //文件名称
        //修改时间
        //
        public int fileID;//文件编号
        public string fileName;//文件名称
        public DateTime editTime;//修改时间
        public int fileSize;//文件大小
        public enum fileType {txt,folder};//文件类型
        public fileType type;
        public int fileNum=0;//如果是文件夹，包含的文件个数
        public FileStructrue blocks;//文件物理结构

        public FCB parent = null, child = null;//父目录指针和子目录指针
        public FCB nextBro=null,preBro=null;//左右兄弟文件指针

        public FCB(FCB fCB)//复制文件
        {
            this.fileID = fileNum++;
            this.fileName = fCB.fileName;
            this.type = fCB.type;
            this.fileSize = fCB.fileSize;
            this.editTime = DateTime.Now;

            if(fCB.type==fileType.folder)
            {
                FCB ch= fCB.child;
                while(ch!=null)
                {
                    this.addChild(new FCB(ch));
                    ch = ch.nextBro;
                }
            }
            else
            {
                this.blocks = new FileStructrue();
                List<int> ids = fCB.blocks.getSpaceList();
                string content = "";
                foreach(int id in ids)
                {
                    content += MainPage.blankSpace.getDataFromMemory(id);
                }
                this.blocks = MainPage.blankSpace.CreateSpaces(content);
            }
        }
        public FCB(string name,fileType tp)
        {
            this.fileID = fileNum++;
            this.fileName = name;
            this.type = tp;
            this.fileSize = 0;
            this.editTime = DateTime.Now;
            if(tp==fileType.txt)
            {
                this.blocks = new FileStructrue();
            }
        }
       
        
        public void Delete()
        {
            if (parent != null && parent.child == this)
            {
                parent.child = nextBro;
            }
            else if (preBro != null)
            {
                preBro.nextBro = nextBro;
            }
        }
        public void addChild(FCB child)
        {
            if (this.child == null)
            {
                this.child = child;
                child.parent = this;
                
            }
            else
            {
                FCB p = this.child;
                while (p.nextBro != null)
                {
                    p = p.nextBro;
                }
                p.nextBro = child;
                child.preBro = p;
                child.parent = this;
            }
        }
        public string getPath()
        {
            string path = this.fileName;
            FCB p = this.parent;
            while(p!=null)
            {
                path = p.fileName + (p.parent == null ? "://" : "/") + path;
                p = p.parent;            
            }
            return path;
        }
    }
}
