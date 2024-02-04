using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMeasure.Model
{
    public class MeshNetworkEntity(IPAddress ipAddress, int port, string meshId, string meshServerName)
    {
        public IPAddress IpAddress { get; set; } = ipAddress;
        public int Port { get; set; } = port;
        public string MeshId { get; set; } = meshId;
        public string MeshServerName { get; set; } = meshServerName;

        public override Boolean Equals(Object? obj)
        {
            return obj is MeshNetworkEntity entity &&
                   EqualityComparer<IPAddress>.Default.Equals(IpAddress, entity.IpAddress) &&
                   Port == entity.Port &&
                   MeshId == entity.MeshId &&
                   MeshServerName == entity.MeshServerName;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(IpAddress, Port, MeshId, MeshServerName);
        }
    }
}
