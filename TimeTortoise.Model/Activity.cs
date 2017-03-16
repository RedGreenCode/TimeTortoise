using System.Collections.Generic;

namespace TimeTortoise.Model
{
	public class Activity
	{
		public Activity()
		{
			TimeSegments = new List<TimeSegment>();
		}

		public int Id { get; set; }
		public string Name { get; set; }
		public List<TimeSegment> TimeSegments { get; }
	}
}
