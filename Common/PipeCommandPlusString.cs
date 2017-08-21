using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [Serializable]
    public class PipeCommandPlusString
    {
        public string m_szCommand;
        public string m_szString;

        public PipeCommandPlusString() { }

        public PipeCommandPlusString(string sz, string szString)
        {
            m_szCommand = sz;
            m_szString = szString;
        }

        public string GetCommand()
        {
            return m_szCommand;
        }

        public string GetTransmittedString()
        {
            return m_szString;
        }
    }
}
