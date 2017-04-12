using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using Xunit;
using Moq;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel.Tests
{
	public class MainViewModelTests
	{
		public static List<Activity> GetActivities()
		{
			var timeSegment1 = new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 10, 0, 0),
				EndTime = new DateTime(2017, 3, 1, 11, 15, 33)
			};
			var timeSegment2 = new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 12, 0, 0),
				EndTime = new DateTime(2017, 3, 1, 13, 15, 33)
			};

			var activity = new Activity {Name = "Activity2"};
			activity.TimeSegments.Add(timeSegment1);
			activity.TimeSegments.Add(timeSegment2);

			return new List<Activity>
			{
				new Activity {Name = "Activity1"},
				activity,
				new Activity {Name = "Activity3"}
			};
		}

		[Fact]
		public void MainViewModel_WhenActivitiesAreLoaded_ContainsActivities()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.LoadActivities();

			// Assert
			Assert.Equal(3, mvm.Activities.Count);
			Assert.Equal("Activity1", mvm.Activities[0].Name);
			Assert.Equal("Activity2", mvm.Activities[1].Name);
			Assert.Equal("3", mvm.Activities[1].GetTimeSegment(1).StartTime.Substring(0, 1));
			Assert.Equal("Activity3", mvm.Activities[2].Name);
		}

		[Fact]
		public void MainViewModel_WhenActivitiesAreLoaded_ContainsTimeSegments()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.LoadActivities();
			mvm.SelectedActivityIndex = 1;

			// Assert
			Assert.Equal(mvm.SelectedActivity.NumObservableTimeSegments, 2);
		}

		[Fact]
		public void SelectedActivity_WhenNoActivityIsSelected_HasNullName()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			var avm = mvm.SelectedActivity;

			// Assert
			Assert.Equal(null, avm.Name);
		}

		[Fact]
		public void SelectedTimeSegment_WhenNoTimeSegmentIsSelected_HasEmptyStartAndEndTimes()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			var tvm = mvm.SelectedTimeSegment;

			// Assert
			Assert.Equal(string.Empty, tvm.StartTime);
			Assert.Equal(string.Empty, tvm.EndTime);
		}

		[Fact]
		public void SelectedActivity_WhenNameIsUpdated_IsUpdatedInActivityList()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.AddActivity();
			mvm.SelectedActivity.Name = "TestName1";

			// Assert
			Assert.Equal("TestName1", mvm.Activities[0].Name);
		}

		[Fact]
		public void SelectedTimeSegment_WhenEndTimeIsUpdated_IsUpdatedInTimeSegmentList()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 0;
			mvm.SelectedTimeSegment.EndTime = "3/1/2017 1:00:00 PM";

			// Assert
			Assert.Equal("3/1/2017 1:00:00 PM", mvm.SelectedActivity.GetTimeSegment(0).EndTime);
		}

		[Fact]
		public void SelectedActivityIndex_WhenNewActivityIsAdded_PointsToNewActivity()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.AddActivity();
			mvm.AddActivity();

			// Assert
			Assert.Equal(1, mvm.SelectedActivityIndex);
		}

		[Fact]
		public void SelectedTimeSegmentIndex_WhenNewTimeSegmentIsAdded_PointsToNewTimeSegment()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.SelectedActivityIndex = 0;
			mvm.AddTimeSegment();
			mvm.AddTimeSegment();

			// Assert
			Assert.Equal(1, mvm.SelectedTimeSegmentIndex);
		}

		[Fact]
		public void IsSaveEnabled_WhenNewActivityIsAdded_SwitchesToTrue()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			Assert.Equal(false, mvm.IsSaveEnabled);
			mvm.AddActivity();

			// Assert
			Assert.Equal(true, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenNewTimeSegmentIsAdded_SwitchesToTrue()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			Assert.Equal(false, mvm.IsSaveEnabled);
			mvm.AddTimeSegment();

			// Assert
			Assert.Equal(true, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedActivityIndexIsOutOfRangeLow1_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedTimeSegmentIndexIsOutOfRangeLow1_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedActivityIndexIsOutOfRangeLow2_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel()) { SelectedActivityIndex = -1 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedTimeSegmentIndexIsOutOfRangeLow2_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel()) { SelectedTimeSegmentIndex = -1 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedActivityIndexIsOutOfRangeHigh_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel()) { SelectedTimeSegmentIndex = 99 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedTimeSegmentIndexIsOutOfRangeHigh_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel()) { SelectedTimeSegmentIndex = 99 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		// This test passes even when mvm.SelectedActivity.Name is not changed. All of the events
		// are firing due to mvm.AddActivity().
		//[Fact]
		//public void ActivityName_WhenChanged_FiresCorrectEvents()
		//{
		//	// Arrange
		//	var mockRepository = new Mock<IRepository>();
		//	mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
		//	var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
		//	// http://stackoverflow.com/a/249042/4803
		//	var receivedEvents = new List<string>();
		//	mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
		//	{
		//		receivedEvents.Add(e.PropertyName);
		//	};

		//	// Act
		//	mvm.AddActivity();
		//	//mvm.SelectedActivity.Name = "TestName1";

		//	// Assert
		//	Assert.Equal("SelectedActivityIndex", receivedEvents[0]);
		//	Assert.Equal("IsSaveEnabled", receivedEvents[1]);
		//	Assert.Equal("SelectedActivity", receivedEvents[2]);
		//}

		//[Fact]
		//public void TimeSegmentEndTime_WhenChanged_FiresCorrectEvents()
		//{
		//	// Arrange
		//	var mockRepository = new Mock<IRepository>();
		//	mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
		//	var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
		//	var receivedEvents = new List<string>();
		//	mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
		//	{
		//		receivedEvents.Add(e.PropertyName);
		//	};

		//	// Act
		//	mvm.AddActivity();
		//	mvm.SelectedTimeSegment.EndTime = "3/1/2017 1:00:00 PM";

		//	// Assert
		//	Assert.Equal("SelectedActivityIndex", receivedEvents[0]);
		//	Assert.Equal("IsSaveEnabled", receivedEvents[1]);
		//	Assert.Equal("SelectedActivity", receivedEvents[2]);
		//}

		[Fact]
		public void ActivityList_WhenActivityIsDeleted_IsUpdated()
		{
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 0;
			mvm.DeleteActivity();

			// Assert
			Assert.Equal(2, mvm.Activities.Count);
		}

		[Fact]
		public void TimeSegmentList_WhenTimeSegmnetIsDeleted_IsUpdated()
		{
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 1;
			mvm.DeleteTimeSegment();

			// Assert
			Assert.Equal(1, mvm.SelectedActivity.NumObservableTimeSegments);
		}

		[Fact]
		public void ActivityName_WhenDeleted_FiresCorrectEvents()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			// http://stackoverflow.com/a/249042/4803
			var receivedEvents = new List<string>();
			mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				receivedEvents.Add(e.PropertyName);
			};

			// Act
			mvm.AddActivity();
			mvm.SelectedActivity.Name = "TestName1";

			// Assert
			Assert.Equal("SelectedActivityIndex", receivedEvents[0]);
			Assert.Equal("IsSaveEnabled", receivedEvents[1]);
			Assert.Equal("SelectedActivity", receivedEvents[2]);
		}

		[Fact]
		public void StartStopButton_WhenPressed_TogglesButtonText()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();

			// Assert
			Assert.Equal("Stop", mvm.StartStopText);
			mvm.StartStop();
			Assert.Equal("Start", mvm.StartStopText);
		}

		[Fact]
		public void TimeSegment_WhenStartAndEndAreOneHourApart_HasElapsedTimeOfOneHour()
		{
			// Arrange
			var startTime = new Mock<IDateTime>();
			startTime.Setup(x => x.Now).Returns(new DateTime(2017, 3, 1, 10, 0, 0));
			var endTime = new Mock<IDateTime>();
			endTime.Setup(x => x.Now).Returns(new DateTime(2017, 3, 1, 11, 0, 0));

			// Act
			var elapsedTime = endTime.Object.Now - startTime.Object.Now;

			// Assert
			Assert.Equal(1, elapsedTime.Hours);
		}

		[Fact]
		public void TimeSegments_WhenTimingStarts_HasOneEntry()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();

			// Assert
			Assert.Equal(1, mvm.SelectedActivity.NumObservableTimeSegments);
		}

		[Fact]
		public void TimeSegment_WhenTimingStarts_HasCorrectStartAndEndTimes()
		{
			// Arrange
			var mockTime = new Mock<IDateTime>();
			var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
			mockTime.Setup(x => x.Now).Returns(startTime);

			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, mockTime.Object, new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();

			// Assert
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedActivity.GetTimeSegment(0).StartTime);
			Assert.Equal(string.Empty, mvm.SelectedActivity.GetTimeSegment(0).EndTime);
		}

		[Fact]
		public void TimeSegment_WhenTimingEnds_HasCorrectStartAndEndTimes()
		{
			// Arrange
			var mockTime = new Mock<IDateTime>();
			var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
			mockTime.Setup(x => x.Now).Returns(startTime);

			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, mockTime.Object, new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();
			var endTime = new DateTime(2017, 3, 1, 11, 0, 0);
			mockTime.Setup(x => x.Now).Returns(endTime);
			mvm.StartStop();

			// Assert
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedActivity.GetTimeSegment(0).StartTime);
			Assert.Equal(endTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedActivity.GetTimeSegment(0).EndTime);
		}

		[Fact]
		public void TimeSegment_WhenTimingEnds_IsSaved()
		{
			// Arrange
			var mockTime = new Mock<IDateTime>();
			var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
			mockTime.Setup(x => x.Now).Returns(startTime);

			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, mockTime.Object, new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();
			var endTime = new DateTime(2017, 3, 1, 11, 0, 0);
			mockTime.Setup(x => x.Now).Returns(endTime);
			mvm.StartStop();

			// Assert
			mockRepository.Verify(x => x.SaveActivity());
		}

		[Fact]
		public void TimeSegmentList_WhenTimerStops_UpdatesUI()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			var receivedEvents = new List<string>();

			// Act
			mvm.AddActivity();
			mvm.SelectedActivityIndex = 0;
			mvm.SelectedActivity.ObservableTimeSegments.CollectionChanged += delegate (object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
			{
				receivedEvents.Add(e.Action.ToString());
			};
			mvm.StartStop();
			mvm.StartStop();

			// Assert
			Assert.Equal("Add", receivedEvents[0]);
			Assert.Equal("Replace", receivedEvents[1]);
		}

		[Fact]
		public void TimeSegmentList_WhenTimeSegmentIsDeleted_IsUpdated()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 0;
			mvm.DeleteTimeSegment();

			// Assert
			Assert.Equal(1, mvm.SelectedActivity.NumObservableTimeSegments);
		}

		[Fact]
		public void ValidationMessage_WhenStartTimeIsValid_IsEmpty()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var vmvm = new ValidationMessageViewModel();
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), vmvm);
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 0;
			mvm.SelectedTimeSegment.StartTime = "3/1/2017 1:00 PM";

			// Assert
			Assert.Equal(string.Empty, vmvm.ValidationMessages);
		}

		[Fact]
		public void ValidationMessage_WhenStartTimeIsInvalid_HasCorrectMessage()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var vmvm = new ValidationMessageViewModel();
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), vmvm);
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 0;
			mvm.SelectedTimeSegment.StartTime = "2/31/2017 1:00 PM";

			// Assert
			Assert.Equal("Please enter a valid start date and time.\r\n", vmvm.ValidationMessages);
		}

		[Fact]
		public void ValidationMessage_WhenEndTimeIsInvalid_HasCorrectMessage()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			var vmvm = new ValidationMessageViewModel();
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), vmvm);
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 0;
			mvm.SelectedTimeSegment.EndTime = "2/31/2017 1:00 PM";

			// Assert
			Assert.Equal("Please enter a valid end date and time.\r\n", vmvm.ValidationMessages);
		}
	}
}
