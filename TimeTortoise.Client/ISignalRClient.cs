using System;
using System.Collections.Generic;

namespace TimeTortoise.Client
{
	public interface ISignalRClient
	{
		Queue<DateTime> Messages { get; }

		void ConnectToServer();
	}
}