using System;
using System.Collections.Generic;
using System.Text;

namespace TalentPlus.Shared
{
	public interface IReminderService
	{
		void Remind(DateTime dateTime, string title, string message, string activityId, int alarmId);
		void Remind(int delayTime, string title, string message, string activityId, int alarmId);
	}
}
