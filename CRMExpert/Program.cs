using System;

namespace CRMExpert 
{
    internal class Program
    {
        static List<Meeting> _meetings = new List<Meeting>();
        static void Main(string[] args)
        {
             ShowMenu();
        }
        /// <summary>
        /// Показать меню
        /// </summary>
        private static void ShowMenu()
        {
            Console.WriteLine("1 -- Создать новую встречу");
            Console.WriteLine("2 -- Изменить существующее встречу");
            Console.WriteLine("3 -- Удалить встречу");
            Console.WriteLine("4 -- Посмотреть встречи на определенный день");
            switch(Console.ReadKey().Key)
            {
                case ConsoleKey.D1:
                    Console.WriteLine();
                    CreateMeeting();
                    break;
                case ConsoleKey.D2:
                    Console.WriteLine();
                    EditMeeting();
                    break;
                case ConsoleKey.D3:
                    Console.WriteLine();
                    DeleteMeeting();
                    break;
                case ConsoleKey.D4:
                    Console.WriteLine();
                    ShowMeetings();
                    break;
                default:
                    Console.WriteLine();
                    ShowMenu();
                    break;
            }
        }
        /// <summary>
        /// Удалить встречу
        /// </summary>
        private static void DeleteMeeting()
        {
            int id;
            foreach (var meet in _meetings)
            {
                meet.ShowAsync(true);
                Console.WriteLine("------------------------------------------");
            }
            Meeting meeting = new Meeting();
            bool isValid = false;
            while (!isValid)
            {
                meeting = null;
                InsertNumber(null, "Ввведите порядковый номер встречи", 2, out id);

                isValid = true;
                meeting = _meetings.FirstOrDefault(i => i.ID == id);
                if (meeting == null)
                {
                    Console.WriteLine("Нет встречи с таким номером.");
                    isValid = false;
                }
            }

            _meetings.Remove(meeting);
            meeting.Token.Cancel();
            ShowMenu();
        }
        /// <summary>
        /// Редактировать встречу
        /// </summary>
        private static void EditMeeting()
        {
            int id;
            bool isNotIntersected = false;

            foreach (var meet in _meetings)
            {
                meet.ShowAsync(true);
                Console.WriteLine("------------------------------------------");
            }
            Meeting meeting = new Meeting();
            bool isValid = false;
            while (!isValid)
            {
                meeting = null;
                InsertNumber(null, "Ввведите порядковый номер встречи", 2, out id);

                isValid = true;
                meeting = _meetings.FirstOrDefault(i => i.ID == id);
                if (meeting == null)
                {
                    Console.WriteLine("Нет встречи с таким номером.");
                    isValid = false;
                }
            }

            Console.WriteLine("Введите навзание встречи:");
            meeting.Name = Console.ReadLine();

            while (!isNotIntersected)
            {
                InsertDate("Введите время начала встречи(в формате DD/MM/YYYY hh:mm:ss):", 1, out _, meeting);

                InsertDate("Введите время окончания встречи(в формате DD/MM/YYYY hh:mm:ss):", 2, out _, meeting);
                foreach (Meeting m in _meetings)
                {
                    if ((meeting.BeginTime >= m.BeginTime && meeting.BeginTime <= m.EndTime)
                        || (meeting.EndTime >= m.BeginTime && meeting.EndTime <= m.BeginTime)
                        || (meeting.BeginTime <= m.BeginTime && meeting.EndTime >= m.EndTime))
                    {
                        Console.WriteLine("Встречи перескаются.");
                        isNotIntersected = false;
                        break;
                    }
                }
            }

            InsertNumber(meeting, "Введите время, за которое нужно уведомить о встрече(в минутах):", 1, out _);

            meeting.Token.Cancel();

            Parallel.Invoke(() => ShowMenu(),
                            () => meeting.NotifyAsync(meeting.Token));
        }
        /// <summary>
        /// Показать встречи за выбранный день
        /// </summary>
        private static void ShowMeetings()
        {
            DateTime? dt = new DateTime();
            InsertDate("Введите нужный день:", 3, out dt);

            foreach (var meet in _meetings)
            {
                if (meet.BeginTime.Date == dt.Value.Date)
                {
                    meet.ShowAsync();
                    Console.WriteLine("------------------------------------------");
                }
            }

            ShowMenu();
        }
        /// <summary>
        /// Создать новую встречу
        /// </summary>
        private static void CreateMeeting()
        {
            bool isNotIntersected = false;
            Meeting meeting = new Meeting();
            meeting.ID = Meeting.CurrentID;

            Console.WriteLine("Введите навзание встречи:");
            meeting.Name = Console.ReadLine();
            while (!isNotIntersected)
            {
                isNotIntersected = true;
                InsertDate("Введите время начала встречи(в формате DD/MM/YYYY hh:mm:ss):", 1, out _, meeting);

                InsertDate("Введите время окончания встречи(в формате DD/MM/YYYY hh:mm:ss):", 2, out _, meeting);
                foreach (Meeting m in _meetings)
                {
                    if ((meeting.BeginTime >= m.BeginTime && meeting.BeginTime <= m.EndTime)
                        || (meeting.EndTime >= m.BeginTime && meeting.EndTime <= m.BeginTime)
                        || (meeting.BeginTime <= m.BeginTime && meeting.EndTime >= m.EndTime))
                    {
                        Console.WriteLine("Встречи перескаются.");
                        isNotIntersected = false;
                        break;
                    }
                }
            }

            InsertNumber(meeting, "Введите время, за которое нужно уведомить о встрече(в минутах):", 1, out _);

            meeting.Token = new CancellationTokenSource();
            Meeting.CurrentID++;
           
            _meetings.Add(meeting);

            Parallel.Invoke(() => ShowMenu(),
                            () => meeting.NotifyAsync(meeting.Token));
        }
        /// <summary>
        /// Ввести число
        /// </summary>
        /// <param name="meeting">Встреча</param>
        /// <param name="mes">Первое сообщение</param>
        /// <param name="type">Тип ввода</param>
        /// <param name="number">Число</param>
        private static void InsertNumber(Meeting meeting, string mes, int type, out int number)
        {
            number = 0;
            int minutes;
            bool isValid = false;
            while (!isValid)
            {
                Console.WriteLine(mes);
                isValid = int.TryParse(Console.ReadLine(), out minutes);
                try
                {
                    switch (type)
                    {
                        case 1:
                            meeting.NotificationTime = minutes;
                            break;
                        case 2:
                            number = minutes;
                            break;
                    }
                }
                catch
                {
                    isValid = false;
                }
                if (!isValid)
                    switch (type)
                    {
                        case 1:
                            Console.WriteLine("Время некорректно.");
                            break;
                        case 2:
                            Console.WriteLine("Идентификатор некорректен.");
                            break;
                    }
            }
        }
        /// <summary>
        /// Ввести дату и время
        /// </summary>
        /// <param name="mes">Первое сообщение</param>
        /// <param name="type">Тип ввода</param>
        /// <param name="dt">Дата и время</param>
        /// <param name="meeting">Встреча</param>
        private static void InsertDate(string mes, int type, out DateTime? dt, Meeting meeting = null)
        {
            dt = new DateTime();
            DateTime begin;
            bool isValid = false;
            while (!isValid)
            {
                Console.WriteLine(mes);
                isValid = DateTime.TryParse(Console.ReadLine(), out begin);
                try
                {
                    switch (type)
                    {
                        case 1:
                            meeting.BeginTime = begin;
                            break;
                        case 2:
                            meeting.EndTime = begin;
                            break;
                        case 3:
                            dt = begin;
                            break;
                    }
                }
                catch
                {
                    isValid = false;
                }
                if (!isValid)
                    switch (type)
                    {
                        case 1:
                            Console.WriteLine("Время начала встречи некорректно.");
                            break;
                        case 2:
                            Console.WriteLine("Время окончания встречи некорректно.");
                            break;
                        case 3:
                            Console.WriteLine("День некорректен.");
                            break;
                    }
            }
        }
    }
}