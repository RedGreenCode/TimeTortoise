using System;

namespace TimeTortoise.Client
{
	public interface ISignalRClient
	{
		void ConnectToServer();

		DateTime GetNewestMessage();
	}
}
