using System;

namespace TimeTortoise.Model
{
	public class SystemDateTime : IDateTime
	{

		public SystemDateTime()
		{
		}

		public SystemDateTime(DateTime? dateTime)
		{
			Value = dateTime ?? DateTime.MinValue;
		}

		public DateTime Value { get; }

		public DateTime Now => DateTime.Now;
	}
}
