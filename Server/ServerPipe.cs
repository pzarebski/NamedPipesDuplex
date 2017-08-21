using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class ServerPipe : BasicPipe
    {
        public event EventHandler<EventArgs> GotConnectionEvent;

        NamedPipeServerStream m_pPipe;
        int m_nPipeId;

        public ServerPipe(string szPipeName, int nPipeId)
            : base("Server")
        {
            m_szPipeName = szPipeName;
            m_nPipeId = nPipeId;
            m_pPipe = new NamedPipeServerStream(
                szPipeName,
                PipeDirection.InOut,
                NamedPipeServerStream.MaxAllowedServerInstances,
                PipeTransmissionMode.Message,
                PipeOptions.Asynchronous);
            base.SetPipeStream(m_pPipe);
            m_pPipe.BeginWaitForConnection(new AsyncCallback(StaticGotPipeConnection), this);
        }

        void StaticGotPipeConnection(IAsyncResult pAsyncResult)
        {
            ServerPipe pThis = pAsyncResult.AsyncState as ServerPipe;
            pThis.GotPipeConnection(pAsyncResult);
        }

        void GotPipeConnection(IAsyncResult pAsyncResult)
        {
            m_pPipe.EndWaitForConnection(pAsyncResult);

            Debug.WriteLine("Server Pipe " + m_szPipeName + " got a connection");

            if (GotConnectionEvent != null)
            {
                GotConnectionEvent(this, new EventArgs());
            }

            // lodge the first read request to get us going
            //
            StartReadingAsync();
        }

        public override int PipeId() { return m_nPipeId; }
    }
}
