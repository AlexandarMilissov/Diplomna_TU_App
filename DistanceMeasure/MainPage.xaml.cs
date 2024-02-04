using DistanceMeasure.ViewModel;
using System.Net.Sockets;

namespace DistanceMeasure
{
    public partial class MainPage : ContentPage
    {
        public MainPage(MainPageViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
