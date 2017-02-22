using Microsoft.EntityFrameworkCore;

namespace TimeTortoise.DAL
{
	public sealed class SqliteContext : Context
	{
		public SqliteContext()
		{
			Database.Migrate();
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			optionsBuilder.UseSqlite("Filename=TimeTortoise.db");
		}
	}
}
