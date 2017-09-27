using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTortoise.Model;
using Xunit;

namespace TimeTortoise.IntegrationTests
{
	public class SettingsIntegrationTests
	{
		[Fact]
		public void SettingsUtility_WhenUsingPhysicalFile_ReturnsCorrectSettings()
		{
			// Arrange/Act
			var settingsUtility = new SettingsUtility(".");

			// Assert
			Assert.Equal(42, settingsUtility.Settings.IdleTimeoutSeconds);
		}
	}
}
