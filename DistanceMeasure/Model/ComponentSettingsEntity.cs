using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMeasure.Model
{
    public class ComponentSettingsEntity(string name, List<SettingEntity> settings)
    {
        public string Name { get; } = name;

        public List<SettingEntity> Settings { get; } = settings;

        public override Boolean Equals(Object? obj)
        {
            return obj is ComponentSettingsEntity entity &&
                   Name == entity.Name;
        }

        public override Int32 GetHashCode()
        {
            return HashCode.Combine(Name);
        }
    }
}
