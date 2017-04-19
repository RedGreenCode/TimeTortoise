using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace TimeTortoise.ViewModel
{
	public class ValidationMessageViewModel : NotificationBase
	{
		private class ValidationMessage
		{
			public ValidationMessage(bool isValid, string message)
			{
				IsValid = isValid;
				Message = message;
			}

			public bool IsValid { get; set; }
			public string Message { get; }
		}

		private readonly Dictionary<string, ValidationMessage> _messages;

		public ValidationMessageViewModel()
		{
			_messages = new Dictionary<string, ValidationMessage>
			{
				{"StartTime", new ValidationMessage(true, "Please enter a valid start date and time.")},
				{"EndTime", new ValidationMessage(true, "Please enter a valid end date and time.")}
			};
		}

		public void SetFieldValid([CallerMemberName] string fieldName = null)
		{
			_messages[fieldName].IsValid = true;
		}

		public void SetFieldInvalid([CallerMemberName] string fieldName = null)
		{
			_messages[fieldName].IsValid = false;
		}

		private string _validationMessages = string.Empty;
		public string ValidationMessages
		{
			get => _validationMessages;
			set => SetProperty(ref _validationMessages, value);
		}

		public void SetValidationMessages()
		{
			var messages = new StringBuilder();
			foreach (var message in _messages)
			{
				if (!message.Value.IsValid) messages.AppendLine(message.Value.Message);
			}
			ValidationMessages = messages.ToString();
		}
	}
}
