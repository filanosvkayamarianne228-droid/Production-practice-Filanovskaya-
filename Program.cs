using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Newtonsoft.Json;

namespace Проект_1
{
    public class Request
    {
        // Уникальный идентификатор заявки (автоматически генерируется)
        public int Id { get; set; }
        // Детальное описание проблемы или запроса
        public string Description { get; set; }
        // Текущее состояние заявки (Новая, В работе, Выполнена, Закрыта)
        public string Status { get; set; }
        // Дата и время создания заявки (устанавливается автоматически)
        public DateTime CreatedAt { get; set; }
        // ФИО сотрудника, который подал заявку
        public string Applicant { get; set; }
        // ФИО сотрудника IT-отдела, назначенного для решения проблемы
        public string Executor { get; set; }
        // Дублирующее поле для ФИО заявителя (для совместимости)
        public object ClientName { get; internal set; }
        // Подразделение или отдел, из которого поступила заявка
        public string Department { get; internal set; }

        // Категория проблемы (Оборудование, ПО, Сеть и т.д.)
        public string Category { get; internal set; }
        // Дата и время завершения работы по заявке
        public string CompletedAt { get; internal set; }
    }

    public class RequestRepository
    {
        // Коллекция для хранения всех заявок в памяти
        private List<Request> _requests = new List<Request>();
        // Счетчик для генерации уникальных идентификаторов
        private int _currentId = 1;


        // Метод для добавления новой заявки
        public void Add(Request request)
        {
            // Присвоение уникального ID
            request.Id = _currentId++;
            // Установка текущей даты и времени создания запроса
            request.CreatedAt = DateTime.Now;
            // Добавление в коллекцию список 
            _requests.Add(request);
        } 

        // Получение всех заявок 
        public List<Request> GetAll()
        {
            return _requests;
        }

        // Поиск заявки по уникальному идентификатору
        public Request GetById(int id)
        {
            return _requests.FirstOrDefault(r => r.Id == id);
        }

