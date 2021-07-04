using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    public class BlankSpace//管理空白区
    {
        public int usedSpace = 0;//已经使用的空间
        public static int capacity = 10 * 1024;//10k空间
        private bool[] bitMap = new bool[capacity];
        private MemorySpace[] spaces = new MemorySpace[capacity];

        public BlankSpace()
        {
            for(int i=0;i<capacity;i++)
            {
                bitMap[i] = true;
            }
        }
        public string getDataFromMemory(int i)//从内存获取数据
        {
            return spaces[i].getData();
        }
        public void FreeSpace(int id)
        {
            bitMap[id] = true;
        }
        public void FreeSpace(List<int>ids)
        {
            foreach(int i in ids)
            {
                bitMap[i] = true;
            }
        }
        public FileStructrue CreateSpaces(string data)
        {
            FileStructrue table = new FileStructrue();
            while (data.Count() > MemorySpace.memorySize)//若超出内存块范围
            {
                table.AddIndex(
                    newSpace(data.Substring(0, MemorySpace.memorySize)));
                data = data.Remove(0, MemorySpace.memorySize);
            }
            table.AddIndex(newSpace(data));//直接置入内存块

            return table;
        }
        public int newSpace(string data)
        {
            usedSpace%= capacity;
            int p = usedSpace;
            while (true)
            {
                if (bitMap[p])
                {
                    spaces[p] = new MemorySpace();
                    spaces[p].setData(data);
                    usedSpace = p + 1;
                    return p;
                }
                else
                {
                    p += 1;
                    p %= capacity;
                }
                if (p == usedSpace)break;
            }
            return -1;
        }
    }
}
