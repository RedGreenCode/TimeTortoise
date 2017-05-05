using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

using Xunit;
using Moq;

using TimeTortoise.Model;
using TimeTortoise.TestHelper;

namespace TimeTortoise.ViewModel.Tests
{
	public class MainViewModelTests
	{
		[Fact]
		public void MainViewModel_WhenActivitiesAreLoaded_ContainsActivities()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

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
		public void MainViewModel_WhenActivitiesAndTimeSegmentsAreLoaded_ActivityContainsTimeSegments()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.LoadActivities();
			mvm.SelectedActivityIndex = 1;
			mvm.LoadTimeSegments();

			// Assert
			Assert.Equal(4, mvm.SelectedActivity.NumObservableTimeSegments);
		}

		[Fact]
		public void SelectedActivity_WhenNoActivityIsSelected_IsNull()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			var avm = mvm.SelectedActivity;

			// Assert
			Assert.Equal(null, avm);
		}

		[Fact]
		public void SelectedTimeSegment_WhenNoTimeSegmentIsSelected_IsNull()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			var tvm = mvm.SelectedTimeSegment;

			// Assert
			Assert.Equal(null, tvm);
		}

		[Fact]
		public void SetSelectedTimeSegment_WhenSelectedTimeSegmentIndexIsInvalid_SetsSelectedTimeSegmentToNull()
		{
			// Arrange
			var mvm =
				new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
				{
					SelectedActivityIndex = 1,
					SelectedTimeSegmentIndex = 0
				};
			var tvm = mvm.SelectedTimeSegment;
			Assert.NotEqual(null, tvm);

			// Act
			mvm.SelectedTimeSegmentIndex = -1;
			tvm = mvm.SelectedTimeSegment;

			// Assert
			Assert.Equal(null, tvm);
		}

		[Fact]
		public void SelectedTimeSegment_WhenNoActivityIsSelected_IsNull()
		{
			// Arrange
			var mvm =
				new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
				{
					SelectedTimeSegmentIndex = 1
				};

			// Act
			var tvm = mvm.SelectedTimeSegment;

			// Assert
			Assert.Equal(null, tvm);
		}

		[Fact]
		public void SelectedTimeSegmentStartAndEndTimes_WhenNoTimeSegmentIsSelected_AreEmpty()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Assert
			Assert.Equal(string.Empty, mvm.SelectedTimeSegmentStartTime);
			Assert.Equal(string.Empty, mvm.SelectedTimeSegmentEndTime);
		}

		[Fact]
		public void SelectedActivity_WhenNameIsUpdated_IsUpdatedInActivityList()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.AddActivity();
			mvm.SelectedActivity.Name = "TestName1";

			// Assert
			Assert.Equal("TestName1", mvm.Activities[mvm.Activities.Count - 1].Name);
		}

		[Fact]
		public void SelectedTimeSegment_WhenEndTimeIsUpdated_IsUpdatedInTimeSegmentList()
		{
			// Arrange
			var endTime = "3/1/2017 1:00:00 PM";
			var mvm =
				new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
				{
					SelectedActivityIndex = 1,
					SelectedTimeSegmentIndex = 0,
					SelectedTimeSegment = {EndTime = endTime}
				};

			// Assert
			Assert.Equal(endTime, mvm.SelectedActivity.GetTimeSegment(0).EndTime);
			Assert.Equal(endTime, mvm.SelectedTimeSegmentEndTime);
		}

		[Fact]
		public void SelectedActivityIndex_WhenNewActivityIsAdded_PointsToNewActivity()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			mvm.AddActivity();
			mvm.AddActivity();

			// Assert
			Assert.Equal(4, mvm.SelectedActivityIndex);
		}

		[Fact]
		public void SelectedTimeSegment_WhenNewTimeSegmentIsAdded_PointsToNewTimeSegment()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());
			mvm.AddActivity();

			// Act
			mvm.AddTimeSegment();
			mvm.AddTimeSegment();
			var ts = mvm.SelectedActivity.GetTimeSegment(1);

			// Assert
			Assert.Equal(1, mvm.SelectedTimeSegmentIndex);
			Assert.Equal(ts, mvm.SelectedTimeSegment);
		}

		[Fact]
		public void IsSaveEnabled_WhenNewActivityIsAdded_SwitchesToTrue()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			Assert.Equal(false, mvm.IsSaveEnabled);
			mvm.AddActivity();

			// Assert
			Assert.Equal(true, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsTimeSegmentDeleteEnabled_WhenNewTimeSegmentIsAdded_SwitchesToTrue()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
			{
				SelectedActivityIndex = 0
			};
			Assert.Equal(false, mvm.IsTimeSegmentDeleteEnabled);

			// Act
			mvm.AddTimeSegment();

			// Assert
			Assert.Equal(true, mvm.IsTimeSegmentDeleteEnabled);
		}

		[Fact]
		public void IsTimeSegmentAddEnabled_WhenActivityIsSelected_SwitchesToTrue()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			Assert.Equal(false, mvm.IsTimeSegmentAddEnabled);
			mvm.SelectedActivityIndex = 0;

			// Assert
			Assert.Equal(true, mvm.IsTimeSegmentAddEnabled);
		}

		[Fact]
		public void SaveAndDeleteButtons_WhenSelectedTimeSegmentIndexIsInRange_AreEnabled()
		{
			// Arrange
			var mvm =
				new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
				{
					SelectedActivityIndex = 1,
					SelectedTimeSegmentIndex = 3
				};

			// Assert
			Assert.Equal(true, mvm.IsSaveEnabled);
			Assert.Equal(true, mvm.IsTimeSegmentDeleteEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedActivityIndexIsOutOfRangeLow1_IsFalse()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void SaveAndDeleteButtons_WhenSelectedTimeSegmentIndexIsOutOfRangeLow1_AreDisabled()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedActivityIndexIsOutOfRangeLow2_IsFalse()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel()) { SelectedActivityIndex = -1 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedTimeSegmentIndexIsOutOfRangeLow2_IsFalse()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel()) { SelectedTimeSegmentIndex = -1 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedActivityIndexIsOutOfRangeHigh_IsFalse()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel()) { SelectedActivityIndex = 99 };

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void ActivityList_WhenActivityIsDeleted_IsUpdated()
		{
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 0;
			mvm.DeleteActivity();

			// Assert
			Assert.Equal(2, mvm.Activities.Count);
		}

		[Fact]
		public void Activity_WhenAddedThenDeletedThenAdded_IsValid()
		{
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());
			mvm.LoadActivities();

			// Act
			mvm.AddActivity();
			mvm.DeleteActivity();
			mvm.AddActivity();

			// Assert
			Assert.Equal(4, mvm.Activities.Count);
		}

		[Fact]
		public void TimeSegmentList_WhenTimeSegmentIsDeleted_IsUpdated()
		{
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());
			mvm.LoadActivities();

			// Act
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 1;
			mvm.DeleteTimeSegment();

			// Assert
			Assert.Equal(3, mvm.SelectedActivity.NumObservableTimeSegments);
		}

		[Fact]
		public void ActivityName_WhenDeleted_FiresCorrectEvents()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());
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
			var n = 0;
			Assert.Equal("SelectedActivityIndex", receivedEvents[n++]);
			Assert.Equal("IsSaveEnabled", receivedEvents[n++]);
			Assert.Equal("IsTimeSegmentAddEnabled", receivedEvents[n++]);
			Assert.Equal("SelectedActivity", receivedEvents[n++]);
			Assert.True(n > 0);
		}

		[Fact]
		public void StartStopButton_WhenPressed_TogglesButtonText()
		{
			// Arrange
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

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
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

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

			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), mockTime.Object, new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();
			mvm.SelectedTimeSegmentIndex = 0;

			// Assert
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegment.StartTime);
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegmentStartTime);
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegment.EndTime);
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegmentEndTime);
		}

		[Fact]
		public void TimeSegment_WhenTimingEnds_HasCorrectStartAndEndTimes()
		{
			// Arrange
			var mockTime = new Mock<IDateTime>();
			var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
			mockTime.Setup(x => x.Now).Returns(startTime);

			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), mockTime.Object, new ValidationMessageViewModel()) {SelectedActivityIndex = 0};

			// Act
			mvm.StartStop();
			var endTime = new DateTime(2017, 3, 1, 11, 0, 0);
			mockTime.Setup(x => x.Now).Returns(endTime);
			mvm.StartStop();
			mvm.SelectedTimeSegmentIndex = 0;

			// Assert
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegment.StartTime);
			Assert.Equal(startTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegmentStartTime);
			Assert.Equal(endTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegment.EndTime);
			Assert.Equal(endTime.ToString(CultureInfo.CurrentUICulture), mvm.SelectedTimeSegmentEndTime);
		}

		[Fact]
		public void TimeSegment_WhenAlternateStartAndEndAccessorsAreSet_HasCorrectStartAndEndTimes()
		{
			// Arrange
			var mockTime = new Mock<IDateTime>();
			const string startTime = "3/1/2017 10:00:00 AM";
			const string endTime = "3/1/2017 11:00:00 AM";

			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), mockTime.Object, new ValidationMessageViewModel()) { SelectedActivityIndex = 0 };

			// Act
			mvm.AddTimeSegment();
			mvm.SelectedTimeSegmentStartTime = startTime;
			mvm.SelectedTimeSegmentEndTime = endTime;

			// Assert
			Assert.Equal(startTime, mvm.SelectedTimeSegment.StartTime);
			Assert.Equal(endTime, mvm.SelectedTimeSegment.EndTime);
		}

		[Fact]
		public void TimeSegment_WhenTimingEnds_IsSaved()
		{
			// Arrange
			var mockRepository = Helper.GetMockRepository();
			var mockTime = new Mock<IDateTime>();
			var startTime = new DateTime(2017, 3, 1, 10, 0, 0);
			mockTime.Setup(x => x.Now).Returns(startTime);

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
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel());
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
		public void ValidationMessage_WhenStartTimeIsValid_IsEmpty()
		{
			// Arrange
			var vmvm = new ValidationMessageViewModel();
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), vmvm);
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
			var vmvm = new ValidationMessageViewModel();
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), vmvm);
			mvm.LoadActivities();

			// Act
			mvm.AddActivity();
			mvm.AddTimeSegment();
			mvm.SelectedTimeSegment.StartTime = "2/31/2017 1:00 PM";

			// Assert
			Assert.Equal("Please enter a valid start date and time.\r\n", vmvm.ValidationMessages);
		}

		[Fact]
		public void ValidationMessage_WhenEndTimeIsInvalid_HasCorrectMessage()
		{
			// Arrange
			var vmvm = new ValidationMessageViewModel();
			var mvm = new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), vmvm);
			mvm.LoadActivities();

			// Act
			mvm.AddActivity();
			mvm.AddTimeSegment();
			mvm.SelectedTimeSegment.EndTime = "2/31/2017 1:00 PM";

			// Assert
			Assert.Equal("Please enter a valid end date and time.\r\n", vmvm.ValidationMessages);
		}

		[Fact]
		public void SelectedTimeSegmentStartTime_WhenNoTimeSegmentIsSelected_DoesntThrow()
		{
			// Arrange/Act
			var mvm =
				new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
				{
					SelectedTimeSegmentStartTime = string.Empty
				};

			// Assert
			Assert.Equal(null, mvm.SelectedTimeSegment);
		}


		[Fact]
		public void SelectedTimeSegmentEndTime_WhenNoTimeSegmentIsSelected_DoesntThrow()
		{
			// Arrange/Act
			var mvm =
				new MainViewModel(Helper.GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel())
				{
					SelectedTimeSegmentEndTime = string.Empty
				};

			// Assert
			Assert.Equal(null, mvm.SelectedTimeSegment);
		}
	}
}
