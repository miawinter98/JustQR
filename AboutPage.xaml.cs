namespace JustQR;

public partial class AboutPage {
	public AboutPage() {
		InitializeComponent();
	}

	private async void LearnMore_Clicked(object sender, EventArgs e) {
		await Launcher.Default.OpenAsync("https://github.com/miawinter98/JustQR");
	}

	private async void LearnMore_QRCoder_Clicked(object? sender, EventArgs e) {
		await Launcher.Default.OpenAsync("https://github.com/codebude/QRCoder");
	}
}