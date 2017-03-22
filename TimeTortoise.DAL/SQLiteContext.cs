using Microsoft.EntityFrameworkCore;

namespace TimeTortoise.DAL
{
	public sealed class SqliteContext : Context
	{
		private readonly string _localPath;

		public SqliteContext(string localPath)
		{
			_localPath = localPath;
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
				optionsBuilder.UseSqlite($@"Filename={_localPath}\TimeTortoise.db");
			}
		}
	}
}
