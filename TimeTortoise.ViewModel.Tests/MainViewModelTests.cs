using System.Collections.Generic;
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
	}
}
