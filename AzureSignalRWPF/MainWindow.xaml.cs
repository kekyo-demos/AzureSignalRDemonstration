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

using AzureSignalR;
using Microsoft.AspNet.SignalR.Client;
using Newtonsoft.Json;
using System;
using System.Deployment.Application;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AzureSignalRWPF
{
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow : Window
	{
		private HubConnection client_;
		private IHubProxy shareWhiteBoardHub_;

		private Point lastPoint_;
		private bool captured_;

		public MainWindow()
		{
			InitializeComponent();

			var colors =
				(from red in new byte[] { 0, 127, 255 }
				 from green in new byte[] { 0, 127, 255 }
				 from blue in new byte[] { 0, 127, 255 }
				 let color = Color.FromArgb(255, red, green, blue)
				 let brush = new SolidColorBrush(color)
				 select Tuple.Create(color, brush)).
				ToArray();

			colorBox.ItemsSource = colors;
			colorBox.SelectedIndex = 0;
		}

		private async void UserControl_Loaded(object sender, RoutedEventArgs e)
		{
			var url = (ApplicationDeployment.IsNetworkDeployed == true) ?
				ApplicationDeployment.CurrentDeployment.ActivationUri :
				new Uri("http://ENTER-YOUR-INSTASNCE.cloudapp.net");

			client_ = new HubConnection(url.GetLeftPart(UriPartial.Authority) + "/");
			client_.TraceLevel = TraceLevels.All;
			client_.TransportConnectTimeout = new TimeSpan(0, 0, 7);

			shareWhiteBoardHub_ = client_.CreateHubProxy("SharedWhiteBoardHub");
			shareWhiteBoardHub_.On<LineInformation>("DrawLine", this.OnDrawLine);

			await client_.Start();

			whiteBoard.Visibility = Visibility.Visible;
			progressBar.Visibility = Visibility.Collapsed;
		}

		private void whiteBoard_DragEnter(object sender, MouseButtonEventArgs e)
		{
			lastPoint_ = e.GetPosition(whiteBoard);
			captured_ = true;
		}

		private void InvokePlaceLine(Point point)
		{
			if ((point.X < 0) || (point.Y < 0))
			{
				return;
			}

			var color = ((Tuple<Color, SolidColorBrush>)colorBox.SelectedItem).Item1;

			var lineInformation = new LineInformation
			{
				X0 = lastPoint_.X,
				Y0 = lastPoint_.Y,
				X1 = point.X,
				Y1 = point.Y,
				Alpha = color.A,
				Red = color.R,
				Green = color.G,
				Blue = color.B
			};

			lastPoint_ = point;

			Debug.WriteLine(string.Format("InvokePlaceLine: {0}", JsonConvert.SerializeObject(lineInformation)));

			shareWhiteBoardHub_.Invoke("PlaceLine", lineInformation);
		}

		private void whiteBoard_DragOver(object sender, MouseEventArgs e)
		{
			if (captured_ == false)
			{
				return;
			}

			var canvas = (Canvas)sender;
			var point = e.GetPosition(canvas);

			this.InvokePlaceLine(point);
		}

		private void whiteBoard_DragLeave(object sender, MouseButtonEventArgs e)
		{
			if (captured_ == false)
			{
				return;
			}

			captured_ = false;

			var canvas = (Canvas)sender;
			var point = e.GetPosition(canvas);

			this.InvokePlaceLine(point);
		}

		private void OnDrawLine(LineInformation lineInformation)
		{
			Debug.WriteLine(string.Format("OnDrawLine: {0}", JsonConvert.SerializeObject(lineInformation)));

			this.Dispatcher.BeginInvoke(new Action(() =>
			{
				var polyline = new Polyline
				{
					Stroke = new SolidColorBrush(Color.FromArgb(
						lineInformation.Alpha,
						lineInformation.Red,
						lineInformation.Green,
						lineInformation.Blue)),
					StrokeThickness = 2
				};

				polyline.Points.Add(new Point(lineInformation.X0, lineInformation.Y0));
				polyline.Points.Add(new Point(lineInformation.X1, lineInformation.Y1));

				whiteBoard.Children.Add(polyline);
			}));
		}
	}
}
