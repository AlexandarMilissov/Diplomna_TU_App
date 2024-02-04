using DistanceMeasure.View;

namespace DistanceMeasure
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(MeshPage), typeof(MeshPage));
        }
    }
}
