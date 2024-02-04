using CommunityToolkit.Mvvm.ComponentModel;
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

        void ConnectToMesh()
        {
            if (SelectedMesh == null)
            {
                return;
            }
            IPEndPoint meshEndPoint = new(SelectedMesh.IpAddress, SelectedMesh.Port);
            tcpClient = new TcpClient(meshEndPoint);


            MeshNodes.Add(new MeshNodeEntity("Test Node", new System.Net.NetworkInformation.PhysicalAddress([0x00, 0x00, 0x00, 0x00, 0x00, 0x00])));
            MeshNodes.Add(new MeshNodeEntity("Test Node 2", new System.Net.NetworkInformation.PhysicalAddress([0x00, 0x00, 0x00, 0x00, 0x00, 0x01])));
            MeshNodes.Add(new MeshNodeEntity("Test Node 3", new System.Net.NetworkInformation.PhysicalAddress([0x00, 0x00, 0x00, 0x00, 0x00, 0x02])));
        }
        void DisconnectFromMesh()
        {
            MeshNodes.Clear();
            tcpClient?.Close();
            tcpClient = null;
        }
    }
}
