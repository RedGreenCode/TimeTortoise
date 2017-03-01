using System.Collections.Generic;

using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public interface IRepository
	{
		List<Activity> LoadActivities();
		void SaveActivity(Activity activity);
		void DeleteActivity(Activity activity);
	}
}
