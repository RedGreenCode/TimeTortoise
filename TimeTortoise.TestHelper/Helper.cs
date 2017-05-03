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

		public static List<TimeSegment> GetTimeSegments()
		{
			var timeSegments = new List<TimeSegment>
			{
				new TimeSegment
				{
					StartTime = new DateTime(2017, 3, 1, 10, 0, 0),
					EndTime = new DateTime(2017, 3, 1, 11, 15, 33)
				},
				new TimeSegment
				{
					StartTime = new DateTime(2017, 3, 1, 12, 0, 0),
					EndTime = new DateTime(2017, 3, 1, 13, 15, 33)
				},
				new TimeSegment
				{
					StartTime = new DateTime(2017, 3, 1, 13, 30, 0),
					EndTime = new DateTime(2017, 3, 1, 13, 45, 33)
				},
				new TimeSegment
				{
					StartTime = new DateTime(2017, 3, 1, 14, 0, 0),
					EndTime = new DateTime(2017, 3, 1, 14, 16, 39)
				}
			};

			return timeSegments;
		}

		public static List<Activity> GetActivities()
		{

			var activity = new Activity { Id = 1, Name = "Activity2" };
			var timeSegments = GetTimeSegments();
			foreach (var timeSegment in timeSegments)
				activity.TimeSegments.Add(timeSegment);

			return new List<Activity>
			{
				new Activity {Id = 0, Name = "Activity1"},
				activity,
				new Activity {Id = 2, Name = "Activity3"}
			};
		}
	}
}
