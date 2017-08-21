using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Server
{
    class Program
    {
        static JavaScriptSerializer serializer;
        static ServerPipe m_pServerPipe;

        static void Main(string[] args)
        {
            m_pServerPipe = new ServerPipe("SQUALL_PIPE", 0);
            m_pServerPipe.ReadDataEvent += M_pServerPipe_ReadDataEvent;
            m_pServerPipe.PipeClosedEvent += M_pServerPipe_PipeClosedEvent;
            serializer = new JavaScriptSerializer();

            while (true)
            {
                Thread.Sleep(1000);
            }
        }

        private static void M_pServerPipe_PipeClosedEvent(object sender, EventArgs e)
        {
            Debug.WriteLine("Server: Pipe was closed, shutting down");
        }

        private static void M_pServerPipe_ReadDataEvent(object sender, PipeEventArgs e)
        {
            // this gets called on an anonymous threade

            byte[] pBytes = e.m_pData;
            string szBytes = Encoding.Default.GetString(pBytes, 0, e.m_pData.Length);
            PipeCommandPlusString pCmd = serializer.Deserialize<PipeCommandPlusString>(szBytes.Trim(new char[] { '\0' }));
            string szValue = pCmd.GetTransmittedString();

            if (szValue == "CONNECT")
            {
                Debug.WriteLine("Got command from client: " + pCmd.GetCommand() + "-" + pCmd.GetTransmittedString() + ", writing command back to client");
                PipeCommandPlusString pCmdToSend = new PipeCommandPlusString("SERVER", "CONNECTED");
                // fire off an async write
                Task t = m_pServerPipe.SendCommandAsync(pCmdToSend);
            }
            else
            {
                Console.WriteLine("Data from client: " + szValue);
            }
        }
    }
}
