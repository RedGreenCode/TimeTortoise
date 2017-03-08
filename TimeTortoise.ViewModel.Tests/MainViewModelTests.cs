using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xunit;
using Moq;
using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel.Tests
{
	public class MainViewModelTests
	{
		[Fact]
		public void MainViewModel_WhenActivitiesAreLoaded_ContainsActivities()
		{
			// Arrange
			var activities = new List<Activity>
			{
				new Activity {Name = "TestName1"},
				new Activity {Name = "TestName2"}
			};
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(activities);
			var mvm = new MainViewModel(mockRepository.Object);

			// Act
			mvm.LoadActivities();

			// Assert
			Assert.Equal(2, mvm.Activities.Count);
			Assert.Equal("TestName1", mvm.Activities[0].Name);
			Assert.Equal("TestName2", mvm.Activities[1].Name);
		}

		[Fact]
		public void SelectedActivity_WhenNoActivityIsSelected_HasNullName()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Act
			var avm = mvm.SelectedActivity;

			// Assert
			Assert.Equal(null, avm.Name);
		}

		[Fact]
		public void SelectedActivity_WhenNameIsUpdated_IsUpdatedInActivityList()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Act
			mvm.Add();
			mvm.SelectedActivity.Name = "TestName1";

			// Assert
			Assert.Equal("TestName1", mvm.Activities[0].Name);
		}

		[Fact]
		public void SelectedIndex_WhenNewActivityIsAdded_PointsToNewActivity()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Act
			mvm.Add();
			mvm.Add();

			// Assert
			Assert.Equal(1, mvm.SelectedIndex);
		}

		[Fact]
		public void IsSaveEnabled_WhenNewActivityIsAdded_SwitchesToTrue()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Act
			Assert.Equal(false, mvm.IsSaveEnabled);
			mvm.Add();

			// Assert
			Assert.Equal(true, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedIndexIsOutOfRangeLow1_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedIndexIsOutOfRangeLow2_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Act
			mvm.SelectedIndex = -1;

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void IsSaveEnabled_WhenSelectedIndexIsOutOfRangeHigh_IsFalse()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object) {SelectedIndex = 1};

			// Assert
			Assert.Equal(false, mvm.IsSaveEnabled);
		}

		[Fact]
		public void Save_WhenListIsEmpty_Throws()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

			// Act/Assert
			Assert.Throws<InvalidOperationException>(() => mvm.Save());
		}

		[Fact]
		public void ActivityName_WhenChanged_FiresCorrectEvents()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);
			// http://stackoverflow.com/a/249042/4803
			var receivedEvents = new List<string>();
			mvm.PropertyChanged += delegate(object sender, PropertyChangedEventArgs e)
			{
				receivedEvents.Add(e.PropertyName);
			};

			// Act
			mvm.Add();
			mvm.SelectedActivity.Name = "TestName1";

			// Assert
			Assert.Equal("SelectedIndex", receivedEvents[0]);
			Assert.Equal("IsSaveEnabled", receivedEvents[1]);
			Assert.Equal("SelectedActivity", receivedEvents[2]);
		}

		[Fact]
		public void ActivityList_WhenActivityIsDeleted_IsUpdated()
		{
			var activities = new List<Activity>
			{
				new Activity {Name = "TestName1"},
				new Activity {Name = "TestName2"}
			};
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(activities);
			var mvm = new MainViewModel(mockRepository.Object);
			mvm.LoadActivities();

			// Act
			mvm.SelectedIndex = 0;
			mvm.Delete();

			// Assert
			Assert.Equal(1, mvm.Activities.Count);
		}

		[Fact]
		public void ActivityName_WhenDeleted_FiresCorrectEvents()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);
			// http://stackoverflow.com/a/249042/4803
			var receivedEvents = new List<string>();
			mvm.PropertyChanged += delegate (object sender, PropertyChangedEventArgs e)
			{
				receivedEvents.Add(e.PropertyName);
			};

			// Act
			mvm.Add();
			mvm.SelectedActivity.Name = "TestName1";

			// Assert
			Assert.Equal("SelectedIndex", receivedEvents[0]);
			Assert.Equal("IsSaveEnabled", receivedEvents[1]);
			Assert.Equal("SelectedActivity", receivedEvents[2]);
		}

		[Fact]
		public void StartStopButton_WhenPressed_TogglesButtonText()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(new List<Activity>());
			var mvm = new MainViewModel(mockRepository.Object);

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
	}
}
