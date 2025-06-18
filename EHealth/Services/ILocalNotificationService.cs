using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EHealth.Services
{
    public interface ILocalNotificationService
    {
        void ScheduleNotification(DateTime notifyTime, string title, string message);
    }
}

