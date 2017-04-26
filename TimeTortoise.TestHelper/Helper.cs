using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using TimeTortoise.DAL;
using TimeTortoise.Model;

namespace TimeTortoise.TestHelper
{
	public static class Helper
	{
		// Helper methods
		public static SqliteConnection GetConnection()
		{
			var connection = new SqliteConnection("DataSource=:memory:");
			connection.Open();
			return connection;
		}

		public static Context GetContext(SqliteConnection connection)
		{
			var options = new DbContextOptionsBuilder<SqliteContext>().UseSqlite(connection).Options;
			var context = new SqliteContext(options);
			return context;
		}

		public static List<Activity> GetActivities()
		{
			var timeSegment1 = new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 10, 0, 0),
				EndTime = new DateTime(2017, 3, 1, 11, 15, 33)
			};
			var timeSegment2 = new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 12, 0, 0),
				EndTime = new DateTime(2017, 3, 1, 13, 15, 33)
			};
			var timeSegment3 = new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 13, 30, 0),
				EndTime = new DateTime(2017, 3, 1, 13, 45, 33)
			};
			var timeSegment4 = new TimeSegment
			{
				StartTime = new DateTime(2017, 3, 1, 14, 0, 0),
				EndTime = new DateTime(2017, 3, 1, 14, 16, 39)
			};

			var activity = new Activity { Name = "Activity2" };
			activity.TimeSegments.Add(timeSegment1);
			activity.TimeSegments.Add(timeSegment2);
			activity.TimeSegments.Add(timeSegment3);
			activity.TimeSegments.Add(timeSegment4);

			return new List<Activity>
			{
				new Activity {Name = "Activity1"},
				activity,
				new Activity {Name = "Activity3"}
			};
		}
	}
}
