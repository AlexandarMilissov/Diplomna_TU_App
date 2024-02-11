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
        ObservableCollection<ComponentSettingsEntity> meshSettings = [];

        [ObservableProperty]
        MeshNetworkEntity? selectedMesh;
        public void ApplyQueryAttributes(IDictionary<String, Object> query)
        {
            SelectedMesh = (MeshNetworkEntity)query[nameof(MeshNetworkEntity)];
        }

        [RelayCommand]
        void GetAllNodes()
        {
            if (tcpClient == null)
            {
                return;
            }
            MeshNodes.Clear();
            _ = tcpClient.Client.Send(MessageBuilder.BuildMessage(MessagesEnum.TCP_GET_NODES_REQUEST));
        }

        [RelayCommand]
        void GetAllGlobalSettings()
        {
            if (tcpClient == null)
            {
                return;
            }
            MeshSettings.Clear();
            _ = tcpClient.Client.Send(MessageBuilder.BuildMessage(MessagesEnum.TCP_GLOBAL_OPTIONS_REQUEST));
        }

        [RelayCommand]
        void OpenNode()
        {
            MeshNodes.Add(new("Test", new([0,0,0,0,0,0])));  
        }

        [RelayCommand]
        void OpenSetting()
        {
            SettingEntity setting = new("Test", 0, ValueEnum.UINT32, [0,0,0,0], false, false);
            MeshSettings.Add(new("Test", [setting]));
        }

        [RelayCommand]
        void BackButton()
        {
            DisconnectFromMesh();
            Shell.Current.GoToAsync("..");
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
            ReceiveTcp();

            GetAllNodes();
            GetAllGlobalSettings();
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
                        // TODO: POPUP and close
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
                        case MessagesEnum.TCP_GLOBAL_OPTIONS_RESPONSE:
                            HandleTcpGlobalOptionsResponse(data);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch (System.IO.IOException e)
            {
                if(e.InnerException is SocketException socketException &&
                    socketException.SocketErrorCode == SocketError.OperationAborted)
                {
                    Debug.WriteLine("Connection reset");
                }
                else
                {
                    Debug.WriteLine(e.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
        }

        void HandleTcpGlobalOptionsResponse(byte[] data)
        {
            Debug.WriteLine("HandleTcpGlobalOptionsResponse");

            string componentName = MessageBuilder.GetMessage<string>(ref data);
            List<SettingEntity> settings = [];

            while (data.Length > 0)
            {
                string settingName  = MessageBuilder.GetMessage<string>     (ref data);
                UInt64 checksum     = MessageBuilder.GetMessage<UInt64>     (ref data);
                ValueEnum value     = MessageBuilder.GetMessage<ValueEnum>  (ref data);
                byte[] settingData  = MessageBuilder.GetMessage<byte[]>     (ref data);
                bool saveToNvs      = MessageBuilder.GetMessage<Boolean>    (ref data);
                bool updateRequired = MessageBuilder.GetMessage<Boolean>    (ref data);

                settings.Add(new(settingName, checksum, value, settingData, saveToNvs, updateRequired));
            }

            ComponentSettingsEntity componentSettingsEntity = new(componentName, settings);

            var itemIndex = MeshSettings.IndexOf(componentSettingsEntity);
            if( itemIndex == -1)
            {
                MeshSettings.Add(componentSettingsEntity);
            }
            else
            {
                MeshSettings[itemIndex] = componentSettingsEntity;
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
