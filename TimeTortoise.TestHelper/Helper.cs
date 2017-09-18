using System;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

using Moq;

using TimeTortoise.Client;
using TimeTortoise.DAL;
using TimeTortoise.ViewModel;
using TimeTortoise.Model;

namespace TimeTortoise.TestHelper
{
	public static class Helper
	{
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

		public static IRepository GetMockRepositoryObject()
		{
			return GetMockRepository().Object;
		}

		public static Mock<IRepository> GetMockRepository()
		{
			var mockRepository = new Mock<IRepository>();
			mockRepository.Setup(x => x.LoadActivities()).Returns(GetActivities());
			mockRepository.Setup(x => x.LoadTimeSegments(1, It.IsAny<IDateTime>(), It.IsAny<IDateTime>())).Returns(GetTimeSegments());
			return mockRepository;
		}

		private static List<TimeSegment> GetTimeSegments()
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

		public static ISignalRClient GetMockSignalRClientObject()
		{
			var mockSignalRClient = new Mock<ISignalRClient>();
			return mockSignalRClient.Object;
		}

		public static ISettingsUtility GetMockSettingsUtility()
		{
			var mockSettings = new Mock<ISettings>();
			mockSettings.Setup(s => s.IdleTimeoutSeconds).Returns(10);

			var mockSettingsUtility = new Mock<ISettingsUtility>();
			mockSettingsUtility.Setup(s => s.Settings).Returns(mockSettings.Object);

			return mockSettingsUtility.Object;
		}

		public static MainViewModel GetMainViewModel(Context context)
		{
			return new MainViewModel(new Repository(context), new SystemDateTime(), new ValidationMessageViewModel(), GetMockSignalRClientObject(), GetMockSettingsUtility());
		}

		public static MainViewModel GetMainViewModel(Context context, IDateTime dateTimeProvider)
		{
			return new MainViewModel(new Repository(context), dateTimeProvider, new ValidationMessageViewModel(), GetMockSignalRClientObject(), GetMockSettingsUtility());
		}

		public static MainViewModel GetMainViewModel(IRepository repository)
		{
			return new MainViewModel(repository, new SystemDateTime(), new ValidationMessageViewModel(), GetMockSignalRClientObject(), GetMockSettingsUtility());
		}

		public static MainViewModel GetMainViewModel(int selectedActivityIndex)
		{
			var mvm = new MainViewModel(GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel(),
				GetMockSignalRClientObject(), GetMockSettingsUtility())
			{
				SelectedActivityIndex = selectedActivityIndex
			};
			return mvm;
		}

		public static MainViewModel GetMainViewModel(string selectedTimeSegmentStartTime)
		{
			var mvm = new MainViewModel(GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel(),
				GetMockSignalRClientObject(), GetMockSettingsUtility())
			{
				SelectedTimeSegmentStartTime = selectedTimeSegmentStartTime
			};
			return mvm;
		}

		public static MainViewModel GetMainViewModel(out ValidationMessageViewModel vmvm)
		{
			vmvm = new ValidationMessageViewModel();
			return new MainViewModel(GetMockRepositoryObject(), new SystemDateTime(), vmvm, GetMockSignalRClientObject(), GetMockSettingsUtility());
		}

		public static MainViewModel GetMainViewModel(int selectedActivityIndex, IDateTime dateTimeProvider, ISignalRClient mockSignalRClientObject)
		{
			var mvm = new MainViewModel(GetMockRepositoryObject(), dateTimeProvider, new ValidationMessageViewModel(), mockSignalRClientObject, GetMockSettingsUtility())
			{
				SelectedActivityIndex = selectedActivityIndex
			};
			return mvm;
		}

		public static MainViewModel GetMainViewModel()
		{
			return new MainViewModel(GetMockRepositoryObject(), new SystemDateTime(), new ValidationMessageViewModel(), GetMockSignalRClientObject(), GetMockSettingsUtility());
		}
	}
}
