using Xunit;
using TimeTortoise.DAL;

namespace TimeTortoise.ViewModel.Tests
{
	/// <summary>
	/// These aren't really tests. They are workarounds for various limitations
	/// and inconveniences related to code coverage and static analysis.
	/// (Don't abuse this class. Write real tests when possible).
	/// </summary>
	public class ContextTests
	{
		[Fact]
		public void UseDbSetAccessors()
		{
			// The DbSet accessors in the Context class don't all appear to be used
			// as far as code coverage can tell, but they can't be removed.
			var dbc = new Context {Activities = null};
			var ts = dbc.TimeSegments;
		}
	}
}
