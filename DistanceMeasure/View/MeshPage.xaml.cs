using DistanceMeasure.ViewModel;

namespace DistanceMeasure.View;

public partial class MeshPage : ContentPage
{
	public MeshPage(MeshPageViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}