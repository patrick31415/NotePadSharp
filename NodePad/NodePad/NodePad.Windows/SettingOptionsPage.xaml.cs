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
		bool InFinal = false, InMain = false, InGray = false;
		Windows.UI.Color FinalColor, MainColor, GrayColor;
		Point relativeFinalColorPosition;
		double relativeMainColor, relativeGrayColor;

		public SettingOptionsPage() {
			this.InitializeComponent();

			Windows.UI.Color col = new Windows.UI.Color();
			col.A = 255;
			col.R = 255;
			col.G = 0;
			col.B = 0;

			FinalColor = col;
			Init();
			ResetGraphics();
		}

		private void cvsTest_PointerPressed(object sender, PointerRoutedEventArgs e) {

		}

		//从FinalColor得到ColorPicker图形的初始化数据
		private void Init() {
			relativeFinalColorPosition.Y = 0.5;
			if (FinalColor.R == FinalColor.G && FinalColor.G == FinalColor.B) {
				relativeGrayColor = (double)FinalColor.R / 255;
				relativeFinalColorPosition.X = 0;
			}
			else {
				Byte maxB = 0, minB = 255;
				maxB = (FinalColor.R > FinalColor.G) ? FinalColor.R : FinalColor.G;
				maxB = (maxB > FinalColor.B) ? maxB : FinalColor.B;
				minB = (FinalColor.R < FinalColor.G) ? FinalColor.R : FinalColor.G;
				minB = (minB < FinalColor.B) ? minB : FinalColor.B;

				relativeFinalColorPosition.X = ((double)(maxB - minB) / 255);
				double p = 255 - relativeFinalColorPosition.X * maxB;
				relativeGrayColor = System.Math.Abs(p) / 255;
				MainColor.A = 255;
				MainColor.R = (Byte)(FinalColor.R * relativeFinalColorPosition.X + p);
				MainColor.G = (Byte)(FinalColor.G * relativeFinalColorPosition.X + p);
				MainColor.B = (Byte)(FinalColor.B * relativeFinalColorPosition.X + p);
				GrayColor.R = GrayColor.G = GrayColor.B = (Byte)p;
				GrayColor.A = 255;
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
					return (double)(510 - r) / 1530;
				else
					return (double)(1020 + r) / 1530;
			}
		}

		//重置所有图形
		void ResetGraphics() {
			Canvas.SetTop(rectSelectGray, rectGrayColor.Height * relativeGrayColor);
			Canvas.SetTop(rectSelectMain, relativeMainColor * rectMainColor.Height);
			Canvas.SetTop(elpSelectFinalIn, relativeFinalColorPosition.Y * rectFinalColor.Height - 3);
			Canvas.SetLeft(elpSelectFinalIn, relativeFinalColorPosition.X * rectFinalColor.Width - 3 + 40);
			Canvas.SetTop(elpSelectFinalOut, relativeFinalColorPosition.Y * rectFinalColor.Height - 5);
			Canvas.SetLeft(elpSelectFinalOut, relativeFinalColorPosition.X * rectFinalColor.Width - 5 + 40);
			gspGrayPart.Color = GrayColor;
			gspMainPart.Color = MainColor;
		}
	}
}
