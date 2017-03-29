using System.Collections.Generic;

using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public interface IRepository
	{
		List<Activity> LoadActivities();
		void AddActivity(Activity activity);
		void SaveActivity();
		void DeleteActivity(Activity activity);
	}
}
