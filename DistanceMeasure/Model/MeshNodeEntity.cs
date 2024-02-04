using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMeasure.Model
{
    public class MeshNodeEntity(string name, PhysicalAddress macAddress)
    {
        public string Name { get; set; } = name;
        public PhysicalAddress MacAddress { get; set; } = macAddress;

        public override Boolean Equals(Object? obj)
        {
            return obj is MeshNodeEntity entity &&
                   EqualityComparer<PhysicalAddress>.Default.Equals(MacAddress, entity.MacAddress);
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(MacAddress);
        }
    }
}
