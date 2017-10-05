using System;
using System.IO;
using System.Text;
using TimeTortoise.Model;

using TimeTortoise.DAL;
using System.Collections.Generic;

namespace TimeTortoise.ViewModel
{
	public class DailySummary
	{
		private readonly ISettingsUtility _settingsUtility;
		private readonly IRepository _repository;

		public DailySummary(ISettingsUtility settingsUtility, IRepository repository)
		{
			_settingsUtility = settingsUtility;
			_repository = repository;
		}

		public DateTime LastUpdated { get; private set; }

		public void WriteFile()
		{
			var timeSinceLastUpdate = DateTime.Now - LastUpdated;
			if (timeSinceLastUpdate.TotalSeconds < _settingsUtility.Settings.DailySummaryUpdateIntervalSeconds) return;

			var date = DateTime.Today;
			var endDate = date.Add(new TimeSpan(24, 59, 59, 999));
			var daysInYear = DateTime.IsLeapYear(date.Year) ? 366 : 365;

			var folderName = $"{date:yyyy-MMMM}";
			var fullName = $@"{_settingsUtility.SettingsPath}\Daily\{folderName}\{date:yyyyMMdd-dddd}.txt";
			var fileInfo = new FileInfo(fullName);
			fileInfo.Directory.Create();

			var hr1 = new string('#', 100);
			var hr2 = new string('-', 100);
			var sb = new StringBuilder();
			sb.AppendLine(hr1);
			sb.AppendLine(hr2);
			sb.AppendLine("Time Tortoise Daily Summary");
			sb.AppendLine();
			sb.AppendLine(string.Format($"{date:dddd, MMMM dd, yyyy}"));
			sb.AppendLine(string.Format($"Day {date.DayOfYear} ---- {daysInYear - date.DayOfYear} days remaining"));

			var timeSummary = new Dictionary<int, TimeSpan>();
			var activityList = _repository.LoadActivities();

			var timeSegments = _repository.LoadAllTimeSegments(new SystemDateTime(date), new SystemDateTime(endDate));
			foreach (var timeSegment in timeSegments)
			{
				var activityId = timeSegment.ActivityId;
				if (timeSummary.ContainsKey(activityId))
				{
					timeSummary[activityId] = timeSummary[activityId].Add(timeSegment.EndTime - timeSegment.StartTime);
				}
				else
				{
					timeSummary.Add(activityId, timeSegment.EndTime - timeSegment.StartTime);
				}
			}

			sb.AppendLine();
			foreach (var activity in activityList)
			{
				if (timeSummary.ContainsKey(activity.Id))
				{
					sb.AppendLine(string.Format(
						$"{activity.Name}\t{timeSummary[activity.Id]:hh\\:mm\\:ss}"));
				}
			}
			sb.AppendLine();

			LastUpdated = DateTime.Now;
			sb.AppendLine(string.Format($"Last updated: {LastUpdated:yyyy-MM-dd hh:mm:ss}"));
			sb.AppendLine();

			sb.AppendLine(hr2);
			sb.AppendLine(hr1);

			File.WriteAllText(fileInfo.FullName, sb.ToString());
		}
	}
}
