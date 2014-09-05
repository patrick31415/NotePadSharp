using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace NodePad {
	public sealed partial class ColorPicker : UserControl {
		Windows.UI.Color FinalColor, MainColor, GrayColor;
		Point relativeFinalColorPosition;
		double relativeMainColor, relativeGrayColor;

		public Windows.UI.Color color {
			get {
				return GetFinalColor();
			}
			set {
				FinalColor = value;
				Init();
				ResetGraphics();
			}
		}

		public ColorPicker() {
			this.InitializeComponent();
			Init();
			ResetGraphics();
		}

		//从FinalColor得到ColorPicker图形的初始化数据
		private void Init() {
			relativeFinalColorPosition.Y = 0.5;
			if (FinalColor.R == FinalColor.G && FinalColor.G == FinalColor.B) {
				relativeGrayColor = 1 - (double)FinalColor.R / 255;
				GrayColor = FinalColor;
				MainColor = GetMainColor(1.0 / 3);
				relativeMainColor = GetMainPosition(MainColor.R, MainColor.G, MainColor.B);
				relativeFinalColorPosition.X = 0;
			}
			else {
				Byte maxB = 0, minB = 255;
				maxB = (FinalColor.R > FinalColor.G) ? FinalColor.R : FinalColor.G;
				maxB = (maxB > FinalColor.B) ? maxB : FinalColor.B;
				minB = (FinalColor.R < FinalColor.G) ? FinalColor.R : FinalColor.G;
				minB = (minB < FinalColor.B) ? minB : FinalColor.B;

				relativeFinalColorPosition.X = ((double)(maxB - minB) / 255);
				double p = (double)minB * 255 / (255 + minB - maxB);
				relativeGrayColor = System.Math.Abs(p) / 255;
				MainColor.A = 255;
				MainColor.R = (Byte)((FinalColor.R - p) / relativeFinalColorPosition.X + p);
				MainColor.G = (Byte)((FinalColor.G - p) / relativeFinalColorPosition.X + p);
				MainColor.B = (Byte)((FinalColor.B - p) / relativeFinalColorPosition.X + p);
				GrayColor.R = GrayColor.G = GrayColor.B = (Byte)p;
				GrayColor.A = 255;

				if (FinalColor.R == maxB)
					MainColor.R = 255;
				if (FinalColor.G == maxB)
					MainColor.G = 255;
				if (FinalColor.B == maxB)
					MainColor.B = 255;
				if (FinalColor.R == minB)
					MainColor.R = 0;
				if (FinalColor.G == minB)
					MainColor.G = 0;
				if (FinalColor.B == minB)
					MainColor.B = 0;
				relativeMainColor = GetMainPosition(MainColor.R, MainColor.G, MainColor.B);
			}
		}

		//得到MainColor的相对位置
		private double GetMainPosition(Byte r, Byte g, Byte b) {
			if (r == 255) {
				if (b == 0)
					return (double)g / 1530;					/*Part 1*/
				else
					return (double)(1530 - b) / 1530;		/*Part 6*/
			}
			else if (r == 0) {
				if (g == 255)
					return (double)(510 + b) / 1530;		/*Part 3*/
				else
					return (double)(1020 - g) / 1530;		/*Part 4*/
			}
			else {
				if (g == 255)
					return (double)(510 - r) / 1530;			/*Part 2*/
				else
					return (double)(1020 + r) / 1530;		/*Part 5*/
			}
		}

		//通知UI，重置所有图形
		void ResetGraphics() {
			Canvas.SetTop(rectSelectGray, rectGrayColor.Height * relativeGrayColor);
			Canvas.SetTop(rectSelectMain, relativeMainColor * rectMainColor.Height);
			Canvas.SetTop(elpSelectFinalIn, relativeFinalColorPosition.Y * rectFinalColor.Height - 4);
			Canvas.SetLeft(elpSelectFinalIn, relativeFinalColorPosition.X * rectFinalColor.Width - 4 + 45);
			Canvas.SetTop(elpSelectFinalOut, relativeFinalColorPosition.Y * rectFinalColor.Height - 5);
			Canvas.SetLeft(elpSelectFinalOut, relativeFinalColorPosition.X * rectFinalColor.Width - 5 + 45);
			gspGrayPart.Color = GrayColor;
			gspMainPart.Color = MainColor;
		}

		Windows.UI.Color GetFinalColor() {
			FinalColor = new Windows.UI.Color();
			FinalColor.A = 255;
			FinalColor.R = (Byte)(GrayColor.R + (MainColor.R - GrayColor.R) * relativeFinalColorPosition.X);
			FinalColor.G = (Byte)(GrayColor.G + (MainColor.G - GrayColor.G) * relativeFinalColorPosition.X);
			FinalColor.B = (Byte)(GrayColor.B + (MainColor.B - GrayColor.B) * relativeFinalColorPosition.X);
			return FinalColor;
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {

		}

		private void UserControl_PointerReleased(object sender, PointerRoutedEventArgs e) {

		}

		private void UserControl_PointerMoved(object sender, PointerRoutedEventArgs e) {

		}

		private void rectGrayColor_PointerPressed(object sender, PointerRoutedEventArgs e) {
			var po = e.GetCurrentPoint(rectGrayColor);
			relativeGrayColor = po.Position.Y / rectGrayColor.Height;
			Color c = new Color();
			c.A = 255;
			c.R = c.G = c.B = (Byte)((1 - relativeGrayColor) * 255);
			GrayColor = c;
			ResetGraphics();
		}

		private void rectGrayColor_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact) {
				var po = e.GetCurrentPoint(rectGrayColor);
				relativeGrayColor = po.Position.Y / rectGrayColor.Height;
				Color c = new Color();
				c.A = 255;
				c.R = c.G = c.B = (Byte)((1 - relativeGrayColor) * 255);
				GrayColor = c;
				ResetGraphics();
			}
		}

		private void rectMainColor_PointerPressed(object sender, PointerRoutedEventArgs e) {
			var po = e.GetCurrentPoint(rectMainColor);
			relativeMainColor = po.Position.Y / rectMainColor.Height;
			MainColor = GetMainColor(relativeMainColor);
			ResetGraphics();
		}

		private void rectMainColor_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact) {
				var po = e.GetCurrentPoint(rectMainColor);
				relativeMainColor = po.Position.Y / rectMainColor.Height;
				MainColor = GetMainColor(relativeMainColor);
				ResetGraphics();
			}
		}

		private Color GetMainColor(double k) {
			Color c = new Color();
			c.A = 255;
			if (k < 1.0 / 6) {
				k -= 1.0 / 6;
				k = -k;
				k *= 6;
				c.R = 255;
				c.B = 0;
				k = 1 - k;
				c.G = (Byte)(k * 255);
			}
			else if (k < 1.0 / 3) {
				k -= 1.0 / 3;
				k = -k;
				k *= 6;
				c.G = 255;
				c.B = 0;
				c.R = (Byte)(k * 255);
			}
			else if (k < 1.0 / 2) {
				k -= 0.5;
				k = -k;
				k *= 6;
				c.R = 0;
				c.G = 255;
				k = 1 - k;
				c.B = (Byte)(k * 255);
			}
			else if (k < 2.0 / 3) {
				k -= 2.0 / 3;
				k = -k;
				k *= 6;
				c.R = 0;
				c.B = 255;
				c.G = (Byte)(k * 255);
			}
			else if (k < 5.0 / 6) {
				k -= 5.0 / 6;
				k = -k;
				k *= 6;
				c.G = 0;
				c.B = 255;
				k = 1 - k;
				c.R = (Byte)(k * 255);
			}
			else {
				k--;
				k = -k;
				k *= 6;
				c.R = 255;
				c.G = 0;
				c.B = (Byte)(k * 255);
			}
			return c;
		}

		private void rectFinalColor_PointerPressed(object sender, PointerRoutedEventArgs e) {
			var po = e.GetCurrentPoint(rectFinalColor);
			relativeFinalColorPosition.X = po.Position.X / rectFinalColor.Width;
			relativeFinalColorPosition.Y = po.Position.Y / rectFinalColor.Height;
			ResetGraphics();
		}

		private void rectFinalColor_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact) {
				var po = e.GetCurrentPoint(rectFinalColor);
				relativeFinalColorPosition.X = po.Position.X / rectFinalColor.Width;
				relativeFinalColorPosition.Y = po.Position.Y / rectFinalColor.Height;
				ResetGraphics();
			}
		}
	}
}
