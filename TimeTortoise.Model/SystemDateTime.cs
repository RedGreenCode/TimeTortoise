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

		public static SystemDateTime MinValue => new SystemDateTime(DateTime.MinValue);

		public static SystemDateTime MaxValue => new SystemDateTime(DateTime.MaxValue);

		public DateTime Value { get; }

		public DateTime Now => DateTime.Now;
	}
}
