using Microsoft.EntityFrameworkCore;
using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public class Context : DbContext
	{
		public DbSet<Activity> Activities { get; set; }

		public DbSet<TimeSegment> TimeSegments { get; set; }

		public Context()
		{
			
		}

		protected Context(DbContextOptions options) : base(options)
		{
			
		}
	}
}
