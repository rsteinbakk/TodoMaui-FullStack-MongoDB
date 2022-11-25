using TodoMaui.Views;

namespace TodoMaui;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
        Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
        Routing.RegisterRoute(nameof(Dashboard), typeof(Dashboard));

    }
}
