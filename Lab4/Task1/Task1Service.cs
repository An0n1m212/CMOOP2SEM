using System;
using System.Collections.Generic;
using System.Text;

namespace Lab4.Task1
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Xml.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;

    namespace Lab4App
    {
        public class Task1Service
        {
            private Record[] _records;

            public Task1Service()
            {
                // Ініціалізація початкового масиву об'єктів
                _records = new Record[]
                {
                new Record("Іваненко Іван Іванович",     "044-111-2233", "044-555-6677", "+380671234567", new DateTime(1995, 5, 15)),
                new Record("Петренко Петро Петрович",    "044-222-3344", "044-666-7788", "+380509876543", new DateTime(1998, 8, 22)),
                new Record("Сидоренко Анна Сергіївна",   "044-333-4455", "044-777-8899", "+380677778899", new DateTime(2001, 12, 3)),
                new Record("Коваленко Олексій Ігорович", "044-444-5566", "044-888-9900", "+380631112233", new DateTime(1992, 3, 30)),
                new Record("Мельник Ольга Миколаївна",  "044-555-6677", "044-999-0011", "+380505554433", new DateTime(1997, 10, 10))
                };
            }

            public void RunMenu()
            {
                while (true)
                {
                    Console.Clear();
                    Console.WriteLine("==================================================");
                    Console.WriteLine("  ЗАВДАННЯ 1 (Варіант 22: Клас «ЗАПИС»)");
                    Console.WriteLine("==================================================");
                    Console.WriteLine("1. Переглянути початковий масив об'єктів");
                    Console.WriteLine("2. XML серіалізація та десеріалізація (XmlSerializer)");
                    Console.WriteLine("3. JSON серіалізація та десеріалізація (JsonSerializer)");
                    Console.WriteLine("4. Виконати запит: Пошук людей з однаковим оператором");
                    Console.WriteLine("0. <- Повернутися у Головне меню");
                    Console.WriteLine("--------------------------------------------------");
                    Console.Write("Виберіть підзавдання: ");

                    string? choice = Console.ReadLine()?.Trim();
                    switch (choice)
                    {
                        case "1":
                            ShowRecords(_records, "ПОЧАТКОВИЙ МАСИВ ЗАПИСІВ");
                            Pause();
                            break;
                        case "2":
                            ExecuteXmlSerialization();
                            Pause();
                            break;
                        case "3":
                            ExecuteJsonSerialization();
                            Pause();
                            break;
                        case "4":
                            ExecuteOperatorQuery();
                            Pause();
                            break;
                        case "0":
                            return; // Повернення до головного меню
                        default:
                            Console.WriteLine("Некоректний вибір. Спробуйте ще раз.");
                            Pause();
                            break;
                    }
                }
            }

            private void ShowRecords(Record[] data, string title)
            {
                Console.WriteLine($"\n=== {title} ===");
                foreach (var r in data)
                {
                    Console.WriteLine(r);
                    Console.WriteLine(new string('-', 40));
                }
            }

            private void ExecuteXmlSerialization()
            {
                Console.WriteLine("\n--- 3. XML серіалізація (XmlSerializer) ---");
                string path = "records.xml";
                XmlSerializer serializer = new XmlSerializer(typeof(Record[]));

                using (FileStream fs = new FileStream(path, FileMode.Create))
                {
                    serializer.Serialize(fs, _records);
                    Console.WriteLine($"[OK] Об'єкти збережено у XML-файл '{path}'");
                }

                using (FileStream fs = new FileStream(path, FileMode.Open))
                {
                    Record[]? restored = serializer.Deserialize(fs) as Record[];
                    Console.WriteLine($"[OK] Об'єкти прочитано з XML-файлу. Прочитано елементів: {restored?.Length ?? 0}");
                }
            }

            private void ExecuteJsonSerialization()
            {
                Console.WriteLine("\n--- 4. JSON серіалізація (JsonSerializer) ---");
                string path = "records.json";
                var options = new JsonSerializerOptions { WriteIndented = true };

                string json = JsonSerializer.Serialize(_records, options);
                File.WriteAllText(path, json);
                Console.WriteLine($"[OK] Об'єкти збережено у JSON-файл '{path}'");

                string readJson = File.ReadAllText(path);
                Record[]? restored = JsonSerializer.Deserialize<Record[]>(readJson);
                Console.WriteLine($"[OK] Об'єкти прочитано з JSON-файлу. Прочитано елементів: {restored?.Length ?? 0}");
            }

            private void ExecuteOperatorQuery()
            {
                Console.WriteLine("\n--- 5. Результат запиту: Групування за мобільними операторами ---");

                var groups = _records
                    .GroupBy(r => r.GetMobileOperatorCode())
                    .Where(g => g.Count() > 1);

                bool found = false;
                foreach (var group in groups)
                {
                    found = true;
                    Console.WriteLine($"\n>>> Мобільний оператор (Код {group.Key}) — Абонентів: {group.Count()} <<<");
                    foreach (var person in group)
                    {
                        Console.WriteLine(person);
                        Console.WriteLine();
                    }
                }

                if (!found)
                {
                    Console.WriteLine("Не знайдено кілька осіб із однаковим оператором.");
                }
            }

            private void Pause()
            {
                Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
                Console.ReadKey();
            }
        }
    }
}
