using Microsoft.EntityFrameworkCore;

namespace TimeTortoise.DAL
{
	public sealed class SqliteContext : Context
	{
		public SqliteContext()
		{
			Database.Migrate();
		}

		public SqliteContext(DbContextOptions options) : base(options)
		{
			Database.Migrate();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			if (!optionsBuilder.IsConfigured)
			{
				optionsBuilder.UseSqlite("Filename=TimeTortoise.db");
			}
		}
	}
}