        // Фильтрация заявок по статусу
        public List<Request> FindByStatus(string status)
        {
            return _requests
                .Where(r => r.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        // Обновление данных существующей заявки
        public void Update(Request request)

        {     // Поиск заявки по ID
            var existing = GetById(request.Id);
            if (existing != null)
            {
                // Обновление полей заявки
                existing.Description = request.Description;
                existing.Status = request.Status;
                existing.Executor = request.Executor;
            }
        }
        // Фильтрация заявок по категории (требует доработки)
        internal List<Request> FindByCategory(string category)
        {
            throw new NotImplementedException();
        }
    }
    internal class Program : ProgramBase

     // Cоздание  RequestRepository
    {
        static RequestRepository repository = new RequestRepository();
        static void Main(string[] args)
        {
            // Приветственное сообщение
            Console.WriteLine("Система учёта заявок ИТ-отдела");
            Console.WriteLine("-----------------------------\n");

            // Основной цикл программы
            bool isRunning = true;
            while (isRunning)
            {
                // Отображение меню
                ShowMenu();

                // Получение выбора пользователя
                string userInput = Console.ReadLine();

                // Обработка выбора
                switch (userInput)
                {
                    case "1":
                        CreateRequest();
                        break;
                    case "2":
                        ShowAllRequests();
                        break;
                    case "3":
                        SearchRequests();
                        break;
                    case "4":
                        UpdateRequest();
                        break;
                    case "5":
                        ShowStatistics();
                        break;
                    case "6":
                        isRunning = false;
                        break;
                    default:
                        Console.WriteLine("Неверный выбор. Попробуйте снова.");
                        break;
                }

                // Ожидание нажатия клавиши
                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
                Console.Clear();
            }
        }
        // Обновление заявок при вводе 
        private static void UpdateRequest()
        {
            Console.Clear();
            Console.WriteLine("Обновление заявки\n");

            Console.Write("Введите номер заявки: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Request request = repository.GetById(id);

                if (request == null)
                {
                    Console.WriteLine("Заявка не найдена.");
                    return;
                }

                DisplayRequestDetails(request);

                Console.WriteLine("\nВыберите поле для изменения:");
                Console.WriteLine("1. Статус");
                Console.WriteLine("2. Исполнитель");
                Console.WriteLine("3. Описание");
                Console.WriteLine("4. Категория");
                Console.WriteLine("5. Отдел заявителя");
                Console.Write("Ваш выбор: ");

                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        Console.Write("Введите новый статус: ");
                        string newStatus = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(newStatus))
                        {
                            Console.WriteLine("Статус не может быть пустым!");
                            return;
                        }

                        request.Status = newStatus;
                        break;

                    case "2":
                        Console.Write("Введите нового исполнителя: ");
                        string newExecutor = Console.ReadLine();

                        if (string.IsNullOrWhiteSpace(newExecutor))
                        {
                            Console.WriteLine("Исполнитель не может быть пустым!");
                            return;
                        }

                        request.Executor = newExecutor;
                        break;

                    case "3":
                        Console.Write("Введите новое описание: ");
                        request.Description = Console.ReadLine();
                        break;

                    case "4":
                        Console.Write("Введите новую категорию: ");
                        request.Category = Console.ReadLine();
                        break;

                    case "5":
                        Console.Write("Введите новый отдел: ");
                        request.Department = Console.ReadLine();
                        break;

                    default:
                        Console.WriteLine("Неверный выбор.");
                        return;
                }

                // Подтверждение изменений
             Console.WriteLine("\nПодтвердите обновление заявки (Y/N): ");
                if (Console.ReadLine().ToUpper() == "Y")
                {
                    repository.Update(request);
                    Console.WriteLine("Заявка успешно обновлена!");
                }
                else
                {
                    Console.WriteLine("Обновление отменено.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат номера.");
            }

            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

        private static void ShowStatistics()
        {
            Console.Clear();
            Console.WriteLine("Статистика заявок\n");

            var allRequests = repository.GetAll();

            // Подсчет общих показателей
            int totalRequests = allRequests.Count;
            int openRequests = repository.FindByStatus("Новая").Count;
            int inProgress = repository.FindByStatus("В работе").Count;
            int closed = repository.FindByStatus("Закрыта").Count;

            Console.WriteLine($"Всего заявок: {totalRequests}");
            Console.WriteLine($"Новых заявок: {openRequests}");
            Console.WriteLine($"В работе: {inProgress}");
            Console.WriteLine($"Закрытых: {closed}\n");

            // Статистика по категориям
            Console.WriteLine("Распределение по категориям:");
            var categories = allRequests
                .GroupBy(r => r.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() });

            foreach (var category in categories)
            {
                Console.WriteLine($"{category.Category}: {category.Count} заявок");
            }
        }


        // Отображение главного меню
        private static void ShowMenu()
        {
            Console.WriteLine("Выберите действие:");
            Console.WriteLine("1. Создать новую заявку");
            Console.WriteLine("2. Просмотреть все заявки");
            Console.WriteLine("3. Поиск заявок");
            Console.WriteLine("4. Обновить заявку");
            Console.WriteLine("5. Статистика");
            Console.WriteLine("6. Выйти из программы");
            Console.Write("\nВаш выбор: ");
        }

        // Создание новой заявки
        private static void CreateRequest()
        {
            Console.Clear();
            Console.WriteLine("Создание новой заявки\n");

            Request newRequest = new Request();

            Console.Write("ФИО заявителя: ");
            newRequest.ClientName = Console.ReadLine();

            Console.Write("Отдел: ");
            newRequest.Department = Console.ReadLine();

            Console.Write("Описание проблемы: ");
            newRequest.Description = Console.ReadLine();

            Console.Write("Категория: ");
            newRequest.Category = Console.ReadLine();

            repository.Add(newRequest);

            Console.WriteLine($"\nЗаявка №{newRequest.Id} успешно создана!");
        }

        // Отображение всех заявок
        private static void ShowAllRequests()
        {
            Console.Clear();
            Console.WriteLine("Список всех заявок\n");

            List<Request> requests = repository.GetAll();

            if (requests.Count == 0)
            {
                Console.WriteLine("Заявок нет.");
                return;
            }

            foreach (var request in requests)
            {
                Console.WriteLine($"№{request.Id}");
                Console.WriteLine($"Заявитель: {request.ClientName}");
                Console.WriteLine($"Отдел: {request.Department}");
                Console.WriteLine($"Описание: {request.Description}");
                Console.WriteLine($"Категория: {request.Category}");
                Console.WriteLine($"Статус: {request.Status}");
                Console.WriteLine($"Исполнитель: {request.Executor ?? "Не назначен"}");
                Console.WriteLine($"Дата создания: {request.CreatedAt}");
                Console.WriteLine($"Дата завершения: {request.CompletedAt ?? "Не завершено"}\n");
            }
        }

        // Поиск заявок
        private static void SearchRequests()
        {
            Console.Clear();
            Console.WriteLine("Поиск заявок\n");

            Console.WriteLine("Выберите критерий поиска:");
            Console.WriteLine("1. По номеру заявки");
            Console.WriteLine("2. По ФИО заявителя");
            Console.WriteLine("3. По статусу");
            Console.WriteLine("4. По категории");
            Console.Write("Ваш выбор: ");

            string searchType = Console.ReadLine();

            switch (searchType)
            {
                case "1":
                    SearchByNumber();
                    break;
                case "2":
                    SearchByClientName();
                    break;
                case "3":
                    SearchByStatus();
                    break;
                case "4":
                    SearchByCategory();
                    break;
                default:
                    Console.WriteLine("Неверный выбор.");
                    break;
            }
        }

        private static void SearchByCategory()
        {
            Console.Write("Введите категорию: ");
            string category = Console.ReadLine();
            List<Request> results = repository.FindByCategory(category);
            DisplaySearchResults(results);
        }

        private static void SearchByStatus()
        {
            Console.Write("Введите статус: ");
            string status = Console.ReadLine();
            List<Request> results = repository.FindByStatus(status);
            DisplaySearchResults(results);
        }

        private static void SearchByClientName()
        {
            Console.Write("Введите ФИО заявителя: ");
            string name = Console.ReadLine();

            List<Request> results = repository.GetAll()
                .Where(r => string.Equals((string)r.ClientName, name, StringComparison.OrdinalIgnoreCase))
                .ToList();

            DisplaySearchResults(results);
        }

        private static void DisplaySearchResults(List<Request> results)
        {
            if (results.Count == 0)
            {
                Console.WriteLine("Заявки не найдены.");
                return;
            }

            foreach (var request in results)
            {
                DisplayRequestDetails(request);
            }
        }

        private static void SearchByNumber()
        {
            Console.Write("Введите номер заявки: ");
            if (int.TryParse(Console.ReadLine(), out int id))
            {
                Request request = repository.GetById(id);
                if (request != null)
                {
                    DisplayRequestDetails(request);
                }
                else
                {
                    Console.WriteLine("Заявка не найдена.");
                }
            }
            else
            {
                Console.WriteLine("Неверный формат номера.");
            }
        }

        // При проверке данных
        private static void DisplayRequestDetails(Request request)
        {
            Console.WriteLine($"№{request.Id}");
            Console.WriteLine($"Заявитель: {request.ClientName}");
            Console.WriteLine($"Отдел: {request.Department}");
            Console.WriteLine($"Описание: {request.Description}");
            Console.WriteLine($"Категория: {request.Category}");
            Console.WriteLine($"Статус: {request.Status}");
            Console.WriteLine($"Исполнитель: {request.Executor ?? "Не назначен"}");
            Console.WriteLine($"Дата создания: {request.CreatedAt}");
            Console.WriteLine($"Дата завершения: {request.CompletedAt ?? "Не завершено"}\n");
        }
        private static void ClearScreen()
        {
            Console.Clear();
            Console.WriteLine("Система учёта заявок ИТ-отдела");
            Console.WriteLine("-----------------------------\n");
        }

        // Валидация ввода номера заявки
        private static bool ValidateRequestNumber(string input, out int id)
        {
            return int.TryParse(input, out id) && id > 0;
        }

        // Обработка ошибок при вводе 
        private static void HandleError(string errorMessage)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"\nОшибка: {errorMessage}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Нажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }

        // Сохранение данных (временная реализация)
        private static void SaveData()
        {
            // Здесь можно добавить логику сохранения в файл или базу данных
            Console.WriteLine("Данные сохранены.");
        }

        // Загрузка данных (временная реализация)
        private static void LoadData()
        {
            // Здесь можно добавить логику загрузки из файла или базы данных
            Console.WriteLine("Данные загружены.");
        }

        // Обработка выхода из программы
        private static void ExitProgram()
        {
            Console.WriteLine("\nСпасибо за использование системы!");
            Console.WriteLine("Программа завершает работу...");
            SaveData(); // Сохранение перед выходом
        }

        // Дополнительные методы форматирования вывода
        private static void DrawSeparator()
        {
            Console.WriteLine(new string('-', 40));
        }

        private static void WaitForUser()
        {
            Console.WriteLine("\nНажмите любую клавишу для продолжения...");
            Console.ReadKey();
        }

    }
     //Cоздаем базовый класс для работы с данными
    internal class ProgramBase
    {
        private const string DATA_FILE = "requests.json";

        public static void SaveRequests(List<Request> requests)
        {
            try
            {
                // Проверяем, что все свойства сериализуются корректно
                string json = JsonConvert.SerializeObject(
                    requests,Newtonsoft.Json.Formatting.Indented,
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore,
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    });

                File.WriteAllText(DATA_FILE, json);
                Console.WriteLine("Данные успешно сохранены!");
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Ошибка сериализации: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка сохранения: {ex.Message}");
            }
        }

        public static List<Request> LoadRequests()
        {
            if (!File.Exists(DATA_FILE))
            {
                return new List<Request>();
            }

            try
            {
                string json = File.ReadAllText(DATA_FILE);
                return JsonConvert.DeserializeObject<List<Request>>(
                    json,
                    new JsonSerializerSettings
                    {
                        MissingMemberHandling = MissingMemberHandling.Ignore
                    }) ?? new List<Request>();
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Ошибка десериализации: {ex.Message}");
                return new List<Request>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка загрузки: {ex.Message}");
                return new List<Request>();
            }
        }
    }
}
