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
		private ObservableDictionary defaultViewModel = new ObservableDictionary();


		private class VisualModel {
			public string Title, Subtitle;

			public VisualModel() {

			}

			public VisualModel(string s1, string s2) {
				Title = s1;
				Subtitle = s2;
			}
		}

		/// <summary>
		/// 可将其更改为强类型视图模型。
		/// </summary>
		public ObservableDictionary DefaultViewModel {
			get { return this.defaultViewModel; }
		}

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

			int n = 100;
			VisualModel[] vm = new VisualModel[n];
			for (int i = 0; i < n; ++i) {
				vm[i] = new VisualModel();
				vm[i].Title = i.ToString();
				vm[i].Subtitle = "s" + i.ToString();
			}

			var result =
				from t in vm
				select new { Title = t.Title, Subtitle = t.Subtitle };

			this.ItemViewSource.Source = result;
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
			tbxContent.Text = lstInfo.MaxHeight.ToString();
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

	}
}
