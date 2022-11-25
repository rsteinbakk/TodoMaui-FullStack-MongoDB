using TodoMaui.ViewModels;

namespace TodoMaui.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginPageVM vm)
	{
		InitializeComponent();
        BindingContext = vm;

    }
}