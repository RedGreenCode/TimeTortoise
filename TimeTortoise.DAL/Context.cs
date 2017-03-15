using Microsoft.EntityFrameworkCore;
using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public class Context : DbContext
	{
		internal DbSet<Activity> Activities { get; set; }

		internal DbSet<TimeSegment> TimeSegments { get; set; }

		protected Context()
		{
			
		}

		protected Context(DbContextOptions options) : base(options)
		{
			
		}
	}
}
