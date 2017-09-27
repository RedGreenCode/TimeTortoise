using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading;

using Moq;
using TimeTortoise.TestHelper;

using TimeTortoise.DAL;
using TimeTortoise.Model;
using TimeTortoise.ViewModel;
using TimeTortoise.Client;

namespace TimeTortoise.Console
{
	public class Program
	{
		private void PrintAddEvents()
		{
			var mvm = Helper.GetMainViewModel();
			mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				System.Console.WriteLine($"Received event for property {e.PropertyName}");
			};
			mvm.AddActivity();
			mvm.SelectedActivity.Name = "TestName1";
		}

		private void PrintDeleteEvents()
		{
			var mvm = Helper.GetMainViewModel();
			mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				System.Console.WriteLine($"Received property changed event for property {e.PropertyName}");
			};
			mvm.Activities.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
			{
				System.Console.WriteLine($"Received collection changed event for action {e.Action}");
			};
			System.Console.WriteLine("Add");
			mvm.AddActivity();
			System.Console.WriteLine("Add");
			mvm.AddActivity();
			System.Console.WriteLine("Select");
			mvm.SelectedActivityIndex = 1;
			System.Console.WriteLine("Delete");
			mvm.DeleteActivity();
		}

		private void PrintTimeSegments()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					var mvm = Helper.GetMainViewModel();
					var activities = Helper.GetActivities();
					foreach (var activity in activities)
					{
						mvm.AddActivity();
						mvm.SelectedActivity.Name = activity.Name;
						foreach (var timeSegment in activity.TimeSegments)
						{
							mvm.AddTimeSegment();
							mvm.SelectedTimeSegment.StartTime = timeSegment.StartTime.ToString();
							mvm.SelectedTimeSegment.EndTime = timeSegment.EndTime.ToString();
						}
					}
					mvm.Save();
				}
				using (var context = Helper.GetContext(connection))
				{
					var mvm = Helper.GetMainViewModel();
					mvm.LoadActivities();
					var selectedActivityIndex = 0;
					foreach (var activity in mvm.Activities)
					{
						mvm.SelectedActivityIndex = selectedActivityIndex++;
						mvm.LoadTimeSegments();
						foreach (var timeSegment in mvm.SelectedActivity.ObservableTimeSegments)
						{
							System.Console.WriteLine($"{timeSegment.StartTime} - {timeSegment.EndTime}");
						}
					}
				}
			}
			finally
			{
				connection.Close();
			}
		}

		private void StartClient()
		{
			var client = new SignalRClient("http://127.0.0.1:8080");
			client.ConnectToServer();
			while (true)
			{
				var message = client.GetNewestMessage();
				if (message > DateTime.MinValue)
				{
					System.Console.WriteLine(message);
				}

				Thread.Sleep(1000);
			}
		}

		private void GetContext()
		{
			var connection = Helper.GetConnection();
			var context = Helper.GetContext(connection);
		}

		public static void Main(string[] args)
		{
			try
			{
				var p = new Program();
				//p.PrintAddEvents();
				//p.PrintDeleteEvents();
				//p.PrintTimeSegments();
				p.GetContext();
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
		}
	}
}
