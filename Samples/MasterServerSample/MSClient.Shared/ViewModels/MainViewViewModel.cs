using Avalonia.Controls;
using Avalonia.Threading;
using Lidgren.Network;
using MSCommon;
using ReactiveUI;
using ReactiveUI.Legacy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MSClient.ViewModels
{
    public class MainViewViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Task> GetHostListCommand { get; set; }

        public ICommand RequestPunchThruCommand { get; set; }

        public string MasterServerAddress { get => _masterServerAddress; set { this.RaiseAndSetIfChanged(ref _masterServerAddress, value); } }

        public IReactiveList<string> Hosts { get => _hosts; set { this.RaiseAndSetIfChanged(ref _hosts, value); } }

        public string Output { get => _output; set { this.RaiseAndSetIfChanged(ref _output, value); } }

        public int SelectedHostIndex { get => _selectedHostIndex; set { this.RaiseAndSetIfChanged(ref _selectedHostIndex, value); } }

        public ViewModelActivator Activator { get; }

        private static NetClient m_client;
        private static IPEndPoint m_masterServer;
        private static Dictionary<long, IPEndPoint[]> m_hostList;
        private string _masterServerAddress;
        private bool _readingMessages;
        private IReactiveList<string> _hosts;
        private string _output;
        private int _selectedHostIndex;

        public MainViewViewModel()
        {
            MasterServerAddress = "localhost";
            Hosts = new ReactiveList<string>();
            Hosts.Add(" ");
            SelectedHostIndex = 0;

            GetHostListCommand = ReactiveCommand.CreateFromObservable(GetHostList);
            RequestPunchThruCommand = ReactiveUI.ReactiveCommand.Create(RequestPunchThru);

            m_hostList = new Dictionary<long, IPEndPoint[]>();

            NetPeerConfiguration config = new NetPeerConfiguration("game");
            config.EnableMessageType(NetIncomingMessageType.UnconnectedData);
            config.EnableMessageType(NetIncomingMessageType.NatIntroductionSuccess);
            m_client = new NetClient(config);
            m_client.Start();
        }

        private async Task ReadMessages()
        {
            NetIncomingMessage inc;

            while (true)
            {
                while ((inc = m_client.ReadMessage()) != null)
                {
                    switch (inc.MessageType)
                    {
                        case NetIncomingMessageType.VerboseDebugMessage:
                        case NetIncomingMessageType.DebugMessage:
                        case NetIncomingMessageType.WarningMessage:
                        case NetIncomingMessageType.ErrorMessage:
                            // throw new NotImplementedException();
                            //NativeMethods.AppendText(m_mainForm.richTextBox1, inc.ReadString());
                            break;
                        case NetIncomingMessageType.UnconnectedData:
                            if (inc.SenderEndPoint.Equals(m_masterServer))
                            {
                                // it's from the master server - must be a host
                                var id = inc.ReadInt64();
                                var hostInternal = inc.ReadIPEndPoint();
                                var hostExternal = inc.ReadIPEndPoint();

                                m_hostList[id] = new IPEndPoint[] { hostInternal, hostExternal };

                                await Dispatcher.UIThread.InvokeAsync(() =>
                                {
                                    Hosts.Clear();
                                    foreach (var kvp in m_hostList)
                                    {
                                        Hosts.Add(kvp.Key.ToString() + " (" + kvp.Value[1] + ")");
                                    }
                                });
                            }
                            break;
                        case NetIncomingMessageType.NatIntroductionSuccess:
                            string token = inc.ReadString();
                            Output += $"Nat introduction success to {inc.SenderEndPoint} token is: {token}\n";
                            break;
                    }
                }

                System.Threading.Thread.Sleep(200);
            }
        }

        private IObservable<Task> GetHostList()
        {
            return Observable.Start(() => GetServerList(MasterServerAddress));
        }

        private void RequestPunchThru()
        {
            if (SelectedHostIndex > -1 && !string.IsNullOrEmpty(Hosts[SelectedHostIndex].Trim()))
            {
                var splits = Hosts[SelectedHostIndex].ToString().Split(' ');
                var host = long.Parse(splits[0]);

                RequestNATIntroduction(host);
            }
        }

        public static void RequestNATIntroduction(long hostid)
        {
            if (hostid == 0)
            {
                //MessageBox.Show("Select a host in the list first");
                throw new NotImplementedException();
                return;
            }

            if (m_masterServer == null)
                throw new Exception("Must connect to master server first!");

            NetOutgoingMessage om = m_client.CreateMessage();
            om.Write((byte)MasterServerMessageType.RequestIntroduction);

            // write my internal ipendpoint
            IPAddress mask;
            om.Write(new IPEndPoint(NetUtility.GetMyAddress(out mask), m_client.Port));

            // write requested host id
            om.Write(hostid);

            // write token
            om.Write("mytoken");

            m_client.SendUnconnectedMessage(om, m_masterServer);
        }
        private async Task GetServerList(string masterServerAddress)
        {
            //
            // Send request for server list to master server
            //
            m_masterServer = new IPEndPoint(NetUtility.Resolve(masterServerAddress), CommonConstants.MasterServerPort);

            NetOutgoingMessage listRequest = m_client.CreateMessage();
            listRequest.Write((byte)MasterServerMessageType.RequestHostList);
            m_client.SendUnconnectedMessage(listRequest, m_masterServer);

            if (!_readingMessages)
            {
                _readingMessages = true;

                await ReadMessages();
            }
        }
    }
}
