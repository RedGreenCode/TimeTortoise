using System;
using System.Collections.Generic;

namespace TimeTortoise.Client
{
	public interface ISignalRClient
	{
		void ConnectToServer();

		DateTime GetNewestMessage();
	}
}
