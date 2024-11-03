using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

class Program
{
    static List<string> allCities = new List<string>();
    static List<string> usedCities = new List<string>();
    static List<string> players = new List<string>();
    static bool playAgainstComputer = false;

    static void Main()
    {
        LoadCitiesFromFile("cities.txt");
        Console.WriteLine("Добро пожаловать в игру 'Города'!");
        playAgainstComputer = ChooseGameMode();
        int playerCount = playAgainstComputer ? 1 : GetPlayerCount();

        for (int i = 1; i <= playerCount; i++)
        {
            players.Add($"Игрок {i}");
        }

        if (playAgainstComputer)
        {
            players.Add("Компьютер");
        }

        string previousCity = "";
        int currentPlayerIndex = 0;

        while (players.Count > 1)
        {
            string currentPlayer = players[currentPlayerIndex];
            Console.WriteLine($"\nХод {currentPlayer}");
            string currentCity;

            if (currentPlayer == "Компьютер")
            {
                currentCity = ComputerTurn(previousCity);
                if (string.IsNullOrEmpty(currentCity))
                {
                    Console.WriteLine("Компьютер не может назвать город и проигрывает!");
                    players.Remove(currentPlayer);
                    break;
                }
                else
                {
                    Console.WriteLine($"Компьютер назвал город: {currentCity}");
                }
            }
            else
            {
                Console.Write("Введите город: ");
                currentCity = Console.ReadLine().Trim();

                if (currentCity.ToLower() == "сдаюсь")
                {
                    Console.WriteLine($"{currentPlayer} сдался.");
                    players.RemoveAt(currentPlayerIndex);
                    if (currentPlayerIndex >= players.Count)
                    {
                        currentPlayerIndex = 0;
                    }
                    continue;
                }

                string currentCityLower = currentCity.ToLower();

                if (!IsCityInList(currentCityLower))
                {
                    Console.WriteLine("Такого города не существует. Попробуйте снова.");
                    continue;
                }
                else if (usedCities.Contains(currentCityLower))
                {
                    Console.WriteLine("Этот город уже был назван. Попробуйте другой.");
                    continue;
                }
                else if (!IsValidCity(previousCity, currentCityLower))
                {
                    Console.WriteLine($"Город должен начинаться с буквы '{GetLastLetter(previousCity)}'. Попробуйте снова.");
                    continue;
                }
            }

            usedCities.Add(currentCity.ToLower());
            previousCity = currentCity;

            if (!CanPlayerContinue(previousCity))
            {
                Console.WriteLine($"{currentPlayer} не может назвать город и проигрывает!");
                players.Remove(currentPlayer);
                break;
            }

            currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
        }

        if (players.Count == 1)
        {
            Console.WriteLine($"\nПобедил {players[0]}! Игра завершена.");
        }
    }

    static bool ChooseGameMode()
    {
        while (true)
        {
            Console.Write("Хотите играть против компьютера? (да/нет): ");
            string choice = Console.ReadLine().ToLower();

            if (choice == "да") return true;
            if (choice == "нет") return false;

            Console.WriteLine("Некорректный выбор. Пожалуйста, введите 'да' или 'нет'.");
        }
    }

    static int GetPlayerCount()
    {
        int count;
        while (true)
        {
            Console.Write("Введите количество игроков (2 или больше): ");
            if (int.TryParse(Console.ReadLine(), out count) && count >= 2)
            {
                return count;
            }
            else
            {
                Console.WriteLine("Некорректный ввод. Попробуйте снова.");
            }
        }
    }

    static string ComputerTurn(string previousCity)
    {
        Random rand = new Random();
        char lastLetter = GetLastLetter(previousCity);

        List<string> availableCities = allCities
            .Where(city => !usedCities.Contains(city.ToLower()) && city.ToLower()[0] == lastLetter)
            .ToList();

        if (availableCities.Count == 0) return null;

        string chosenCity = availableCities[rand.Next(availableCities.Count)];
        usedCities.Add(chosenCity.ToLower());
        return chosenCity;
    }

    static void LoadCitiesFromFile(string fileName)
    {
        try
        {
            allCities = new List<string>(File.ReadAllLines(fileName));
            Console.WriteLine("Города успешно загружены из файла.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка при загрузке файла: {ex.Message}");
        }
    }

    static bool IsCityInList(string city)
    {
        return allCities.Contains(city, StringComparer.OrdinalIgnoreCase);
    }

    static bool IsValidCity(string previousCity, string currentCity)
    {
        if (string.IsNullOrEmpty(previousCity)) return true;
        return currentCity[0] == GetLastLetter(previousCity);
    }

    static char GetLastLetter(string city)
    {
        city = city.ToLower();
        char lastChar = city[city.Length - 1];

        if (lastChar == 'ь' || lastChar == 'ы' || lastChar == 'ъ')
        {
            lastChar = city[city.Length - 2];
        }

        return lastChar;
    }

    static bool CanPlayerContinue(string previousCity)
    {
        char lastLetter = GetLastLetter(previousCity);
        return allCities.Any(city => !usedCities.Contains(city.ToLower()) && city.ToLower()[0] == lastLetter);
    }
}
