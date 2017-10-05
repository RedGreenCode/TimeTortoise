using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTortoise.Model
{
	public interface ISettingsUtility
	{
		void ReadSettings();
		ISettings Settings { get; }
		string SettingsPath { get; }
	}
}
