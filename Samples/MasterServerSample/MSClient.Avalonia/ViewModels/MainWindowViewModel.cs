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

    }
}
