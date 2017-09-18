using System;

using Moq;
using Xunit;

using TimeTortoise.DAL;
using TimeTortoise.TestHelper;
using TimeTortoise.Model;

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
			var mvm = Helper.GetMainViewModel();

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
			var mvm = Helper.GetMainViewModel();

			// Act
			var exception = Record.Exception(() => mvm.AddTimeSegment());

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidOperationException>(exception);
		}

		[Fact]
		public void LoadTimeSegments_WhenNoActivityIsSelected_Throws()
		{
			// Arrange
			var mvm = Helper.GetMainViewModel();

			// Act
			var exception = Record.Exception(() => mvm.LoadTimeSegments());

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidOperationException>(exception);
		}

		[Fact]
		public void SetSelectedTimeSegmentIndex_WhenSelectedTimeSegmentIndexIsInvalid_Throws()
		{
			// Arrange
			var mvm = Helper.GetMainViewModel();
			mvm.SelectedActivityIndex = 1;
			mvm.SelectedTimeSegmentIndex = 0;

			// Act
			var exception = Record.Exception(() => mvm.SelectedTimeSegmentIndex = mvm.SelectedActivity.NumTimeSegments);

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<IndexOutOfRangeException>(exception);
		}

		[Fact]
		public void DeleteTimeSegment_WhenNoTimeSegmentIsSelected_Throws()
		{
			var mvm = Helper.GetMainViewModel();
			mvm.LoadActivities();

			// Act
			mvm.AddActivity();
			var exception = Record.Exception(() => mvm.DeleteTimeSegment());

			// Assert
			Assert.NotNull(exception);
			Assert.IsType<InvalidOperationException>(exception);
		}
	}
}
