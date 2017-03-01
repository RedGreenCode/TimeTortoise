using System.Collections.Generic;
using System.Linq;
using TimeTortoise.Model;

namespace TimeTortoise.DAL
{
	public class Repository : IRepository
	{
		private readonly Context _context;

		public Repository(Context context)
		{
			_context = context;
		}

		public List<Activity> LoadActivities()
		{
			return _context.Activities.ToList();
		}

		public void SaveActivity(Activity activity)
		{
			if (activity.Id == 0) _context.Add(activity);

			_context.SaveChanges();
		}

		public void DeleteActivity(Activity activity)
		{
			_context.Remove(activity);
			_context.SaveChanges();
		}
	}
}
