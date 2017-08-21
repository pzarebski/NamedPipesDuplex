using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class PipeEventArgs
    {
        public byte[] m_pData;
        public int m_nDataLen;

        public PipeEventArgs(byte[] pData, int nDataLen)
        {
            m_pData = pData;
            m_nDataLen = nDataLen;
        }
    }
}
