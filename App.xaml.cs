namespace JustQR;

public partial class App : Application {
	public App() {
		InitializeComponent();

		MainPage = new AppShell();
	}

	protected override Window CreateWindow(IActivationState? activationState) {
		var window = base.CreateWindow(activationState);
		
		window.MinimumWidth = 350;
		window.MinimumHeight = 450;

		window.Width = 400;
		window.Height = 700;
		window.X = 10;
		window.Y = 10;

		return window;
	}
}