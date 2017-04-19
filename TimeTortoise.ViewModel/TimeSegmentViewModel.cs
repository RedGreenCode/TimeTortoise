using System;
using System.Globalization;
using TimeTortoise.Model;

namespace TimeTortoise.ViewModel
{
	public class TimeSegmentViewModel : NotificationBase<TimeSegment>
	{
		private readonly ValidationMessageViewModel _validationMessageViewModel;

		public TimeSegmentViewModel(TimeSegment timeSegment, ValidationMessageViewModel validationMessageViewModel) : base(timeSegment)
		{
			_validationMessageViewModel = validationMessageViewModel;
			TimeSegment = timeSegment;
		}

		public TimeSegment TimeSegment { get; }

		public string StartTime
		{
			get => This.StartTime.ToString(CultureInfo.CurrentUICulture);
			set
			{
				_validationMessageViewModel.SetFieldValid();
				try
				{
					SetProperty(() => This.StartTime = DateTime.Parse(value));
					RaisePropertyChanged("Duration");
				}
				catch (FormatException)
				{
					_validationMessageViewModel.SetFieldInvalid();
				}
				finally
				{
					_validationMessageViewModel.SetValidationMessages();
				}
			}
		}

		public string EndTime
		{
			get => This.EndTime.ToString(CultureInfo.CurrentUICulture);
			set
			{
				_validationMessageViewModel.SetFieldValid();
				try
				{
					SetProperty(() => This.EndTime = DateTime.Parse(value));
					RaisePropertyChanged("Duration");
				}
				catch (FormatException)
				{
					_validationMessageViewModel.SetFieldInvalid();
				}
				finally
				{
					_validationMessageViewModel.SetValidationMessages();
				}
			}
		}

		public string Duration => 
			This.EndTime == DateTime.MinValue ?
			string.Empty :
			string.Format(@"{0:hh\:mm\:ss}", This.EndTime - This.StartTime);
	}
}
