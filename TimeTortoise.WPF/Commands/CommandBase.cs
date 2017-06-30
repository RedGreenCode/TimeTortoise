using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using Hardcodet.Wpf.TaskbarNotification;

namespace TimeTortoise.WPF.Commands
{
	/// <summary>
	/// Basic implementation of the <see cref="ICommand"/>
	/// interface, which is also accessible as a markup
	/// extension.
	/// 
	/// Forked from https://bitbucket.org/hardcodet/notifyicon-wpf/src/a72c42d41e187761d27486f4c031f78f0018f2b1/Hardcodet.NotifyIcon.Wpf/Source/Sample%20Project/Commands/CommandBase.cs?at=master&amp;fileviewer=file-view-default
	/// </summary>
	public abstract class CommandBase<T> : MarkupExtension, ICommand
		where T : class, ICommand, new()
	{
		/// <summary>
		/// A singleton instance.
		/// </summary>
		private static T _command;

		/// <summary>
		/// Gets a shared command instance.
		/// </summary>
		public override object ProvideValue(IServiceProvider serviceProvider)
		{
			return _command ?? (_command = new T());
		}

		/// <summary>
		/// Fires when changes occur that affect whether
		/// or not the command should execute.
		/// </summary>
		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		/// <summary>
		/// Defines the method to be called when the command is invoked.
		/// </summary>
		/// <param name="parameter">Data used by the command.
		/// If the command does not require data to be passed,
		/// this object can be set to null.
		/// </param>
		public abstract void Execute(object parameter);

		/// <summary>
		/// Defines the method that determines whether the command
		/// can execute in its current state.
		/// </summary>
		/// <returns>
		/// This default implementation always returns true.
		/// </returns>
		/// <param name="parameter">Data used by the command.  
		/// If the command does not require data to be passed,
		/// this object can be set to null.
		/// </param>
		public virtual bool CanExecute(object parameter)
		{
			return !IsDesignMode;
		}


		private static bool IsDesignMode => (bool)
			DependencyPropertyDescriptor.FromProperty(DesignerProperties.IsInDesignModeProperty,
					typeof (FrameworkElement))
				.Metadata.DefaultValue;


		/// <summary>
		/// Resolves the window that owns the TaskbarIcon class.
		/// </summary>
		/// <param name="commandParameter"></param>
		/// <returns></returns>
		protected Window GetTaskbarWindow(object commandParameter)
		{
			if (IsDesignMode) return null;

			//get the showcase window off the taskbaricon
			var tb = commandParameter as TaskbarIcon;
			return tb == null ? null : TryFindParent<Window>(tb);
		}

		#region TryFindParent helper

		/// <summary>
		/// Finds a parent of a given item on the visual tree.
		/// </summary>
		/// <typeparam name="T">The type of the queried item.</typeparam>
		/// <param name="child">A direct or indirect child of the
		/// queried item.</param>
		/// <returns>The first parent item that matches the submitted
		/// type parameter. If not matching item can be found, a null
		/// reference is being returned.</returns>
		private static T TryFindParent<T>(DependencyObject child)
			where T : DependencyObject
		{
			//get parent item
			var parentObject = GetParentObject(child);

			//we've reached the end of the tree
			if (parentObject == null) return null;

			//check if the parent matches the type we're looking for
			var parent = parentObject as T;
			return parent ?? TryFindParent<T>(parentObject);
		}

		/// <summary>
		/// This method is an alternative to WPF's
		/// <see cref="VisualTreeHelper.GetParent"/> method, which also
		/// supports content elements. Keep in mind that for content element,
		/// this method falls back to the logical tree of the element!
		/// </summary>
		/// <param name="child">The item to be processed.</param>
		/// <returns>The submitted item's parent, if available. Otherwise
		/// null.</returns>
		private static DependencyObject GetParentObject(DependencyObject child)
		{
			if (child == null) return null;
			var contentElement = child as ContentElement;

			if (contentElement == null) return VisualTreeHelper.GetParent(child);
			var parent = ContentOperations.GetParent(contentElement);
			if (parent != null) return parent;

			var fce = contentElement as FrameworkContentElement;
			return fce?.Parent;

			//if it's not a ContentElement, rely on VisualTreeHelper
		}

		#endregion
	}
}
