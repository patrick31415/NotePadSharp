using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

//“设置浮出控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=273769 上有介绍

namespace NodePad {
	public sealed partial class SettingOptionsPage : SettingsFlyout {
		//bool InFinal = false, InMain = false, InGray = false;
		Windows.UI.Color fontColor = Windows.UI.Colors.Black, backColor = Windows.UI.Colors.White;
		public SettingOptionsPage() {
			this.InitializeComponent();

			var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;

			fontColor = (tbx.Foreground as SolidColorBrush).Color;
			clpFont.color = fontColor;
			cvsTest.Fill = new SolidColorBrush(clpFont.color);

			backColor = (tbx.Background as SolidColorBrush).Color;
			clpBackground.color = backColor;
			cvsBackground.Fill = new SolidColorBrush(backColor);

			for (int i = 0; i < 30; ++i) {
				cbxFontSize.Items.Add(i * 2 + 2);
			}
			cbxFontSize.SelectedIndex = (int) (tbx.FontSize / 2 - 1);

			//if (tbx.TextWrapping == TextWrapping.Wrap)
			//	tgsWrap.IsOn = true;
			//else
			//	tgsWrap.IsOn = false;

			tblHint.Visibility = CompareColor(clpBackground.color, clpFont.color);
		}

		private void cvsTest_PointerPressed(object sender, PointerRoutedEventArgs e) {
			if (clpFont.Visibility == Windows.UI.Xaml.Visibility.Visible)
				clpFont.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
			else
				clpFont.Visibility = Windows.UI.Xaml.Visibility.Visible;
		}

		private void clpFont_PointerPressed(object sender, PointerRoutedEventArgs e) {
			cvsTest.Fill = new SolidColorBrush(clpFont.color);
			var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
			tbx.Foreground = new SolidColorBrush(clpFont.color);

			tblHint.Visibility = CompareColor(clpBackground.color, clpFont.color);
		}

		private void clpFont_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact) {
				cvsTest.Fill = new SolidColorBrush(clpFont.color);
				var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
				tbx.Foreground = new SolidColorBrush(clpFont.color);

				tblHint.Visibility = CompareColor(clpBackground.color, clpFont.color);
			}
		}

		private void cbxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e) {
			var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
			tbx.FontSize = cbxFontSize.SelectedIndex * 2 + 2;
		}

		private void cvsBackground_PointerPressed(object sender, PointerRoutedEventArgs e) {
			if (clpBackground.Visibility == Windows.UI.Xaml.Visibility.Collapsed)
				clpBackground.Visibility = Windows.UI.Xaml.Visibility.Visible;
			else
				clpBackground.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		}

		private void clpBackground_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact) {
				cvsBackground.Fill = new SolidColorBrush(clpBackground.color);
				var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
				tbx.Background = cvsBackground.Fill;

				tblHint.Visibility = CompareColor(clpBackground.color, clpFont.color);
			}
		}

		private void clpBackground_PointerPressed(object sender, PointerRoutedEventArgs e) {
			cvsBackground.Fill = new SolidColorBrush(clpBackground.color);
			var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
			tbx.Background = cvsBackground.Fill;

			tblHint.Visibility = CompareColor(clpBackground.color, clpFont.color);
		}

		private void tgsWrap_Toggled(object sender, RoutedEventArgs e) {
			var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
			if (tgsWrap != null) {
				if (tgsWrap.IsOn)
					tbx.TextWrapping = TextWrapping.Wrap;
				else
					tbx.TextWrapping = TextWrapping.NoWrap;
			}
		}

		private async void SettingsFlyout_Unloaded(object sender, RoutedEventArgs e) {
			try {
				var f = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFileAsync("UserSetting.txt");
				string s = "";
				s += (cbxFontSize.SelectedIndex * 2 + 2) + " ";
				Windows.UI.Color color = clpFont.color;
				s += color.R + " ";
				s += color.G + " ";
				s += color.B + " ";
				color = clpBackground.color;
				s += color.R + " ";
				s += color.G + " ";
				s += color.B + " ";
				if (tgsWrap.IsOn)
					s += 1;
				else
					s += 2;
				
				await Windows.Storage.FileIO.WriteTextAsync(f, s);
			}
			catch (Exception ex) {
				var tbx = ((Window.Current.Content as Frame).Content as ItemEditPage).FindName("tbxContent") as TextBox;
				tbx.Text = ex.ToString();
			}
		}

		Visibility CompareColor(Windows.UI.Color color1, Windows.UI.Color color2) {
			int r = color1.R - color2.R;
			int g = color1.G - color2.G;
			int b = color1.B - color2.B;

			if (r * r + g * g + b * b <= 9000)
				return Windows.UI.Xaml.Visibility.Visible;
			else
				return Windows.UI.Xaml.Visibility.Collapsed;
		}
	}
}
