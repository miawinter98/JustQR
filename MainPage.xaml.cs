using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using QRCoder;
using Color = System.Drawing.Color;

namespace JustQR;

public partial class MainPage {
	public MainPage() {
		InitializeComponent();
	}

	private async Task UpdateQrCode() {
		var qrGenerator = new QRCodeGenerator();
		var data = qrGenerator.CreateQrCode(TextEditor.Text, QRCodeGenerator.ECCLevel.Default);
		var code = new PngByteQRCode(data);
		string file = Path.GetTempFileName() + ".png";
		await File.WriteAllBytesAsync(file, code.GetGraphic(20, Color.Black, Color.White));
		QrImage.Source = ImageSource.FromFile(file);
	}

	private async void TextEditor_OnTextChanged(object? sender, TextChangedEventArgs e) {
		await UpdateQrCode();
	}

	private async void Button_OnClicked(object? sender, EventArgs e) {
		var source = QrImage.Source as FileImageSource;
		if (source is null || Path.IsPathRooted(source) is false) return;

		var fileSaverResult = await FileSaver.Default.SaveAsync(
			Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
			"qr_code.png", File.OpenRead(source.File));
		if (fileSaverResult.IsSuccessful) {
			string? directory = Path.GetDirectoryName(fileSaverResult.FilePath);
			if (string.IsNullOrWhiteSpace(directory))
				directory = fileSaverResult.FilePath;
			await Launcher.Default.OpenAsync(directory);
			Toast.Make("Success");
		} else {
			Toast.Make("Error saving qr code file.");
		}
	}
}