using DistanceMeasure.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMeasure.Model
{
    public class SettingEntity(string name, UInt64 checksum, ValueEnum value, byte[] data, bool saveToNvs, bool updateRequired)
    {
        public string Name { get; } = name;

        public UInt64 Checksum { get; } = checksum;

        public ValueEnum Value { get; } = value;

        public byte[] Data { get; } = data;

        public bool SaveToNvs { get; } = saveToNvs;

        public bool UpdateRequired { get; } = updateRequired;

        public override Boolean Equals(Object? obj)
        {
            return obj is SettingEntity entity &&
                   Name == entity.Name;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
