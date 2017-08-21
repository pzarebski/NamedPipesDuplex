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

namespace Client
{
    class Program
    {
        static JavaScriptSerializer serializer;
        static ClientPipe m_pClientPipe;
        static void Main(string[] args)
        {
            m_pClientPipe = new ClientPipe(".", "SQUALL_PIPE");
            m_pClientPipe.ReadDataEvent += PClientPipe_ReadDataEvent;
            m_pClientPipe.PipeClosedEvent += M_pClientPipe_PipeClosedEvent;
            m_pClientPipe.Connect();
            serializer = new JavaScriptSerializer();

            PipeCommandPlusString pCmd = new PipeCommandPlusString("CLIENT", "CONNECT");
            m_pClientPipe.SendCommandAsync(pCmd);
            Thread.Sleep(3000);


            string command = "";
            while ((command = Console.ReadLine()) != "exit")
            {
                m_pClientPipe.SendCommandAsync(new PipeCommandPlusString("CLIENT", command));
            }

            m_pClientPipe.ReadDataEvent -= PClientPipe_ReadDataEvent;
            m_pClientPipe.PipeClosedEvent -= M_pClientPipe_PipeClosedEvent;
            m_pClientPipe.Close();
            m_pClientPipe = null;
        }

        private static void M_pClientPipe_PipeClosedEvent(object sender, EventArgs e)
        {
            // wait around for server to shut us down
        }

        private static void PClientPipe_ReadDataEvent(object sender, PipeEventArgs e)
        {
            byte[] pBytes = e.m_pData;
            string szBytes = Encoding.Default.GetString(pBytes, 0, e.m_nDataLen);
            PipeCommandPlusString pCmd = serializer.Deserialize<PipeCommandPlusString>(szBytes.Trim(new char[] { '\0' }));
            string szValue = pCmd.GetTransmittedString();

            Console.WriteLine("Got command from server: " + pCmd.GetCommand() + "-" + pCmd.GetTransmittedString());

            if (szValue == "CONNECTED")
            {
                PipeCommandPlusString pCmdToSend = new PipeCommandPlusString("CLIENT", "DATA");
                m_pClientPipe.SendCommandAsync(pCmdToSend);
            }
        }
    }
}
