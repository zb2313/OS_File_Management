using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    //三级索引 文件的物理结构
    //根据文件管理PPT设计
    [Serializable]
    public class FileStructrue//索引结构
    {
        public static int capacity = 10;
        private int[] index = new int[capacity];
        public int usedSize =0;
        private FirstIndex fIndex;//所属于的二级索引
        private SecondIndex sIndex;//所属于的三级索引
        public FileStructrue()
        {

        }
        public bool checkFull() { return capacity <= usedSize; }
        public bool AddIndex(int id)
        {
            if(!checkFull())//index没满
            {
                index[usedSize++] = id;
                if (checkFull())
                {
                    fIndex = new FirstIndex();
                }
            }
            else if(!fIndex.checkFull())//所属于的二级索引没满
            {
                fIndex.AddIndex(id);
                if(fIndex.checkFull())
                {
                    sIndex = new SecondIndex();
                }
            }
            else if(!sIndex.checkFull())//所属于的三级索引没满
            {
                sIndex.AddIndex(id);
            }
            else return false;
            return true;
        }
        public List<int>getSpaceList()//获取全部索引项
        {
            List<int> spaces = new List<int>();
            for(int i=0;i<usedSize;i++)
            {
                spaces.Add(index[i]);
            }
            if (usedSize == capacity)//
            {
                for (int j = 0; j < fIndex.usedSize; j++)
                {
                    spaces.Add(fIndex.index[j]);
                }
            }
            if (fIndex != null && fIndex.checkFull())//二级索引满了看三级索引
            {
                foreach (FirstIndex fIndex in sIndex.fIndex)
                {
                    for (int k = 0; k < fIndex.usedSize; k++)
                    {
                        spaces.Add(fIndex.index[k]);
                    }
                }
            }

            return spaces;
        }
        
    }
    [Serializable]
    public class FirstIndex
    {
        public static int capacity = 128;
        public int[] index = new int[capacity];
        public int usedSize = 0;
        public FirstIndex()
        {

        }
        public void AddIndex(int id)
        {
            index[usedSize++] = id;
        }
        public bool checkFull() { return capacity <= usedSize; }
    }
    [Serializable]
    public class SecondIndex
    {
        public static int capacity = 128;
        public List<FirstIndex>fIndex 
            = new List<FirstIndex> { new FirstIndex() };
        public int usedSize = 0;
        public SecondIndex()
        {

        }
        public bool checkFull() { return capacity <= usedSize; }
        public void AddIndex(int id)
        {
            FirstIndex temp = fIndex[usedSize];
            if(temp.checkFull())
            {
                fIndex.Add(new FirstIndex());
                temp = fIndex[++usedSize];
            }
            temp.AddIndex(id);
        }
    }
}
