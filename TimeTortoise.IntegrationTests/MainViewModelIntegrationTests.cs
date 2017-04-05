using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Moq;
using TimeTortoise.DAL;
using TimeTortoise.Model;
using TimeTortoise.ViewModel;

using Xunit;

namespace TimeTortoise.IntegrationTests
{
	public class MainViewModelIntegrationTests
	{
		// Helper methods
		private static SqliteConnection GetConnection()
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();
			return connection;
		}

		private static Context GetContext(SqliteConnection connection)
		{
			var options = new DbContextOptionsBuilder<SqliteContext>().UseSqlite(connection).Options;
			var context = new SqliteContext(options);
			return context;
		}

		// Integration tests

		[Fact]
		public void SelectedActivity_WhenNoActivitySelected_HasNullName()
		{
			var connection = GetConnection();
			try
			{
				using (var context = GetContext(connection))
				{
					// Arrange
					var mvm = new MainViewModel(new Repository(context), new SystemDateTime(), new ValidationMessageViewModel());

					// Act
					var avm = mvm.SelectedActivity;

					// Assert
					Assert.Equal(null, avm.Name);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void ActivitiesList_WithEmptyDatabase_IsEmpty()
		{
			var connection = GetConnection();
			try
			{
				using (var context = GetContext(connection))
				{
					// Arrange
					var mvm = new MainViewModel(new Repository(context), new SystemDateTime(), new ValidationMessageViewModel());

					// Act
					mvm.LoadActivities();

					// Assert
					Assert.Equal(0, mvm.Activities.Count);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void ActivitiesList_WithEmptyDatabaseOnDisk_IsEmpty()
		{
			// Arrange
			var mvm = new MainViewModel(Path.GetTempPath(), new ValidationMessageViewModel());

			// Act
			mvm.LoadActivities();

			// Assert
			Assert.Equal(0, mvm.Activities.Count);
		}

		[Fact]
		public void Activity_WhenSaved_AppearsInActivityList()
		{
			var connection = GetConnection();
			try
			{
				using (var context = GetContext(connection))
				{
					// Arrange
					var mvm = new MainViewModel(new Repository(context), new SystemDateTime(), new ValidationMessageViewModel());

					// Act
					mvm.AddActivity();
					mvm.Save();
					mvm.LoadActivities();

					// Assert
					Assert.Equal(1, mvm.Activities.Count);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void Activity_WhenDeleted_DisappearsFromActivityList()
		{
			var connection = GetConnection();
			try
			{
				using (var context = GetContext(connection))
				{
					// Arrange
					var mvm = new MainViewModel(new Repository(context), new SystemDateTime(), new ValidationMessageViewModel());
					mvm.AddActivity();
					mvm.Save();
					mvm.LoadActivities();

					// Simulate the behavior of the ListView
					mvm.Activities.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
					{
						if (e.Action == NotifyCollectionChangedAction.Remove) mvm.SelectedActivityIndex = -1;
					};

					// Act
					mvm.SelectedActivityIndex = 0;
					mvm.DeleteActivity();
					mvm.LoadActivities();

					// Assert
					Assert.Equal(0, mvm.Activities.Count);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void SystemDateTime_ReturnsCurrentDateTime()
		{
			// Arrange
			var sdt = new SystemDateTime();

			// Assert
			Assert.Equal(DateTime.Now.ToString(CultureInfo.InvariantCulture), sdt.Now.ToString(CultureInfo.InvariantCulture));
		}

		[Fact]
		public void TimeSegment_WhenSaved_HasCorrectStartAndEndTimes()
		{
			var connection = GetConnection();
			try
			{
				using (var context = GetContext(connection))
				{
					// Arrange
					var mockTime = new Mock<IDateTime>();
					var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					var mvm = new MainViewModel(new Repository(context), mockTime.Object, new ValidationMessageViewModel());

					// Act
					mvm.AddActivity();
					mvm.StartStop();
					var endTime = new DateTime(2017, 3, 1, 11, 0, 0);
					mockTime.Setup(x => x.Now).Returns(endTime);
					mvm.StartStop();
					mvm.Save();
					mvm.LoadActivities();

					// Assert
					Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].TimeSegments[0].StartTime);
					Assert.Equal(endTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].TimeSegments[0].EndTime);
				}
			}
			finally
			{
				connection.Close();
			}
		}
	}
}
