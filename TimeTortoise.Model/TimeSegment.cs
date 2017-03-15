using System;

namespace TimeTortoise.Model
{
	public class TimeSegment
	{
		public int Id { get; set; }
		public int ActivityId { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
	}
}
