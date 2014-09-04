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
		/*
		private double rectMainColor_Position = 0;
		private double rectGrayColor_Position = 0;
		private Point rectFinalColor_Position = new Point();
		private Color colorFinal = new Color();
		private double relativeMainPosition = 0;
		private double relativeGrayPosition = 0;
		private double relativeFinalColorPosition = 0;
		private bool isInFinalColor = false, isInMainColor = false, isInGrayColor = false;
		*/
		
		bool InFinal = false, InMain = false, InGray = false;
		Windows.UI.Color FinalColor, MainColor, GrayColor;
		Point relativeFinalColorPosition;
		double relativeMainColor, relativeGrayColor;
		Brush refColor;

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
			//colorFinal = Colors.Black;
			Init();
			ResetGraphics();
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
			refColor = new SolidColorBrush(color);
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

		/*
		private void Resize() {
			Init();

			cvsFinalColor.Width = this.Width;
			cvsFinalColor.Height = this.Height * 0.75;

			Canvas.SetTop(rectGrayColor, 0);
			Canvas.SetLeft(rectGrayColor, 0);
			rectGrayColor.Width = cvsFinalColor.Width * 0.1;
			rectGrayColor.Height = cvsFinalColor.Height;

			Canvas.SetLeft(rectFinalColor, cvsFinalColor.Width * 0.2);
			Canvas.SetTop(rectFinalColor, 0);
			rectFinalColor.Width = cvsFinalColor.Width * 0.6;
			rectFinalColor.Height = cvsFinalColor.Height;

			Canvas.SetTop(rectMainColor, 0);
			Canvas.SetLeft(rectMainColor, cvsFinalColor.Width * 0.9);
			rectMainColor.Width = cvsFinalColor.Width * 0.1;
			rectMainColor.Height = cvsFinalColor.Height;

			rectMainColor_Position = relativeMainPosition * rectMainColor.Height;
			rectGrayColor_Position = relativeGrayPosition * rectGrayColor.Height;

			Canvas.SetTop(elpsSelectPosition, cvsFinalColor.Height * 0.5 - 5);
			Canvas.SetLeft(elpsSelectPosition, cvsFinalColor.Width * 0.2 + rectFinalColor.Width * relativeFinalColorPosition - 5);

			Canvas.SetTop(rectSelectMainColor, rectMainColor_Position);
			Canvas.SetLeft(rectSelectMainColor, cvsFinalColor.Width * 0.9);
			rectSelectMainColor.Width = cvsFinalColor.Width * 0.1;
			rectSelectMainColor.Height = 1;

			Canvas.SetTop(rectSelectGrayColor, rectGrayColor_Position);
			Canvas.SetLeft(rectSelectGrayColor, 0);
			rectSelectGrayColor.Width = cvsFinalColor.Width * 0.1;
			rectSelectGrayColor.Height = 1;

			Point[] pp = new Point[3];
			Point[] pp1 = new Point[3];
			PointCollection p = new PointCollection();
			PointCollection p1 = new PointCollection();

			pp[0].X = cvsFinalColor.Width * 0.2;
			pp[0].Y = 0;
			pp[1].Y = rectGrayColor_Position;
			pp[1].X = cvsFinalColor.Width * 0.1;
			pp[2].X = cvsFinalColor.Width * 0.2;
			pp[2].Y = rectFinalColor.Height;
			for (int i = 0; i < 3; ++i)
				p.Add(pp[i]);
			plgSelectGray.Points = p;

			pp1[0].X = cvsFinalColor.Width * 0.8;
			pp1[0].Y = 0;
			pp1[1].Y = rectMainColor_Position;
			pp1[1].X = cvsFinalColor.Width * 0.9;
			pp1[2].X = cvsFinalColor.Width * 0.8;
			pp1[2].Y = rectMainColor.Height;
			for (int i = 0; i < 3; ++i)
				p1.Add(pp1[i]);
			plgSelectMain.Points = p1;
		}

		public void Init() {
			if (colorFinal.R == colorFinal.G && colorFinal.G == colorFinal.B) {
				relativeMainPosition = 0.5;
				relativeGrayPosition = (double)colorFinal.R / 255;
				relativeFinalColorPosition = 0;
			}
			else {
				Byte maxB = 0, minB = 255;
				maxB = (colorFinal.R > colorFinal.G) ? colorFinal.R : colorFinal.G;
				maxB = (maxB > colorFinal.B) ? maxB : colorFinal.B;
				minB = (colorFinal.R < colorFinal.G) ? colorFinal.R : colorFinal.G;
				minB = (minB < colorFinal.B) ? minB : colorFinal.B;

				relativeFinalColorPosition = (double)((maxB - minB) / 255);
				double p = 255 - relativeFinalColorPosition * maxB;
				relativeGrayPosition = System.Math.Abs(p) / 255;
				relativeMainPosition = GetMainPosition((Byte)(colorFinal.R * relativeFinalColorPosition + p), (Byte)(colorFinal.G * relativeFinalColorPosition + p), (Byte)(colorFinal.B * relativeFinalColorPosition + p));
			}
		}

		private double GetMainPosition(Byte r, Byte g, Byte b) {
			if (r == 255) {
				if (b == 0)
					return (double)g / 1530;					Part 1
				else
					return (double)(1530 - b) / 1530;		Part 6
			}
			else if (r == 0) {
				if (g == 255)
					return (double)(510 + b) / 1530;		Part 3
				else
					return (double)(1020 - g) / 1530;		Part 4
			}
			else {
				if (g == 255)
					return (double)(510 - r) / 1530;
				else
					return (double)(1020 + r) / 1530;
			}
		}

		public void Update() {
			SetRectMain();
			SetRectGray();
		}

		private void rectMainColor_PointerPressed(object sender, PointerRoutedEventArgs e) {
			isInMainColor = true;
			rectMainColor_Position = e.GetCurrentPoint(rectMainColor).Position.Y;
			Canvas.SetTop(rectSelectMainColor, rectMainColor_Position);

			SetRectMain();

			if (MousePressed != null)
				MousePressed(sender, e);
		}

		private void rectFinalColor_PointerPressed(object sender, PointerRoutedEventArgs e) {
			isInFinalColor = true;
			rectFinalColor_Position = e.GetCurrentPoint(rectFinalColor).Position;
			Canvas.SetLeft(elpsSelectPosition, rectFinalColor_Position.X - 5 + Canvas.GetLeft(rectFinalColor));
			Canvas.SetTop(elpsSelectPosition, rectFinalColor_Position.Y - 5);
			colorFinal = GetFinalColor();

			if (MousePressed != null)
				MousePressed(sender, e);
		}

		private void rectFinalColor_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact && e.Pointer.IsInRange && isInFinalColor) {
				rectFinalColor_Position = e.GetCurrentPoint(rectFinalColor).Position;
				Canvas.SetLeft(elpsSelectPosition, rectFinalColor_Position.X - 5 + Canvas.GetLeft(rectFinalColor));
				Canvas.SetTop(elpsSelectPosition, rectFinalColor_Position.Y - 5);
				colorFinal = GetFinalColor();

				if (MouseMoved != null)
					MouseMoved(sender, e);
			}
		}

		private void rectMainColor_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact && e.Pointer.IsInRange && isInMainColor) {
				rectMainColor_Position = e.GetCurrentPoint(rectMainColor).Position.Y;
				Canvas.SetTop(rectSelectMainColor, rectMainColor_Position);
				gspMainColor.Color = GetMainColor();

				SetRectMain();

				if (MouseMoved != null)
					MouseMoved(sender, e);
			}
		}

		private void SetRectMain() {
			gspMainColor.Color = GetMainColor();
			SolidColorBrush sb = new SolidColorBrush();
			sb.Color = gspMainColor.Color;
			plgSelectMain.Fill = sb;

			//改变三角形位置
			var pc = plgSelectMain.Points;
			pc[1] = new Point(pc[1].X, rectMainColor_Position);
			plgSelectMain.Points = pc;

			colorFinal = GetFinalColor();
		}

		private int Interpolation(int a, int b, double k) {
			return (int)(a + (b - a) * (1 - k));
		}

		public Color GetMainColor() {
			double k = rectMainColor_Position / rectMainColor.Height;
			Color a = new Color();
			a.A = 255;
			if (k <= 0.167) {
				a.R = 255;
				a.G = (Byte)Interpolation(0, 255, (1 - 6 * k));
				a.B = 0;
				return a;
			}
			else if (k <= 0.333) {
				a.R = (Byte)Interpolation(255, 0, (2 - 6 * k));
				a.G = 255;
				a.B = 0;
				return a;
			}
			else if (k <= 0.5) {
				a.R = 0;
				a.G = 255;
				a.B = (Byte)Interpolation(0, 255, (3 - 6 * k));
				return a;
			}
			else if (k <= 0.667) {
				a.R = 0;
				a.G = (Byte)Interpolation(255, 0, (4 - 6 * k));
				a.B = 255;
				return a;
			}
			else if (k <= 0.833) {
				a.R = (Byte)Interpolation(0, 255, (5 - 6 * k));
				a.G = 0;
				a.B = 255;
				return a;
			}
			else {
				a.R = 255;
				a.G = 0;
				a.B = (Byte)Interpolation(255, 0, (6 - 6 * k));
				return a;
			}
		}

		public Color GetGrayColor() {
			Color grayColor = new Color();
			double k = rectGrayColor_Position / rectGrayColor.Height;
			Byte m = (Byte)Interpolation(0, 255, k);
			grayColor.R = grayColor.G = grayColor.B = m;
			grayColor.A = 255;
			return grayColor;
		}

		private void rectGrayColor_PointerPressed(object sender, PointerRoutedEventArgs e) {
			isInGrayColor = true;

			rectGrayColor_Position = e.GetCurrentPoint(rectGrayColor).Position.Y;
			Canvas.SetTop(rectSelectGrayColor, rectGrayColor_Position);

			SetRectGray();
			if (MousePressed != null)
				MousePressed(sender, e);
		}

		private void SetRectGray() {
			gspGrayColor.Color = GetGrayColor();
			SolidColorBrush sb = new SolidColorBrush();
			sb.Color = gspGrayColor.Color;
			plgSelectGray.Fill = sb;

			//改变三角形位置
			var pc = plgSelectGray.Points;
			pc[1] = new Point(pc[1].X, rectGrayColor_Position);
			plgSelectGray.Points = pc;

			colorFinal = GetFinalColor();
		}

		private void rectGrayColor_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (e.Pointer.IsInContact && e.Pointer.IsInRange && isInGrayColor) {
				rectGrayColor_Position = e.GetCurrentPoint(rectGrayColor).Position.Y;
				Canvas.SetTop(rectSelectGrayColor, rectGrayColor_Position);

				SetRectGray();

				if (MouseMoved != null)
					MouseMoved(sender, e);
			}
		}

		private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e) {
			Resize();
			Update();
		}

		private Color GetFinalColor() {
			Color colorMain = GetMainColor();
			Color colorGray = GetGrayColor();
			Color color = new Color(); ;

			relativeFinalColorPosition = rectFinalColor_Position.X / rectFinalColor.Width;
			color.A = 255;
			color.R = (Byte)(colorGray.R + (colorMain.R - colorGray.R) * relativeFinalColorPosition);
			color.G = (Byte)(colorGray.G + (colorMain.G - colorGray.G) * relativeFinalColorPosition);
			color.B = (Byte)(colorGray.B + (colorMain.B - colorGray.B) * relativeFinalColorPosition);

			return color;
		}

		public event PointerEventHandler MouseMoved;
		public event PointerEventHandler MousePressed;
		public event PointerEventHandler MouseReleased;

		private void rectFinalColor_PointerReleased(object sender, PointerRoutedEventArgs e) {
			isInMainColor = false;
			isInGrayColor = false;
			isInFinalColor = false;
		}

		private void rectGrayColor_PointerReleased(object sender, PointerRoutedEventArgs e) {
			isInMainColor = false;
			isInGrayColor = false;
			isInFinalColor = false;
		}

		private void rectMainColor_PointerReleased(object sender, PointerRoutedEventArgs e) {
			isInMainColor = false;
			isInGrayColor = false;
			isInFinalColor = false;
		}

		private void UserControl_PointerReleased(object sender, PointerRoutedEventArgs e) {
			isInMainColor = false;
			isInGrayColor = false;
			isInFinalColor = false;
		}

		private void UserControl_PointerMoved(object sender, PointerRoutedEventArgs e) {
			if (isInFinalColor) {
				var current = e.GetCurrentPoint(cvsFinalColor).Position;
				 if (current.X > Width * 0.8) {
					 Canvas.SetLeft(elpsSelectPosition, Width * 0.8 - 5);
					 Canvas.SetTop(elpsSelectPosition, current.Y - 5);
				 }
				 else if (current.X < Width * 0.2) {
					 Canvas.SetLeft(elpsSelectPosition, Width * 0.2 - 5);
					 Canvas.SetTop(elpsSelectPosition, current.Y - 5);
				 }

				 if (current.Y > Height) {
					 Canvas.SetTop(elpsSelectPosition, Height - 5);
				 }
				 else if (current.Y < 0) {
					 Canvas.SetTop(elpsSelectPosition, -5);
				 }
			}

			if (isInMainColor) {
				var current = e.GetCurrentPoint(cvsFinalColor).Position;
				if (current.X > Width || current.X < Width * 0.9) {
				}
				if (current.Y > Height) {
					relativeMainPosition = 1;
				}
				else if (current.Y < 0) {
					relativeMainPosition = 0;
				}
			}

			if (isInGrayColor) {
				var current = e.GetCurrentPoint(cvsFinalColor).Position;
				if (current.X < 0 || current.X > Width * 0.1) {
					rectGrayColor_Position = current.Y;
					relativeGrayPosition = rectGrayColor_Position / rectGrayColor.Height;
					gspGrayColor.Color = GetGrayColor();
					Canvas.SetTop(rectSelectGrayColor, relativeGrayPosition * rectGrayColor.Height);
				}

				if (current.Y > Height) {
					relativeGrayPosition = 1;
					rectGrayColor_Position = rectGrayColor.Height;
					gspGrayColor.Color = GetGrayColor();
					Canvas.SetTop(rectSelectGrayColor, relativeGrayPosition * rectGrayColor.Height);
				}
				else if (current.Y < 0) {
					relativeGrayPosition = 0;
					rectGrayColor_Position = 0;
					gspGrayColor.Color = GetGrayColor();
					Canvas.SetTop(rectSelectGrayColor, relativeGrayPosition * rectGrayColor.Height);
				}
				SetRectGray();
			}
		}
		*/
	}
}
