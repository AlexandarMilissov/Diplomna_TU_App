using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DistanceMeasure.Model;
using DistanceMeasure.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace DistanceMeasure.ViewModel
{
    public partial class MeshPageViewModel : ObservableObject, IQueryAttributable
    {
        TcpClient? tcpClient;

        public MeshPageViewModel()
        {
            PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(SelectedMesh))
                {
                    DisconnectFromMesh();
                    ConnectToMesh();
                }
            };
        }

        [ObservableProperty]
        ObservableCollection<MeshNodeEntity> meshNodes = [];

        [ObservableProperty]
        MeshNetworkEntity? selectedMesh;
        public void ApplyQueryAttributes(IDictionary<String, Object> query)
        {
            SelectedMesh = (MeshNetworkEntity)query[nameof(MeshNetworkEntity)];
        }

        [RelayCommand]
        void TapTest()
        {
            Debug.WriteLine("Tapped");
        }

        void ConnectToMesh()
        {
            if (SelectedMesh == null)
            {
                return;
            }
            tcpClient = new TcpClient();
            tcpClient.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

            tcpClient.Connect(new(SelectedMesh.IpAddress, SelectedMesh.Port));

            tcpClient.Client.Send(MessageBuilder.BuildMessage(MessagesEnum.TCP_GET_NODES_REQUEST));
            
            ReceiveTcp();
        }
        void DisconnectFromMesh()
        {
            MeshNodes.Clear();
            tcpClient?.Close();
            tcpClient = null;
        }

        async void ReceiveTcp()
        {
            try
            {
                if(tcpClient == null)
                {
                    Debug.WriteLine("TcpClient is null");
                    return;
                }
                while(true)
                {
                    Memory<byte> buffer = new byte[1024];
                    int bytesRead = await tcpClient.GetStream().ReadAsync(buffer);
                    if (bytesRead == 0)
                    {
                        Debug.WriteLine("Connection closed");
                        break;
                    }
                    byte[] data = buffer.ToArray()[..bytesRead];

                    MessagesEnum message = MessageBuilder.GetMessage<MessagesEnum>(ref data);

                    switch (message)
                    {
                        case MessagesEnum.TCP_NODE_CONNECTED:
                            HandleTcpNodeConnected(data);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandleTcpNodeConnected(byte[] data)
        {
            PhysicalAddress physicalAddress = new(MessageBuilder.GetMessage<byte[]>(ref data));
            string name = MessageBuilder.GetMessage<string>(ref data);
            MeshNodes.Add(new(name, physicalAddress));
        }

    }
}
