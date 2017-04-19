using TimeTortoise.DAL;
using TimeTortoise.Model;
using Xunit;

namespace TimeTortoise.IntegrationTests
{
	/// <summary>
	/// These aren't really tests. They are workarounds for various limitations
	/// and inconveniences related to code coverage and static analysis.
	/// (Don't abuse this class. Write real tests when possible).
	/// </summary>
	public class ContextTests
	{
		[Fact]
		public void UseAccessors()
		{
			// A number of accessors are used by Entity Framework, but code analysis tools don't know that.
			// Artificially use them here to indicate that they are useful and should not be removed.
			var dbc = new Context {Activities = null};
			var ts = dbc.TimeSegments;
			Assert.NotNull(ts);
			dbc.TimeSegments = null;

			var activity = new Activity();
			var activityId = activity.Id;
			Assert.Equal(0, activityId);
			activity.Id = 0;

			var timeSegment = new TimeSegment();
			var timeSegmentId = timeSegment.Id;
			Assert.Equal(0, timeSegmentId);
			timeSegment.Id = 0;
			activityId = timeSegment.ActivityId;
			Assert.Equal(0, activityId);
			timeSegment.ActivityId = 0;
		}
	}
}
