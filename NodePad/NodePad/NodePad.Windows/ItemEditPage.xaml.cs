using NodePad.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace NodePad {
	/// <summary>
	/// 基本页，提供大多数应用程序通用的特性。
	/// </summary>
	public sealed partial class ItemEditPage : Page {

		private NavigationHelper navigationHelper;
		//private ObservableDictionary defaultViewModel = new ObservableDictionary();
		private Windows.Storage.StorageFile File = null;
		bool isSaved = false;
		int stat = 1, preStat = 0;

		/// <summary>
		/// 可将其更改为强类型视图模型。
		/// </summary>

		/// <summary>
		/// NavigationHelper 在每页上用于协助导航和
		/// 进程生命期管理
		/// </summary>
		public NavigationHelper NavigationHelper {
			get { return this.navigationHelper; }
		}


		public ItemEditPage() {
			this.InitializeComponent();
			this.navigationHelper = new NavigationHelper(this);
			this.navigationHelper.LoadState += navigationHelper_LoadState;
			this.navigationHelper.SaveState += navigationHelper_SaveState;
		}

		/// <summary>
		///使用在导航过程中传递的内容填充页。 在从以前的会话
		/// 重新创建页时，也会提供任何已保存状态。
		/// </summary>
		/// <param name="sender">
		/// 事件的来源; 通常为 <see cref="NavigationHelper"/>
		/// </param>
		/// <param name="e">事件数据，其中既提供在最初请求此页时传递给
		/// <see cref="Frame.Navigate(Type, Object)"/> 的导航参数，又提供
		/// 此页在以前会话期间保留的状态的
		/// 的字典。 首次访问页面时，该状态将为 null。</param>
		private void navigationHelper_LoadState(object sender, LoadStateEventArgs e) {
			File = e.NavigationParameter as Windows.Storage.StorageFile;
			SetUI();
		}

		/// <summary>
		/// 保留与此页关联的状态，以防挂起应用程序或
		/// 从导航缓存中放弃此页。  值必须符合
		/// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
		/// </summary>
		///<param name="sender">事件的来源；通常为 <see cref="NavigationHelper"/></param>
		///<param name="e">提供要使用可序列化状态填充的空字典
		///的事件数据。</param>
		private void navigationHelper_SaveState(object sender, SaveStateEventArgs e) {
		}

		#region NavigationHelper 注册

		/// 此部分中提供的方法只是用于使
		/// NavigationHelper 可响应页面的导航方法。
		/// 
		/// 应将页面特有的逻辑放入用于
		/// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
		/// 和 <see cref="GridCS.Common.NavigationHelper.SaveState"/> 的事件处理程序中。
		/// 除了在会话期间保留的页面状态之外
		/// LoadState 方法中还提供导航参数。

		protected override void OnNavigatedTo(NavigationEventArgs e) {
			navigationHelper.OnNavigatedTo(e);
		}

		protected override void OnNavigatedFrom(NavigationEventArgs e) {
			navigationHelper.OnNavigatedFrom(e);
		}

		#endregion

		private void _new_Click(object sender, RoutedEventArgs e) {
			//tbxContent.Text = lstInfo.MaxHeight.ToString();
		}

		private void pageRoot_SizeChanged(object sender, SizeChangedEventArgs e) {
			ApplicationView view = ApplicationView.GetForCurrentView();

			if (!view.IsFullScreen && e.NewSize.Width <= 420) {
				VisualStateManager.GoToState(this, "State420", false);
			}
			else {
				VisualStateManager.GoToState(this, "Normal", false);
			}
		}

		private async void pageRoot_Loaded(object sender, RoutedEventArgs e) {
			try {
				Windows.Storage.StorageFile f = null;
				bool q = false;
				var g = await Windows.Storage.ApplicationData.Current.LocalFolder.GetFilesAsync();
				foreach (Windows.Storage.StorageFile ff in g) {
					if (ff.Name == "UserSetting.txt") {
						q = true;
						f = ff;
						break;
					}
				}
				if (!q) {
					f = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFileAsync("UserSetting.txt");
					string t = "20 0 0 0 255 255 255 1 1";
					await Windows.Storage.FileIO.WriteTextAsync(f, t);
				}
				string[] s = (await Windows.Storage.FileIO.ReadTextAsync(f)).Split(' ');
				tbxContent.FontSize = Convert.ToInt32(s[0]);
				Windows.UI.Color color = new Windows.UI.Color();
				color.A = 255;
				color.R = Convert.ToByte(s[1]);
				color.G = Convert.ToByte(s[2]);
				color.B = Convert.ToByte(s[3]);
				tbxContent.Foreground = new SolidColorBrush(color);

				color.R = Convert.ToByte(s[4]);
				color.G = Convert.ToByte(s[5]);
				color.B = Convert.ToByte(s[6]);
				tbxContent.Background = new SolidColorBrush(color);

				//自动换行
				int d = (s[7] == null) ? 1 : Convert.ToInt32(s[7]);
				switch (d) {
					case 1:
						tbxContent.TextWrapping = TextWrapping.Wrap;
						break;
					case 2:
						tbxContent.TextWrapping = TextWrapping.NoWrap;
						break;
					default:
						break;
				}

				//粗体
				d = (s[8] == null) ? 2 : Convert.ToInt32(s[8]);
				switch (d) {
					case 1:
						tbxContent.FontWeight = Windows.UI.Text.FontWeights.Bold;
						break;
					case 2:
						tbxContent.FontWeight = Windows.UI.Text.FontWeights.Normal;
						break;
					default:
						break;
				}
			}
			catch (Exception p) {
				tbxContent.Text = p.ToString();
			}
		}

		async void SetUI() {
			if (File != null) {
				pageTitle.Text = File.Name;
				bool isLegal = true;
				try {
					tbxContent.Text = await Windows.Storage.FileIO.ReadTextAsync(File);
				}
				catch {
					isLegal = false;
				}
				if (!isLegal) {
					Windows.UI.Popups.MessageDialog msg = new Windows.UI.Popups.MessageDialog("由于该文件不是UTF-8编码，无法正常打开");
					await msg.ShowAsync();
				}
			}
			else {
				pageTitle.Text = Resources["FileName"] as string;
				tbxContent.Text = "";
			}
		}

		private async void btnNew_Click(object sender, RoutedEventArgs e) {
			if (isSaved) {
				File = null;
				preStat = stat - 1;
				SetUI();
			}
			else {
				Windows.UI.Popups.MessageDialog msg = new Windows.UI.Popups.MessageDialog("修改的文本是否需要保存？");

				msg.Commands.Add(new Windows.UI.Popups.UICommand("是", null, 0));
				msg.Commands.Add(new Windows.UI.Popups.UICommand("否", null, 1));
				var fl =(await msg.ShowAsync()).Id as int?;
				switch (fl) {
					case 0:
						Windows.Storage.Pickers.FileSavePicker fsp = new Windows.Storage.Pickers.FileSavePicker();
						fsp.DefaultFileExtension = ".txt";
						fsp.FileTypeChoices.Add("text", new List<string>() { ".txt" });
						var Sf = await fsp.PickSaveFileAsync();
						if (Sf != null) {
							await Windows.Storage.FileIO.WriteTextAsync(Sf, tbxContent.Text);
							File = null;
							preStat = stat - 1;
							SetUI();
						}
						break;
					default:
						break;
				}
			}
		}

		private async void btnSave_Click(object sender, RoutedEventArgs e) {
			if (File == null) 
				btnSaveAs_Click(sender, e);
			else {
				await Windows.Storage.FileIO.WriteTextAsync(File, tbxContent.Text);
				isSaved = true;
				preStat = stat - 1;
			}
		}

		private async void btnSaveAs_Click(object sender, RoutedEventArgs e) {
			Windows.Storage.Pickers.FileSavePicker fsp = new Windows.Storage.Pickers.FileSavePicker();
			fsp.DefaultFileExtension = ".txt";
			fsp.FileTypeChoices.Add("text", new List<string>() { ".txt" });
			var Sf = await fsp.PickSaveFileAsync();
			if (Sf != null) {
				File = Sf;
				await Windows.Storage.FileIO.WriteTextAsync(File, tbxContent.Text);
				SetUI();
				preStat = stat - 1;
				isSaved = true;
			}
		}

		private async void btnOpen_Click(object sender, RoutedEventArgs e) {
			if (isSaved) {
				Windows.Storage.Pickers.FileOpenPicker fop = new Windows.Storage.Pickers.FileOpenPicker();
				fop.FileTypeFilter.Add(".txt");
				fop.ViewMode = Windows.Storage.Pickers.PickerViewMode.List;
				var Sf = await fop.PickSingleFileAsync();
				if (Sf != null) {
					File = Sf;
					preStat = stat - 1;
					SetUI();
				}
			}
			else {
				Windows.UI.Popups.MessageDialog msg = new Windows.UI.Popups.MessageDialog("修改的文本是否需要保存？");

				msg.Commands.Add(new Windows.UI.Popups.UICommand("是", null, 0));
				msg.Commands.Add(new Windows.UI.Popups.UICommand("否", null, 1));
				var fl = (await msg.ShowAsync()).Id as int?;
				switch (fl) {
					case 0:
						Windows.Storage.Pickers.FileSavePicker fsp = new Windows.Storage.Pickers.FileSavePicker();
						fsp.DefaultFileExtension = ".txt";
						fsp.FileTypeChoices.Add("text", new List<string>() { ".txt" });
						var Sf = await fsp.PickSaveFileAsync();
						if (Sf != null) {
							await Windows.Storage.FileIO.WriteTextAsync(Sf, tbxContent.Text);
							File = null;
							preStat = stat - 1;
							SetUI();
						}
						break;
					default:
						break;
				}
				isSaved = true;
				btnOpen_Click(sender, e);
			}
		}

		private void tbxContent_TextChanged(object sender, TextChangedEventArgs e) {
			if (stat == preStat) 
				isSaved = false;
			else 
				preStat = stat;
		}
	}
}
