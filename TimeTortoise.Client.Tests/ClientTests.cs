using System;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Client.Hubs;

using Xunit;
using Moq;
using Microsoft.AspNet.SignalR.Client;

namespace TimeTortoise.Client.Tests
{
	public class ClientTests
	{
		[Fact]
		public void Messages_WhenMessageIsReceived_ContainsReceivedMessage()
		{
			// Arrange
			var lastUserActivityTime = new DateTime(2017, 3, 1, 10, 0, 0);
			var client = new SignalRClient("http://example.com");

			// Act
			client.ReceiveMessage(lastUserActivityTime);
			var latestMessage = client.GetNewestMessage();

			// Assert
			Assert.Equal(lastUserActivityTime, latestMessage);
		}

		[Fact]
		public void NewestMessage_WhenNoMessagesAreReceived_IsMinDate()
		{
			// Arrange
			var mockHubProxy = new Mock<IHubProxy>();
			mockHubProxy.Setup(s => s.Subscribe(It.IsAny<string>())).Returns(new Subscription());
			var mockHubConnection = new Mock<IHubConnection>();
			mockHubConnection.Setup(c => c.CreateHubProxy(It.IsAny<string>())).Returns(mockHubProxy.Object);
			var client = new SignalRClient(mockHubConnection.Object);

			// Act
			var latestMessage = client.GetNewestMessage();

			// Assert
			Assert.Equal(DateTime.MinValue, latestMessage);
		}
	}
}
