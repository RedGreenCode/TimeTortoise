using System.Collections.Specialized;
using System.Globalization;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using TimeTortoise.DAL;
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
					var mvm = new MainViewModel(new Repository(context));

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
					var mvm = new MainViewModel(new Repository(context));

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
			var mvm = new MainViewModel();

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
					var mvm = new MainViewModel(new Repository(context));

					// Act
					mvm.Add();
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
					var mvm = new MainViewModel(new Repository(context));
					mvm.Add();
					mvm.Save();
					mvm.LoadActivities();

					// Simulate the behavior of the ListView
					mvm.Activities.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
					{
						if (e.Action == NotifyCollectionChangedAction.Remove) mvm.SelectedIndex = -1;
					};

					// Act
					mvm.SelectedIndex = 0;
					mvm.Delete();
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
			var sdt = new Model.SystemDateTime();

			// Assert
			Assert.Equal(System.DateTime.Now.ToString(CultureInfo.InvariantCulture), sdt.Now.ToString(CultureInfo.InvariantCulture));
		}
	}
}
