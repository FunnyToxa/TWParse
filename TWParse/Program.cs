using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TWParse
{
    class Program
    {
        static void Main(string[] args)
        {

            //Приветствие и сообщение о запросе авторизации приложения
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Добрый день");
            Console.WriteLine("Через 3 секунды Вы будете переадресованы на страницу авторизации Twitter");
            Task.Delay(1000).Wait();
            for (int i = 2; i > 0; i--)
            {
                Console.WriteLine($"{i}..");
                Task.Delay(1000).Wait();
            }

            //авторизация в твиттер 
            Twitter twitter = new Twitter();
            // Запрос у пользователя PingCode для доступа к приложению к аккануту пользователя
            var url = twitter.RequestAuth();
            //если не удалось получить url - то скорее всего не указан ключ для доступа к Twitter API
            if (url == "")
            {
                Console.WriteLine("Отсутствует или недействителен ключ для доступа приложения к API Twitter. Дальнейшая работа невозможна!");
                Console.ReadLine();
                return;
            }
            Process.Start(url); //запускаем авторизацию по ссылке
            Console.Write("Введите PIN Code с сайта: ");
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            var pinCode = Console.ReadLine();
            Console.ForegroundColor = ConsoleColor.Green;
            if (twitter.Auth(pinCode)) //TODO: Выполнено | обработать ошибки
            {
                bool isEnd = false;
                while (!isEnd)
                {
                    Console.WriteLine();
                    Console.Write("Введите имя аккаунта Twitter для поиска: ");
                    Console.ForegroundColor = ConsoleColor.DarkGreen;
                    string userName = Console.ReadLine();
                    Console.ForegroundColor = ConsoleColor.Green;

                    //если введено пустое имя - то выходим
                    if (userName.Trim() == "")
                    {
                        isEnd = true;
                        continue;
                    }
                    //подставляем @ при отсутствии
                    if (!userName.StartsWith("@"))
                        userName = $"@{userName}";

                    //ищем последние 5 твитов нужного нам пользователя
                    var tweetsText = twitter.GetTweetsText(userName, 5);
                    if (tweetsText == "")
                        Console.WriteLine("Твиты отсутствуют или пользователь не найден");
                    else
                    {
                        var stat = GetFreq(tweetsText); //считаем частотность букв
                        var publishText = $"{userName}, статистика для последних 5 твитов: {stat}";
                        Console.WriteLine(publishText);
                        //пробуем твитнуть статистику на страницу
                        var isPublish = twitter.PublishTweet(publishText);
                        if (isPublish)
                            Console.WriteLine("Твит со статистикой опубликован на странице");
                        else
                            Console.WriteLine("Не удалось твитнуть статистику...");
                    }
                }
            }
            else
            {
                Console.WriteLine("Вы не смогли авторизироваться в Twitter");
            }
            Console.WriteLine("Спасибо за использование продукта @TwParse");
            Console.ReadLine();
        }

        //функция подсчета частотности букв в тексте
        //TODO: сделать разделение по Языку если необходимо
        private static string GetFreq(string text)
        {
            //тест прописными
            var textlow = text.ToLower();
            //только буквы текста
            var textletter = new string(textlow.ToCharArray().Where(x => char.IsLetter(x)).ToArray());
            //получаем уникальные буквы в тексте 
            var textmas = textletter.ToCharArray().Distinct();
            //кол-во букв в тексте всего
            double txtltCount = textletter.ToCharArray().Count();
            //создаем словарь букв с их частотностью
            //var chs = textmas.ToDictionary(x => x, x => textlow.Count(y => y == x)/txtltCount);
            //формируем лист с объектами класса описвающего требуемую нам информацию.
            var chs = new List<TweetsInfo>(textmas.Select(x => new TweetsInfo(x, Math.Round(textlow.Count(y => y == x) / txtltCount, 4))));
            //формируем json
            var json = JsonConvert.SerializeObject(chs, Formatting.None, new CustomJsonConverter(typeof(TweetsInfo)));
            //TODO: убрать этот костыль (ниже), попробовать сделать в CustomJsonConverter
            json = json.Replace("\"", "").Replace("[", "").Replace("]", "").Replace("},{",",");//приводим к требуемому виду 
            return json;
        }
    }
}


