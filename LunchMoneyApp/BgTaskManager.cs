using Microsoft.Phone.Scheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LunchMoneyApp
{
    class BgTaskManager
    {
        public string TaskName;

        public string TaskDescription;

        public bool isEnabled()
        {
            return null != (ScheduledActionService.Find(TaskName) as PeriodicTask);
        }

        public void disable()
        {
            if (isEnabled())
            {
                ScheduledActionService.Remove(TaskName);
            }
        }

        public void enable()
        {
            if (!isEnabled())
            {
                PeriodicTask task = new PeriodicTask(TaskName);
                task.Description = TaskDescription;
                ScheduledActionService.Add(task);
#if DEBUG
                ScheduledActionService.LaunchForTest(TaskName,
                        TimeSpan.FromMilliseconds(1500));
#endif
            }
        }

        public void toggleState()
        {
            if(isEnabled())
                disable();
            else
                enable();
        }

    }
}
