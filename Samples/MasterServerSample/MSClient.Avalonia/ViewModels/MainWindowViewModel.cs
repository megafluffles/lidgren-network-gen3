using Avalonia.Controls;
using Lidgren.Network;
using MSCommon;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Input;

namespace MSClient.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public ICommand GetHostListCommand { get; set; }

        public string MasterServerAddress { get; set; }

        private static NetClient m_client;
        private static IPEndPoint m_masterServer;
        private static Dictionary<long, IPEndPoint[]> m_hostList;

        public MainWindowViewModel()
        {
            MasterServerAddress = "localhost";
            GetHostListCommand = ReactiveUI.ReactiveCommand.Create(GetHostList);

            m_hostList = new Dictionary<long, IPEndPoint[]>();

            NetPeerConfiguration config = new NetPeerConfiguration("game");
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            m_client = new NetClient(config);
            m_client.Start();
        }

        private void GetHostList()
        {
            GetServerList(MasterServerAddress);
        }

        private static void GetServerList(string masterServerAddress)
        {
            //
            // Send request for server list to master server
            //
            m_masterServer = new IPEndPoint(NetUtility.Resolve(masterServerAddress), CommonConstants.MasterServerPort);

            NetOutgoingMessage listRequest = m_client.CreateMessage();
            listRequest.Write((byte)MasterServerMessageType.RequestHostList);
            m_client.SendUnconnectedMessage(listRequest, m_masterServer);
        }

		static void AppIdle(object sender, EventArgs e)
		{
            NetIncomingMessage inc;
            while ((inc = m_client.ReadMessage()) != null)
            {
                switch (inc.MessageType)
                {
                    case NetIncomingMessageType.VerboseDebugMessage:
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.ErrorMessage:
                        throw new NotImplementedException();
                        //NativeMethods.AppendText(m_mainForm.richTextBox1, inc.ReadString());
                        //break;
                    case NetIncomingMessageType.UnconnectedData:
                        if (inc.SenderEndPoint.Equals(m_masterServer))
                        {
                            // it's from the master server - must be a host
                            var id = inc.ReadInt64();
                            var hostInternal = inc.ReadIPEndPoint();
                            var hostExternal = inc.ReadIPEndPoint();

                            m_hostList[id] = new IPEndPoint[] { hostInternal, hostExternal };

                            // update combo box
                            throw new NotImplementedException();
                            // m_mainForm.comboBox1.Items.Clear();
                            // foreach (var kvp in m_hostList)
                            // 	m_mainForm.comboBox1.Items.Add(kvp.Key.ToString() + " (" + kvp.Value[1] + ")");
                        }
                        break;
                    case NetIncomingMessageType.NatIntroductionSuccess:
                        string token = inc.ReadString();
                        throw new NotImplementedException();
                        //MessageBox.Show("Nat introduction success to " + inc.SenderEndPoint + " token is: " + token);
                        //break;
                }
            }
		}
    }
}
