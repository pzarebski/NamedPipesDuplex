using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO.Pipes;
using System.Web.Script.Serialization;

namespace Common
{
    public abstract class BasicPipe : PipeSender
    {
        public static int MaxLen = 1024 * 1024; // why not
        protected string m_szPipeName;
        protected string m_szDebugPipeName;
        protected JavaScriptSerializer serializer;

        public event EventHandler<PipeEventArgs> ReadDataEvent;
        public event EventHandler<EventArgs> PipeClosedEvent;

        protected byte[] m_pPipeBuffer = new byte[BasicPipe.MaxLen];

        PipeStream m_pPipeStream;

        public BasicPipe(string szDebugPipeName)
        {
            m_szDebugPipeName = szDebugPipeName;
            serializer = new JavaScriptSerializer();
        }

        protected void SetPipeStream(PipeStream p)
        {
            m_pPipeStream = p;
        }

        protected string FullPipeNameDebug()
        {
            return m_szDebugPipeName + "-" + m_szPipeName;
        }

        public abstract int PipeId();

        public void Close()
        {
            m_pPipeStream.WaitForPipeDrain();
            m_pPipeStream.Close();
            m_pPipeStream.Dispose();
            m_pPipeStream = null;
        }

        // called when Server pipe gets a connection, or when Client pipe is created
        public void StartReadingAsync()
        {
            Debug.WriteLine("Pipe " + FullPipeNameDebug() + " calling ReadAsync");

            // okay we're connected, now immediately listen for incoming buffers
            //
            byte[] pBuffer = new byte[MaxLen];
            m_pPipeStream.ReadAsync(pBuffer, 0, MaxLen).ContinueWith(t =>
            {
                Debug.WriteLine("Pipe " + FullPipeNameDebug() + " finished a read request");

                int ReadLen = t.Result;
                if (ReadLen == 0)
                {
                    Debug.WriteLine("Got a null read length, remote pipe was closed");
                    if (PipeClosedEvent != null)
                    {
                        PipeClosedEvent(this, new EventArgs());
                    }
                    return;
                }

                if (ReadDataEvent != null)
                {
                    ReadDataEvent(this, new PipeEventArgs(pBuffer, ReadLen));
                }
                else
                {
                    Debug.Assert(false, "something happened");
                }

                // lodge ANOTHER read request
                //
                StartReadingAsync();

            });
        }

        protected Task WriteByteArray(byte[] pBytes)
        {
            // this will start writing, but does it copy the memory before returning?
            return m_pPipeStream.WriteAsync(pBytes, 0, pBytes.Length);
        }

        public Task SendCommandAsync(PipeCommandPlusString pCmd)
        {
            Debug.WriteLine("Pipe " + FullPipeNameDebug() + ", writing " + pCmd.GetCommand() + "-" + pCmd.GetTransmittedString());
            string szSerializedCmd = serializer.Serialize(pCmd);
            byte[] pSerializedCmd = Encoding.Default.GetBytes(szSerializedCmd);
            Task t = WriteByteArray(pSerializedCmd);
            return t;
        }
    }
}
