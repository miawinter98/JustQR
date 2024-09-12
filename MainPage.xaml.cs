using System.Drawing;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using QRCoder;
using Color = System.Drawing.Color;

namespace JustQR;

public partial class MainPage {
	public MainPage() {
		InitializeComponent();
	}

	private async ValueTask<Color> Parse(string colorString, Color fallback) {
		try {
			return ColorTranslator.FromHtml("#" + colorString);
		} catch { /* moving on */ }
		try {
			return ColorTranslator.FromHtml(colorString);
		} catch { /* giving up */ }

		await DisplayAlert("Color format incorrect", "The color you have entered could not be interpreted.", "Ok");

		return fallback;
	}

	private async Task<byte[]?> UpdateQrCode() {
		if (string.IsNullOrWhiteSpace(TextEditor.Text)) return null;

		var light = await Parse(LightColorText.Text, Color.White);
		var dark = await Parse(DarkColorText.Text, Color.Black);

		var qrGenerator = new QRCodeGenerator();
		var data = qrGenerator.CreateQrCode(TextEditor.Text, QRCodeGenerator.ECCLevel.Default);
		var code = new PngByteQRCode(data);
		byte[] raw = code.GetGraphic(20, dark, light);
		string file = Path.Combine(FileSystem.Current.CacheDirectory, Path.GetRandomFileName() + ".png");
		await File.WriteAllBytesAsync(file, raw);
		QrImage.Source = ImageSource.FromFile(file);
		return raw;
	}

	private async void TextEditor_OnTextChanged(object? sender, TextChangedEventArgs e) {
		await UpdateQrCode();
	}

	private async void Button_OnClicked(object? sender, EventArgs e) {
		var source = QrImage.Source as FileImageSource;
		if (source is null || Path.IsPathRooted(source) is false) return;

		try {
			byte[]? data = await UpdateQrCode();
			if (data is null) return;

			var fileSaverResult = await FileSaver.Default.SaveAsync(
				Environment.GetFolderPath(Environment.SpecialFolder.Personal),
				"qr_code.png", new MemoryStream(data));
			if (fileSaverResult.IsSuccessful) {
				string? directory = Path.GetDirectoryName(fileSaverResult.FilePath);
				if (string.IsNullOrWhiteSpace(directory))
					directory = fileSaverResult.FilePath;
				await Launcher.Default.OpenAsync(directory);
				Toast.Make("Success");
			} else {
				Toast.Make("Error saving qr code file.");
			}
		} catch (Exception ex) {
			await DisplayAlert("Error", "Failed to save QR Code: " + ex.Message, "Ok");
		}
	}

	private async void ButtonRefresh_OnClicked(object? sender, EventArgs e) 
		=> await UpdateQrCode();
}