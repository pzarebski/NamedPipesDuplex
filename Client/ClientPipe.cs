using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class ClientPipe : BasicPipe
    {
        NamedPipeClientStream m_pPipe;

        public ClientPipe(string szServerName, string szPipeName)
            : base("Client")
        {
            m_szPipeName = szPipeName; // debugging
            m_pPipe = new NamedPipeClientStream(szServerName, szPipeName, PipeDirection.InOut, PipeOptions.Asynchronous);
            base.SetPipeStream(m_pPipe); // inform base class what to read/write from
        }

        public void Connect()
        {
            Debug.WriteLine("Pipe " + FullPipeNameDebug() + " connecting to server");
            m_pPipe.Connect(); // doesn't seem to be an async method for this routine. just a timeout.
            StartReadingAsync();
        }

        // the client's pipe index is always 0
        public override int PipeId() { return 0; }
    }
}
