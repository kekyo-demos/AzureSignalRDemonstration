////////////////////////////////////////////////////////////////////////////////////////////////////
//
// AzureSignalRDemonstration -
//     SignalR boot camp on Windows Azure Datacenter came to Japan festival in NAGOYA.
//
// Copyright (c) Kouji Matsui, All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification,
// are permitted provided that the following conditions are met:
//
// * Redistributions of source code must retain the above copyright notice,
//   this list of conditions and the following disclaimer.
// * Redistributions in binary form must reproduce the above copyright notice,
//   this list of conditions and the following disclaimer in the documentation
//   and/or other materials provided with the distribution.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO,
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED.
// IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT,
// INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
// (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
// LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY,
// OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//
////////////////////////////////////////////////////////////////////////////////////////////////////

using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows;
using System.Windows.Navigation;

namespace AzureSignalR
{
	public partial class App : Application
	{
		/// <summary>
		/// Phone アプリケーションのルート フレームへの容易なアクセスを提供します。
		/// </summary>
		/// <returns>Phone アプリケーションのルート フレームです。</returns>
		public PhoneApplicationFrame RootFrame { get; private set; }

		/// <summary>
		/// Application オブジェクトのコンストラクターです。
		/// </summary>
		public App()
		{
			// Global handler for uncaught exceptions. 
			UnhandledException += Application_UnhandledException;

			// Standard Silverlight initialization
			InitializeComponent();

			// Phone 固有の初期化
			InitializePhoneApplication();

			// デバッグ中にグラフィックスのプロファイル情報を表示します。
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// 現在のフレーム レート カウンターを表示します。
				Application.Current.Host.Settings.EnableFrameRateCounter = true;

				// 各フレームで再描画されているアプリケーションの領域を表示します。
				//Application.Current.Host.Settings.EnableRedrawRegions = true;

				// Enable non-production analysis visualization mode, 
				// これにより、色付きのオーバーレイを使用して、GPU に渡されるページの領域が表示されます。
				//Application.Current.Host.Settings.EnableCacheVisualization = true;

				// Disable the application idle detection by setting the UserIdleDetectionMode property of the
				// application's PhoneApplicationService object to Disabled.
				// 注意: これはデバッグ モードのみで使用してください。ユーザーが電話を使用していないときに、ユーザーのアイドル状態の検出を無効にする
				// アプリケーションが引き続き実行され、バッテリ電源が消耗します。
				PhoneApplicationService.Current.UserIdleDetectionMode = IdleDetectionMode.Disabled;
			}

		}

		// (たとえば、[スタート] メニューから) アプリケーションが起動するときに実行されるコード
		// このコードは、アプリケーションが再アクティブ化済みの場合には実行されません
		private void Application_Launching(object sender, LaunchingEventArgs e)
		{
		}

		// アプリケーションがアクティブになった (前面に表示された) ときに実行されるコード
		// このコードは、アプリケーションの初回起動時には実行されません
		private void Application_Activated(object sender, ActivatedEventArgs e)
		{
		}

		// アプリケーションが非アクティブになった (バックグラウンドに送信された) ときに実行されるコード
		// このコードは、アプリケーションの終了時には実行されません
		private void Application_Deactivated(object sender, DeactivatedEventArgs e)
		{
		}

		// (たとえば、ユーザーが戻るボタンを押して) アプリケーションが終了するときに実行されるコード
		// このコードは、アプリケーションが非アクティブになっているときには実行されません
		private void Application_Closing(object sender, ClosingEventArgs e)
		{
		}

		// ナビゲーションに失敗した場合に実行されるコード
		private void RootFrame_NavigationFailed(object sender, NavigationFailedEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// ナビゲーションに失敗しました。デバッガーで中断します。
				System.Diagnostics.Debugger.Break();
			}
		}

		// ハンドルされない例外の発生時に実行されるコード
		private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
		{
			if (System.Diagnostics.Debugger.IsAttached)
			{
				// ハンドルされない例外が発生しました。デバッガーで中断します。
				System.Diagnostics.Debugger.Break();
			}
		}

		#region Phone アプリケーションの初期化

		// 初期化の重複を回避します
		private bool phoneApplicationInitialized = false;

		// このメソッドに新たなコードを追加しないでください
		private void InitializePhoneApplication()
		{
			if (phoneApplicationInitialized)
				return;

			// フレームを作成しますが、まだ RootVisual に設定しないでください。これによって、アプリケーションがレンダリングできる状態になるまで、
			// スプラッシュ スクリーンをアクティブなままにすることができます。
			RootFrame = new PhoneApplicationFrame();
			RootFrame.Navigated += CompleteInitializePhoneApplication;

			// ナビゲーション エラーを処理します
			RootFrame.NavigationFailed += RootFrame_NavigationFailed;

			// 再初期化しないようにします
			phoneApplicationInitialized = true;
		}

		// このメソッドに新たなコードを追加しないでください
		private void CompleteInitializePhoneApplication(object sender, NavigationEventArgs e)
		{
			// ルート visual を設定してアプリケーションをレンダリングできるようにします
			if (RootVisual != RootFrame)
				RootVisual = RootFrame;

			// このハンドラーは必要なくなったため、削除します
			RootFrame.Navigated -= CompleteInitializePhoneApplication;
		}

		#endregion
	}
}