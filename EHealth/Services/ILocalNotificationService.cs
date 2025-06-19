namespace EHealth.Services
{
    public interface ILocalNotificationService
    {
        void ScheduleNotification(DateTime notifyTime, string title, string message);

        void CancelNotification(DateTime notifyTime, string title);
    }
}
