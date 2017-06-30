using System.Windows;

namespace TimeTortoise.WPF.Commands
{
	public class ExitCommand : CommandBase<ExitCommand>
	{
		public override void Execute(object parameter)
		{
			Application.Current.Shutdown();
		}

		public override bool CanExecute(object parameter)
		{
			return true;
		}
	}
}
