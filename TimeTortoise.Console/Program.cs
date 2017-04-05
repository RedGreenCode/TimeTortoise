using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

using Moq;

using TimeTortoise.DAL;
using TimeTortoise.Model;
using TimeTortoise.ViewModel;

namespace TimeTortoise.Console
{
	public class Program
	{
		private void PrintAddEvents()
		{
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				System.Console.WriteLine($"Received event for property {e.PropertyName}");
			};
			mvm.AddActivity();
			mvm.SelectedActivity.Name = "TestName1";
		}

		private void PrintDeleteEvents()
		{
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				System.Console.WriteLine($"Received property changed event for property {e.PropertyName}");
			};
			mvm.Activities.CollectionChanged += delegate(object sender, NotifyCollectionChangedEventArgs e)
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

		public static void Main(string[] args)
		{
			try
			{
				var p = new Program();
				//p.PrintAddEvents();
				p.PrintDeleteEvents();
			}
			catch (Exception e)
			{
				System.Console.WriteLine(e.Message);
			}
		}
	}
}
