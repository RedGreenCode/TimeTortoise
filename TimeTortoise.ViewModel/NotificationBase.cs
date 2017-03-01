using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TimeTortoise.ViewModel
{
	/// <summary>
	/// Forked from https://github.com/johnshew/Minimal-UWP-MVVM-CRUD/blob/master/Simple%20MVVM%20UWP%20with%20CRUD/ViewModels/ViewModelHelpers.cs
	/// </summary>
	public class NotificationBase : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		// SetField (Name, value); // where there is a data member
		protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string property = null)
		{
			if (EqualityComparer<T>.Default.Equals(field, value)) return false;
			field = value;
			RaisePropertyChanged(property);
			return true;
		}

		// SetField(()=> somewhere.Name = value; somewhere.Name, value) // Advanced case where you rely on another property
		protected void SetProperty<T>(T currentValue, T newValue, Action doSet, [CallerMemberName] string property = null)
		{
			doSet.Invoke();
			RaisePropertyChanged(property);
		}

		private void RaisePropertyChanged(string property)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
		}
	}

	public class NotificationBase<T> : NotificationBase where T : class, new()
	{
		protected readonly T This;

		public static implicit operator T(NotificationBase<T> thing) { return thing.This; }

		protected NotificationBase(T thing = null)
		{
			This = thing;
		}
	}
}
