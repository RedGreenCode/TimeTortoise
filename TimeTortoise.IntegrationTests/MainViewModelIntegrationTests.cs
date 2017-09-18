using System;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using Moq;

using TimeTortoise.DAL;
using TimeTortoise.Model;
using TimeTortoise.ViewModel;
using TimeTortoise.TestHelper;

using Xunit;

namespace TimeTortoise.IntegrationTests
{
	public class MainViewModelIntegrationTests
	{
		// Integration tests

		[Fact]
		public void SelectedActivity_WhenNoActivitySelected_IsNull()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mvm = Helper.GetMainViewModel(context);

					// Act
					var avm = mvm.SelectedActivity;

					// Assert
					Assert.Equal(null, avm);
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
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mvm = Helper.GetMainViewModel(context);

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
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mvm = Helper.GetMainViewModel(context);

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
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mvm = Helper.GetMainViewModel(context);
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
		public void TimeSegment_WhenDeleted_DisappearsFromTimeSegmentList()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mvm = Helper.GetMainViewModel(context);
					mvm.AddActivity();
					mvm.SelectedActivityIndex = 0;
					mvm.AddTimeSegment();
					mvm.Save();
					mvm.LoadActivities();

					//// Simulate the behavior of the ListView
					//mvm.Activities.CollectionChanged += delegate (object sender, NotifyCollectionChangedEventArgs e)
					//{
					//	if (e.Action == NotifyCollectionChangedAction.Remove) mvm.SelectedActivityIndex = -1;
					//};

					// Act
					mvm.SelectedActivityIndex = 0;
					mvm.SelectedTimeSegmentIndex = 0;
					mvm.DeleteTimeSegment();
					mvm.LoadActivities();

					// Assert
					Assert.Equal(0, mvm.SelectedActivity.NumTimeSegments);
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
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mockTime = new Mock<IDateTime>();
					var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					var mvm = Helper.GetMainViewModel(context, mockTime.Object);

					// Act
					mvm.AddActivity();
					mvm.StartStop();
					var endTime = new DateTime(2017, 3, 1, 11, 0, 0);
					mockTime.Setup(x => x.Now).Returns(endTime);
					mvm.StartStop();
					mvm.Save();
					mvm.LoadActivities();

					// Assert
					Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].ObservableTimeSegments[0].StartTime);
					Assert.Equal(endTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].ObservableTimeSegments[0].EndTime);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void TimeSegment_WhenAddedAndSaved_HasCorrectStartAndEndTimes()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mockTime = new Mock<IDateTime>();
					var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					var mvm = Helper.GetMainViewModel(context, mockTime.Object);

					// Act
					mvm.AddActivity();
					mvm.AddTimeSegment();
					mvm.Save();
					mvm.LoadActivities();

					// Assert
					Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].ObservableTimeSegments[0].StartTime);
					Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].ObservableTimeSegments[0].EndTime);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void TimeSegment_WhenAddedAndDeleted_IsDeleted()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mockTime = new Mock<IDateTime>();
					var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					var mvm = Helper.GetMainViewModel(context, mockTime.Object);

					// Act
					mvm.AddActivity();
					mvm.AddTimeSegment();
					mvm.DeleteTimeSegment();

					// Assert
					//Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].ObservableTimeSegments[0].StartTime);
					//Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.Activities[0].ObservableTimeSegments[0].EndTime);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void StartedTimeSegment_WhenDeleted_IsDeleted()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mockTime = new Mock<IDateTime>();
					var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					var mvm = Helper.GetMainViewModel(context, mockTime.Object);

					// Act
					mvm.AddActivity();
					Assert.Equal(0, mvm.SelectedActivity.NumTimeSegments);
					mvm.StartStop();
					mvm.SelectedTimeSegmentIndex = 0;
					Assert.Equal(1, mvm.SelectedActivity.NumTimeSegments);
					mvm.DeleteTimeSegment();

					// Assert
					Assert.Equal(0, mvm.SelectedActivity.NumTimeSegments);
				}
			}
			finally
			{
				connection.Close();
			}
		}

		[Fact]
		public void TimeSegmentList_WhenStartTimeIsSpecified_ShowsCorrectTimeSegments()
		{
			var connection = Helper.GetConnection();
			try
			{
				using (var context = Helper.GetContext(connection))
				{
					// Arrange
					var mockTime = new Mock<IDateTime>();
					var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					var mvm = Helper.GetMainViewModel(context, mockTime.Object);

					// Act
					mvm.AddActivity();
					mvm.AddTimeSegment();
					Assert.Equal(1, mvm.Activities[0].ObservableTimeSegments.Count);
					startTime = new DateTime(2017, 3, 2, 10, 0, 0);
					mockTime.Setup(x => x.Now).Returns(startTime);
					mvm.AddTimeSegment();
					mvm.Save();

					// Assert
					Assert.Equal(2, mvm.Activities[0].ObservableTimeSegments.Count);
					mvm.DateRangeStart = new DateTime(2017, 3, 2, 10, 0, 0);
					Assert.Equal(1, mvm.Activities[0].ObservableTimeSegments.Count);
				}
			}
			finally
			{
				connection.Close();
			}
		}
	}
}
