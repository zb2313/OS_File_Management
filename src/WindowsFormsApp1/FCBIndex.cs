using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp1
{
    [Serializable]
    public class FCBIndex//管理FCB
    {
        public Dictionary<int, FCB> fileIndex = new Dictionary<int, FCB>();

        public FCB getFCB(int num)
        {
            if (fileIndex.ContainsKey(num))
            {
                return fileIndex[num];
            }
            return null;
        }
        public void Delete(FCB fCB)
        {
            fileIndex.Remove(fCB.fileID);
        }

        public void Add(FCB fCB)
        {
            fileIndex[fCB.fileID] = fCB;
        }
    }
}
