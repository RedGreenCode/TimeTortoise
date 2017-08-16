using System;
using TimeTortoise.Client;
using Xunit;

namespace TimeTortoise.IntegrationTests
{
	public class ClientTests
	{
		[Fact]
		public void Messages_WhenMessageIsReceived_ContainsReceivedMessage()
		{
			// Arrange
			var lastUserActivityTime = new DateTime(2017, 3, 1, 10, 0, 0);
			var client = new SignalRClient();

			// Act
			client.ReceiveMessage(lastUserActivityTime);
			var latestMessage = client.GetNewestMessage();

			// Assert
			Assert.Equal(lastUserActivityTime, latestMessage);
		}
	}
}
