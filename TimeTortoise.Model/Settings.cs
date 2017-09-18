namespace TimeTortoise.Model
{
	public class Settings : ISettings
	{
		public int IdleTimeoutSeconds { get; set; }
		public string ServerUrl { get; set; }
	}
}
