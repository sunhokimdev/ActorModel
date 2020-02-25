using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamLibrary
{
    public sealed class LocalData
    {
        public LocalData(int id, int cpuCount, int memCount, int diskCount, int netCount)
        {
            LocalID = id;
            CPUValue = cpuCount;
            MEMValue = memCount;
            DiskValue = diskCount;
            NetworkValue = netCount;
        }
        public LocalData()
        {
        }
        public int LocalID
        {
            get; private set;
        }
        public int CPUValue
        {
            get; private set;
        }
        public int MEMValue
        {
            get; private set;
        }
        public int DiskValue
        {
            get; private set;
        }
        public int NetworkValue
        {
            get; private set;
        }
        public int TotalValue
        {
            get
            {
                return CPUValue + MEMValue + DiskValue + NetworkValue;
            }
        }
    }
}
