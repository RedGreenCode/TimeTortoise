using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class ActivityViewModel : NotificationBase<Activity>
	{
		public ActivityViewModel(Activity activity) : base(activity)
		{
		}

		public string Name
		{
			get { return This.Name; }
			set { SetProperty(This.Name, value, () => This.Name = value); }
		}
	}
}
