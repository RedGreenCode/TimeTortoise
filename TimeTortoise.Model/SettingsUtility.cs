using System.IO;
using YamlDotNet.Serialization;

namespace TimeTortoise.Model
{
	public class SettingsUtility : ISettingsUtility
	{
		private readonly string _settingsFileName;

		public SettingsUtility(string settingsPath)
		{
			_settingsFileName = string.Format($@"{settingsPath}\settings.txt");
		}

		public void ReadSettings()
		{
			try
			{
				var input = File.ReadAllText(_settingsFileName);
				var deserializer = new Deserializer();
				Settings = deserializer.Deserialize<Settings>(input);
			}
			catch (FileNotFoundException)
			{
				SetDefaultSettings();
			}
		}

		private void SetDefaultSettings()
		{
			Settings = new Settings();
			Settings.IdleTimeoutSeconds = 300;
			Settings.ServerUrl = "http://127.0.0.1:8080";
		}

		public ISettings Settings { get; private set; }
	}
}
