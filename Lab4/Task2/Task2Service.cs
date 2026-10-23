using Lab4.Task1;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Lab4.Task2
{
    public class Task2Service
    {
        private List<DictionaryModel> _dictionaries = new List<DictionaryModel>();
        private const string StorageDirectory = "Dictionaries";

        public Task2Service()
        {
            Directory.CreateDirectory(StorageDirectory);
            LoadAllDictionaries();
        }

        public void RunMenu()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("  ЗАВДАННЯ 2 (Додаткове: Програма «Словники»)");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Переглянути наявні словники");
                Console.WriteLine("2. Створити новий словник");
                Console.WriteLine("3. Вибрати словник для роботи");
                Console.WriteLine("0. <- Повернутися у Головне меню");
                Console.WriteLine("------------------------------------------");
                Console.Write("Виберіть підзавдання: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        ListDictionaries();
                        Pause();
                        break;
                    case "2":
                        CreateDictionary();
                        Pause();
                        break;
                    case "3":
                        SelectDictionaryMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некоректний вибір.");
                        Pause();
                        break;
                }
            }
        }

        private void ListDictionaries()
        {
            Console.WriteLine("\n--- СПИСОК СЛОВНИКІВ ---");
            if (!_dictionaries.Any())
            {
                Console.WriteLine("Словники відсутні.");
                return;
            }

            foreach (var d in _dictionaries)
            {
                Console.WriteLine($"- {d.Title} (Всього слів: {d.Words.Count})");
            }
        }

        private void CreateDictionary()
        {
            Console.WriteLine("\n--- СТВОРЕННЯ НОВОГО СЛОВНИКА ---");
            Console.Write("Введіть тип/назву словника (наприклад, 'Англо-український'): ");
            string? title = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(title))
            {
                Console.WriteLine("Помилка: Назва словника не може бути порожньою.");
                return;
            }

            if (_dictionaries.Any(d => d.Title.Equals(title, StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Помилка: Словник з такою назвою вже існує.");
                return;
            }

            var newDict = new DictionaryModel(title);
            _dictionaries.Add(newDict);
            SaveDictionary(newDict);
            Console.WriteLine($"[OK] Словник '{title}' успішно створено та збережено у файл.");
        }

        private void SelectDictionaryMenu()
        {
            if (!_dictionaries.Any())
            {
                Console.WriteLine("\nСловники поки не створені.");
                Pause();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine("       ВИБІР СЛОВНИКА ДЛЯ РОБОТИ");
                Console.WriteLine("==========================================");

                for (int i = 0; i < _dictionaries.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {_dictionaries[i].Title} (Слів: {_dictionaries[i].Words.Count})");
                }
                Console.WriteLine("0. <- Назад у меню Завдання 2");
                Console.WriteLine("------------------------------------------");
                Console.Write("Виберіть номер словника: ");

                if (int.TryParse(Console.ReadLine(), out int index) && index >= 0 && index <= _dictionaries.Count)
                {
                    if (index == 0) return;
                    DictionaryWorkMenu(_dictionaries[index - 1]);
                }
                else
                {
                    Console.WriteLine("Невірний вибір.");
                    Pause();
                }
            }
        }

        private void DictionaryWorkMenu(DictionaryModel dict)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine($" РОБОТА ЗІ СЛОВНИКОМ: [{dict.Title}]");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Шукати переклад слова");
                Console.WriteLine("2. Додати слово та переклад(и)");
                Console.WriteLine("3. Замінити слово або переклад");
                Console.WriteLine("4. Видалити слово або переклад");
                Console.WriteLine("5. Показати всі слова у словнику");
                Console.WriteLine("6. Експортувати слово з перекладами у файл");
                Console.WriteLine("0. <- Назад до вибору словника");
                Console.WriteLine("------------------------------------------");
                Console.Write("Виберіть дію: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        SearchTranslation(dict);
                        Pause();
                        break;
                    case "2":
                        AddWord(dict);
                        Pause();
                        break;
                    case "3":
                        EditSubMenu(dict);
                        break;
                    case "4":
                        DeleteSubMenu(dict);
                        break;
                    case "5":
                        ShowAllWords(dict);
                        Pause();
                        break;
                    case "6":
                        ExportWordToFile(dict);
                        Pause();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Некоректний вибір.");
                        Pause();
                        break;
                }
            }
        }

        private void SearchTranslation(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ПОШУК ПЕРЕКЛАДУ ---");
            Console.Write("Введіть слово для пошуку: ");
            string? query = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(query)) return;

            var entry = dict.Words.FirstOrDefault(w => w.Word.Equals(query, StringComparison.OrdinalIgnoreCase));
            if (entry != null)
            {
                Console.WriteLine($"\nСлово: {entry.Word}");
                Console.WriteLine("Варіанти перекладу:");
                foreach (var tr in entry.Translations)
                {
                    Console.WriteLine($" - {tr}");
                }
            }
            else
            {
                Console.WriteLine($"Слово '{query}' не знайдено.");
            }
        }

        private void AddWord(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ДОДАВАННЯ СЛОВА ---");
            Console.Write("Введіть нове слово: ");
            string? word = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(word))
            {
                Console.WriteLine("Помилка: Слово не може бути порожнім.");
                return;
            }

            var existing = dict.Words.FirstOrDefault(w => w.Word.Equals(word, StringComparison.OrdinalIgnoreCase));

            Console.WriteLine("Введіть варіанти перекладу через кому (наприклад: книга, том, книжка):");
            string? inputTr = Console.ReadLine();

            List<string> translations = (inputTr ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.Trim())
                .Where(t => !string.IsNullOrEmpty(t))
                .ToList();

            if (!translations.Any())
            {
                Console.WriteLine("Помилка: Вкажіть хоча б один переклад.");
                return;
            }

            if (existing != null)
            {
                int addedCount = 0;
                foreach (var tr in translations)
                {
                    if (!existing.Translations.Contains(tr, StringComparer.OrdinalIgnoreCase))
                    {
                        existing.Translations.Add(tr);
                        addedCount++;
                    }
                }
                Console.WriteLine($"[OK] Слово вже існувало. Додано нових варіантів перекладу: {addedCount}.");
            }
            else
            {
                dict.Words.Add(new WordEntry(word, translations));
                Console.WriteLine($"[OK] Слово '{word}' додано.");
            }

            SaveDictionary(dict);
        }

        private void EditSubMenu(DictionaryModel dict)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine($" РЕДАГУВАННЯ У СЛОВНИКУ [{dict.Title}]");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Замінити слово");
                Console.WriteLine("2. Замінити варіант перекладу");
                Console.WriteLine("0. <- Назад у меню словника");
                Console.WriteLine("------------------------------------------");
                Console.Write("Виберіть дію: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        ReplaceWord(dict);
                        Pause();
                        break;
                    case "2":
                        ReplaceTranslation(dict);
                        Pause();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        Pause();
                        break;
                }
            }
        }

        private void ReplaceWord(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ЗАМІНА СЛОВА ---");
            Console.Write("Введіть слово для зміни: ");
            string? oldWord = Console.ReadLine()?.Trim();

            var entry = dict.Words.FirstOrDefault(w => w.Word.Equals(oldWord, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            Console.Write($"Введіть нове значення замість '{entry.Word}': ");
            string? newWord = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(newWord)) return;

            entry.Word = newWord;
            SaveDictionary(dict);
            Console.WriteLine("[OK] Слово замінено.");
        }

        private void ReplaceTranslation(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ЗАМІНА ПЕРЕКЛАДУ ---");
            Console.Write("Введіть слово: ");
            string? word = Console.ReadLine()?.Trim();

            var entry = dict.Words.FirstOrDefault(w => w.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            Console.WriteLine("Наявні переклади:");
            for (int i = 0; i < entry.Translations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {entry.Translations[i]}");
            }

            Console.Write("Виберіть номер перекладу для заміни: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= entry.Translations.Count)
            {
                Console.Write($"Введіть новий переклад замість '{entry.Translations[idx - 1]}': ");
                string? newTr = Console.ReadLine()?.Trim();

                if (!string.IsNullOrWhiteSpace(newTr))
                {
                    entry.Translations[idx - 1] = newTr;
                    SaveDictionary(dict);
                    Console.WriteLine("[OK] Переклад успішно замінено.");
                }
            }
        }

        private void DeleteSubMenu(DictionaryModel dict)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("==========================================");
                Console.WriteLine($" ВИДАЛЕННЯ ЗІ СЛОВНИКА [{dict.Title}]");
                Console.WriteLine("==========================================");
                Console.WriteLine("1. Видалити слово (з усіма перекладами)");
                Console.WriteLine("2. Видалити конкретний варіант перекладу");
                Console.WriteLine("0. <- Назад у меню словника");
                Console.WriteLine("------------------------------------------");
                Console.Write("Виберіть дію: ");

                string? choice = Console.ReadLine()?.Trim();
                switch (choice)
                {
                    case "1":
                        DeleteWord(dict);
                        Pause();
                        break;
                    case "2":
                        DeleteTranslation(dict);
                        Pause();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Невірний вибір.");
                        Pause();
                        break;
                }
            }
        }

        private void DeleteWord(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ВИДАЛЕННЯ СЛОВА ---");
            Console.Write("Введіть слово для видалення: ");
            string? word = Console.ReadLine()?.Trim();

            var entry = dict.Words.FirstOrDefault(w => w.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            dict.Words.Remove(entry);
            SaveDictionary(dict);
            Console.WriteLine($"[OK] Слово '{entry.Word}' видалено.");
        }

        private void DeleteTranslation(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ВИДАЛЕННЯ ВАРІАНТА ПЕРЕКЛАДУ ---");
            Console.Write("Введіть слово: ");
            string? word = Console.ReadLine()?.Trim();

            var entry = dict.Words.FirstOrDefault(w => w.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            // Правило з методички: Не можна видалити останній переклад
            if (entry.Translations.Count <= 1)
            {
                Console.WriteLine("[ПОМИЛКА] У цього слова залишився тільки один варіант перекладу!");
                Console.WriteLine("Неможливо видалити останній переклад.");
                return;
            }

            Console.WriteLine("Переклади:");
            for (int i = 0; i < entry.Translations.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {entry.Translations[i]}");
            }

            Console.Write("Виберіть номер перекладу для видалення: ");
            if (int.TryParse(Console.ReadLine(), out int idx) && idx >= 1 && idx <= entry.Translations.Count)
            {
                string removed = entry.Translations[idx - 1];
                entry.Translations.RemoveAt(idx - 1);
                SaveDictionary(dict);
                Console.WriteLine($"[OK] Переклад '{removed}' видалено.");
            }
        }

        private void ShowAllWords(DictionaryModel dict)
        {
            Console.WriteLine($"\n--- ВСІ СЛОВА У СЛОВНИКУ [{dict.Title}] ---");
            if (!dict.Words.Any())
            {
                Console.WriteLine("Словник порожній.");
                return;
            }

            foreach (var w in dict.Words)
            {
                Console.WriteLine($"{w.Word} — {string.Join(", ", w.Translations)}");
            }
        }

        private void ExportWordToFile(DictionaryModel dict)
        {
            Console.WriteLine("\n--- ЕКСПОРТ СЛОВА У ФАЙЛ ---");
            Console.Write("Введіть слово для експорту: ");
            string? word = Console.ReadLine()?.Trim();

            var entry = dict.Words.FirstOrDefault(w => w.Word.Equals(word, StringComparison.OrdinalIgnoreCase));
            if (entry == null)
            {
                Console.WriteLine("Слово не знайдено.");
                return;
            }

            string fileName = $"Export_{entry.Word}.txt";
            string content = $"Словник: {dict.Title}\n" +
                             $"Слово: {entry.Word}\n" +
                             $"Варіанти перекладу:\n" +
                             string.Join("\n", entry.Translations.Select((t, i) => $"  {i + 1}. {t}")) + "\n" +
                             $"Дата експорту: {DateTime.Now:dd.MM.yyyy HH:mm:ss}";

            File.WriteAllText(fileName, content);
            Console.WriteLine($"[OK] Слово збережено у файл: {Path.GetFullPath(fileName)}");
        }

        private void SaveDictionary(DictionaryModel dict)
        {
            try
            {
                string safeName = string.Join("_", dict.Title.Split(Path.GetInvalidFileNameChars()));
                string filePath = Path.Combine(StorageDirectory, $"{safeName}.json");

                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(dict, options);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка збереження]: {ex.Message}");
            }
        }

        private void LoadAllDictionaries()
        {
            try
            {
                _dictionaries.Clear();
                var files = Directory.GetFiles(StorageDirectory, "*.json");
                foreach (var file in files)
                {
                    string json = File.ReadAllText(file);
                    var dict = JsonSerializer.Deserialize<DictionaryModel>(json);
                    if (dict != null) _dictionaries.Add(dict);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Помилка завантаження]: {ex.Message}");
            }
        }

        private void Pause()
        {
            Console.WriteLine("\nНатисніть будь-яку клавішу для продовження...");
            Console.ReadKey();
        }
    }
}
