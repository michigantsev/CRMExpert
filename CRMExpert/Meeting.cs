using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CRMExpert
{
    internal class Meeting
    {
        /// <summary>
        /// Следующий доступный идентификатор
        /// </summary>
        public static int CurrentID = 0;
        private DateTime _beginTime;
        private DateTime _endTime;
        private int _notificationTime;
        /// <summary>
        /// Идентифиактор
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// Время начала встречи
        /// </summary>
        public DateTime BeginTime 
        {
            get
            {
                return _beginTime;
            }
            set
            {
                if (value <= DateTime.Now)
                    throw new Exception("Встречи всегда планируются только на будущее время.");
                else
                    _beginTime = value;
            }
        }
        /// <summary>
        /// Время конца встречи
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {               
                if (value <= BeginTime)
                    throw new Exception("Время окончания не может быть раньше времени начала.");
                else
                    _endTime = value;
            }
        }
        /// <summary>
        /// Название встречи
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Токен для отмены уведомления
        /// </summary>
        public CancellationTokenSource Token { get; set; }
        /// <summary>
        /// Количество минут, за которые нужно уведомить о встрече
        /// </summary>
        public int NotificationTime
        {
            get
            {
                return _notificationTime;
            }
            set
            {
                if (value <= (DateTime.Now - BeginTime).Minutes)
                    throw new Exception("Нельзя создать уведомление в прошлом.");
                else
                    _notificationTime = value;
            }
        }
        /// <summary>
        /// Создать уведомление
        /// </summary>
        /// <param name="cancellationTokenSource">Токен для отмены уведомления</param>
        /// <returns></returns>
        public async Task NotifyAsync(CancellationTokenSource cancellationTokenSource)
        {
            await Task.Delay((int)BeginTime.Subtract(DateTime.Now).TotalMilliseconds-NotificationTime*60000);
            await ShowAsync();
        }
        /// <summary>
        /// Вывести данные о встрече
        /// </summary>
        /// <param name="isIdNeeded">Нужно ли выводить ID</param>
        /// <returns></returns>
        public async Task ShowAsync(bool isIdNeeded = false)
        {
            if (isIdNeeded)
                Console.WriteLine($"Идентификатор: {ID}");
            Console.WriteLine($"Название встречи: {Name}");
            Console.WriteLine($"Время начала: {BeginTime}");
            Console.WriteLine($"Примерное время окончания: {EndTime}");
        }
    }
}
