using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DistanceMeasure.Model;
using DistanceMeasure.Utils;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace DistanceMeasure.ViewModel
{
    public partial class MainPageViewModel : ObservableObject
    {
        static readonly Int16 PORT = 9876;
        readonly UdpClient udpClient;

        public MainPageViewModel()
        {
            IPEndPoint udpEndPoint = new(IPAddress.Any, 0);
            udpClient = new(udpEndPoint)
            {
                EnableBroadcast = true
            };
            udpClient.BeginReceive(new AsyncCallback(ReceiveMeshInfo), null);
        }

        ~MainPageViewModel()
        {
            udpClient.Close();
        }


        [ObservableProperty]
        ObservableCollection<MeshNetworkEntity> items = [];

        [RelayCommand]
        void SearchForMesh()
        {
            byte[] dataBytes = MessageBuilder.BuildMessage(MessagesEnum.UDP_DISCOVER_REQUEST);

            // Get all network interfaces
            foreach (NetworkInterface networkInterface in NetworkInterface.GetAllNetworkInterfaces())
            {
                 // Get all ipv4 unicast addresses
                foreach (UnicastIPAddressInformation unicastIp in networkInterface.GetIPProperties().UnicastAddresses.Where(x => x.Address.AddressFamily == AddressFamily.InterNetwork))
                {
                    // Get the broadcast address
                    IPAddress iPAddress = GetBroadcastAddress(unicastIp.Address, unicastIp.IPv4Mask);
                    try
                    {
                        // Send the data
                        udpClient.Send(dataBytes, dataBytes.Length, iPAddress.ToString(), PORT);
                    }
                    catch (SocketException e)
                    {
                        // If the network is unreachable, ignore the error
                        // Not all interfaces are connected to a network so some will throw this error
                        if(e.SocketErrorCode != SocketError.NetworkUnreachable)
                        {
                            throw;
                        }
                    }   
                }
            }
        }

        void ReceiveMeshInfo(IAsyncResult res)
        {
            IPEndPoint? remoteEndPoint = new(IPAddress.Any, 0);
            byte[] receivedData = udpClient.EndReceive(res, ref remoteEndPoint);
            udpClient.BeginReceive(new AsyncCallback(ReceiveMeshInfo), null);

            // Check if the endpoint is null, this should never happen
            // But EndReceive expects a non-null endpoint
            if(null == remoteEndPoint)
            {
                throw new System.Exception("Remote endpoint is null");
            }

            var messageType = MessageBuilder.GetMessage<MessagesEnum>(ref receivedData);
            if(messageType != MessagesEnum.UDP_DISCOVER_RESPONSE)
            {
                Debug.WriteLine("Received message is not a UDP_DISCOVER_RESPONSE");
                return;
            }

            byte[] meshIdArray = MessageBuilder.GetMessage<byte[]>(ref receivedData);
            string meshId = BitConverter.ToString(meshIdArray);

            string meshName = MessageBuilder.GetMessage<String>(ref receivedData);  

            MeshNetworkEntity meshNetworkEntity = new(remoteEndPoint.Address, PORT, meshId, meshName);

            if(!Items.Contains(meshNetworkEntity))
            {
                Items.Add(meshNetworkEntity);
            }

        }

        public static IPAddress GetBroadcastAddress(IPAddress address, IPAddress mask)
        {
            uint ipAddress = BitConverter.ToUInt32(address.GetAddressBytes(), 0);
            uint ipMaskV4 = BitConverter.ToUInt32(mask.GetAddressBytes(), 0);
            uint broadCastIpAddress = ipAddress | ~ipMaskV4;

            return new IPAddress(BitConverter.GetBytes(broadCastIpAddress));
        }
    }
}
