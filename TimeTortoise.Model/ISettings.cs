namespace TimeTortoise.Model
{
	public interface ISettings
	{
		int IdleTimeoutSeconds { get; set; }
		string ServerUrl { get; set; }
		int DailySummaryUpdateIntervalSeconds { get; set; }
	}
}
