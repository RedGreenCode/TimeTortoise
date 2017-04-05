using System;
using TimeTortoise.Model;
using Xunit;

namespace TimeTortoise.ViewModel.Tests
{
	public class TimeSegmentViewModelTests
	{
		[Fact]
		public void TimeSegmentViewModel_WhenBothStartAndEndTimeExist_FormatsDurationCorrectly()
		{
			// Arrange
			var tsvm = new TimeSegmentViewModel(new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 10, 0, 0),
				EndTime = new DateTime(2017, 3, 1, 11, 15, 35)
			}, new ValidationMessageViewModel());

			// Act
			var duration = tsvm.Duration;

			// Assert
			Assert.Equal("01:15:35", duration);
		}

		[Fact]
		public void TimeSegmentViewModel_WhenEndTimeIsMissing_FormatsDurationCorrectly()
		{
			// Arrange
			var tsvm = new TimeSegmentViewModel(new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 10, 0, 0)
			}, new ValidationMessageViewModel());

			// Act
			var duration = tsvm.Duration;

			// Assert
			Assert.Equal(string.Empty, duration);
		}
	}
}
