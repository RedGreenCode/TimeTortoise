using System;
using System.Windows.Forms;
using Gma.System.MouseKeyHook;

namespace TimeTortoise.Server
{
	/// <summary>
	/// Forked from https://github.com/gmamaladze/globalmousekeyhook/tree/vNext/Demo
	/// </summary>
	public class IdleTime
	{
		private IKeyboardMouseEvents _keyboardMouseEvents;
		private DateTime _lastUserActivity = DateTime.Now;

		public IdleTime()
		{
			Subscribe();
			UpdateLastUserActivityTime();
		}

		private void UpdateLastUserActivityTime()
		{
			LastUserActivityTime = DateTime.Now;
		}

		public DateTime LastUserActivityTime { get; set; }

		private void Subscribe()
		{
			Unsubscribe();
			_keyboardMouseEvents = Hook.GlobalEvents();
			_keyboardMouseEvents.KeyDown += OnKeyDown;
			_keyboardMouseEvents.KeyUp += OnKeyUp;
			_keyboardMouseEvents.KeyPress += HookManager_KeyPress;

			_keyboardMouseEvents.MouseUp += OnMouseUp;
			_keyboardMouseEvents.MouseClick += OnMouseClick;
			_keyboardMouseEvents.MouseDoubleClick += OnMouseDoubleClick;

			_keyboardMouseEvents.MouseMove += HookManager_MouseMove;

			_keyboardMouseEvents.MouseDragStarted += OnMouseDragStarted;
			_keyboardMouseEvents.MouseDragFinished += OnMouseDragFinished;

			_keyboardMouseEvents.MouseWheel += HookManager_MouseWheel;

			_keyboardMouseEvents.MouseDown += OnMouseDown;
		}

		private void Unsubscribe()
		{
			if (_keyboardMouseEvents == null) return;
			_keyboardMouseEvents.KeyDown -= OnKeyDown;
			_keyboardMouseEvents.KeyUp -= OnKeyUp;
			_keyboardMouseEvents.KeyPress -= HookManager_KeyPress;

			_keyboardMouseEvents.MouseUp -= OnMouseUp;
			_keyboardMouseEvents.MouseClick -= OnMouseClick;
			_keyboardMouseEvents.MouseDoubleClick -= OnMouseDoubleClick;

			_keyboardMouseEvents.MouseMove -= HookManager_MouseMove;

			_keyboardMouseEvents.MouseDragStarted -= OnMouseDragStarted;
			_keyboardMouseEvents.MouseDragFinished -= OnMouseDragFinished;

			_keyboardMouseEvents.MouseWheel -= HookManager_MouseWheel;

			_keyboardMouseEvents.MouseDown -= OnMouseDown;

			_keyboardMouseEvents.Dispose();
			_keyboardMouseEvents = null;
		}

		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnKeyUp(object sender, KeyEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void HookManager_MouseMove(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnMouseDown(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnMouseUp(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnMouseClick(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnMouseDoubleClick(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnMouseDragStarted(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void OnMouseDragFinished(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}

		private void HookManager_MouseWheel(object sender, MouseEventArgs e)
		{
			UpdateLastUserActivityTime();
		}
	}
}
