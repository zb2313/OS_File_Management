using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    //内存块管理
    [Serializable]
    public class MemorySpace
    {
        public static int memorySize = 64;//
        private char[] fileData = new char[64];
        private int fileSize = 0;
        
        public MemorySpace()
        {

        }
        public string getData()
        {
            return new string(fileData);
        }
        public void setData(string data)
        {
            fileSize = memorySize > data.Length ? data.Length : memorySize;
            for (int i = 0; i < fileSize; i++)
            {
                this.fileData[i] = data[i];
            }
        }
    }
}
