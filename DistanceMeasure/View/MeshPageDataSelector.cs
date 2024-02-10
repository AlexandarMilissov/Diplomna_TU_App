using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DistanceMeasure.View
{
    class MeshPageDataSelector : DataTemplateSelector
    {
        public required DataTemplate ConnectedModesDateTemplate { get; set; }
        public required DataTemplate GlobalMeshSesttingsDataTemplate { get; set; }


        protected override DataTemplate OnSelectTemplate(Object item, BindableObject container)
        {
            string identifier = (string)item;

            return identifier switch
            {
                "ConnectedNodes" => ConnectedModesDateTemplate,
                "GlobalMeshSettings" => GlobalMeshSesttingsDataTemplate,
                _ => throw new ArgumentException("Invalid identifier"),
            };
        }
    }
}
