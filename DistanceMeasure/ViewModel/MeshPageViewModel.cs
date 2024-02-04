using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DistanceMeasure.Model;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

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

            if (tcpClient == null )
            {
                return;
            }
            char[] charData = ['h', 'e', 'l', 'l', 'o'];
            byte[] data = new byte[charData.Length];
            for (int i = 0; i < charData.Length; i++)
            {
                data[i] = (byte)charData[i];
            }
            tcpClient.Client.Send(data);
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

            MeshNodes.Add(new MeshNodeEntity("Test Node  ", new System.Net.NetworkInformation.PhysicalAddress([0x00, 0x00, 0x00, 0x00, 0x00, 0x00])));
            MeshNodes.Add(new MeshNodeEntity("Test Node 2", new System.Net.NetworkInformation.PhysicalAddress([0x00, 0x00, 0x00, 0x00, 0x00, 0x01])));
            MeshNodes.Add(new MeshNodeEntity("Test Node 3", new System.Net.NetworkInformation.PhysicalAddress([0x00, 0x00, 0x00, 0x00, 0x00, 0x02])));
        
            TapTest();
        }
        void DisconnectFromMesh()
        {
            MeshNodes.Clear();
            tcpClient?.Close();
            tcpClient = null;
        }
    }
}
