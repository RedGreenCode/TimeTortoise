using System;

namespace TimeTortoise.Model
{
	public class SystemDateTime : IDateTime
	{
		public DateTime Now => DateTime.Now;
	}
}
