using System;
using Moq;
using TimeTortoise.DAL;
using TimeTortoise.Model;
using Xunit;

namespace TimeTortoise.ViewModel.Tests.ExceptionTests
{
	/// <summary>
	/// This class contains unit tests that verify that exceptions are thrown when required.
	/// The Microsoft code coverage system thinks these tests are not completely executed,
	/// so it's easier to interpret coverage results when they are kept separate from other tests.
	/// </summary>
	public class ExceptionTests
	{
		[Fact]
		public void StartStop_WhenNoActivityIsSelected_Throws()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(MainViewModelTests.GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			// https://www.richard-banks.org/2015/07/stop-using-assertthrows-in-your-bdd.html
			var exception = Record.Exception(() => mvm.StartStop());

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidOperationException>(exception);
		}

		[Fact]
		public void AddTimeSegment_WhenNoActivityIsSelected_Throws()
		{
			// Arrange
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(MainViewModelTests.GetActivities());
			var mvm = new MainViewModel(mockRepository.Object, new SystemDateTime(), new ValidationMessageViewModel());

			// Act
			var exception = Record.Exception(() => mvm.AddTimeSegment());

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidOperationException>(exception);
		}
	}
}
