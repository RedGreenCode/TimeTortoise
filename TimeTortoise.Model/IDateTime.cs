using System;

namespace TimeTortoise.Model
{
	// https://docs.microsoft.com/en-us/aspnet/core/mvc/controllers/dependency-injection
	public interface IDateTime
	{
		DateTime Now { get; }
	}
}
