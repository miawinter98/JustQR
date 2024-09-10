using System.Diagnostics;
using System.Text;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using QRCoder;

namespace JustQR;

public partial class MainPage {
	public MainPage() {
		InitializeComponent();
	}

	private async Task UpdateQrCode() {
		var qrGenerator = new QRCodeGenerator();
		var data = qrGenerator.CreateQrCode(TextEditor.Text, QRCodeGenerator.ECCLevel.Default);
		var code = new BitmapByteQRCode(data);
		string file = Path.GetTempFileName() + ".png";
		await File.WriteAllBytesAsync(file, code.GetGraphic(20));
		QrImage.Source = ImageSource.FromFile(file);
	}

	private async void TextEditor_OnTextChanged(object? sender, TextChangedEventArgs e) {
		await UpdateQrCode();
	}

	private async void Button_OnClicked(object? sender, EventArgs e) {
		var fileSaverResult = await FileSaver.Default.SaveAsync(
			Environment.GetFolderPath(Environment.SpecialFolder.Personal), 
			"qr_code.png", File.OpenRead(((FileImageSource) QrImage.Source).File));
		if (fileSaverResult.IsSuccessful) {
			Toast.Make("Success");
		} else {
			Toast.Make("Error saving qr code file.");
		}
	}
}