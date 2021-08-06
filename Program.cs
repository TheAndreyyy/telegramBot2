using System;
using System.IO;
using System.Collections.Generic;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.ReplyMarkups;

namespace telegramBot2
{
    class Program
    {
        static bool isBlocked = false;
        static long strangeId = 452548374;
        static List<List<string>> list1Turn = new();
        static List<List<string>> list2Orders = new();
        //  [0] - id
        //  [1] - name
        private static TelegramBotClient client;

        static void Main(string[] args)
        {
            Console.WriteLine("Приложение запущено, значит бот тоже");
            Console.WriteLine("чтобы остановить бота - нажмите Enter...");
            client = new TelegramBotClient("1892718242:AAFGZfp0AtP9nW71YSHeExHmc3_Gt32PDz4");
            client.StartReceiving();
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        public static int Contain(List<List<string>> list, string value, int column)
        {
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i][column] == value)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        public static string ShowInfo()
        {
            string str = "На заказе:\n";
            for (int i = 0; i < list2Orders.Count; i++)
            {
                str += "#" + (i + 1) + "\t" + list2Orders[i][1] + "\n";//name
            }

            str += "\nВ очереди:\n";

            for (int i = 0; i < list1Turn.Count; i++)
            {
                str += "#" + (i + 1) + "\t" + list1Turn[i][1] + "\n";//name
            }
            return str;
        }

        static async void OnMessageHandler(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                var msg = e.Message;
                string log = "empty";
                if (isBlocked == false)
                {
                    switch (e.Message.Text)
                    {
                        case "В очередь":
                            {
                                log = "в очередь: " + "@" + msg.Chat.Username + " | " + msg.Chat.FirstName;
                                await client.SendTextMessageAsync(
                                chatId: strangeId,
                                text: log,
                                replyMarkup: GetButtons(10));
                                //проверяем первый лист, если там - удаляем
                                int tempIndex = Contain(list2Orders, msg.From.Id.ToString(), 0);
                                if (tempIndex > -1)
                                {
                                    list2Orders.RemoveAt(tempIndex);
                                }
                                //проверяем второй, если там - не добавляем
                                tempIndex = Contain(list1Turn, msg.From.Id.ToString(), 0);
                                if (tempIndex > -1)
                                {
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: "вы уже в очереди",
                                    replyMarkup: GetButtons(10));
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: "вы встали в очередь",
                                    replyMarkup: GetButtons(10));
                                    list1Turn.Add(new());
                                    list1Turn[list1Turn.Count - 1].Add(msg.From.Id.ToString());
                                    list1Turn[list1Turn.Count - 1].Add(msg.From.FirstName);
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: ShowInfo(),
                                    replyMarkup: GetButtons(10));
                                }
                            }
                            break;
                        case "На заказ":
                            {
                                log = "на заказ: " + "@" + msg.Chat.Username + " | " + msg.Chat.FirstName;
                                await client.SendTextMessageAsync(
                                chatId: strangeId,
                                text: log,
                                replyMarkup: GetButtons(10));
                                //проверяем первый лист, если там - удаляем
                                int tempIndex = Contain(list1Turn, msg.From.Id.ToString(), 0);
                                if (tempIndex > -1)
                                {
                                    list1Turn.RemoveAt(tempIndex);
                                }
                                //проверяем второй, если там - не добавляем
                                tempIndex = Contain(list2Orders, msg.From.Id.ToString(), 0);
                                if (tempIndex > -1)
                                {
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: "вы уже на заказе",
                                    replyMarkup: GetButtons(10));
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: "вы уехали на заказ",
                                    replyMarkup: GetButtons(10));
                                    list2Orders.Add(new());
                                    list2Orders[list2Orders.Count - 1].Add(msg.From.Id.ToString());
                                    list2Orders[list2Orders.Count - 1].Add(msg.From.FirstName);
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: ShowInfo(),
                                    replyMarkup: GetButtons(10));
                                }
                            }
                            break;
                        case "Обновить":
                            {
                                log = "обновил: " + "@" + msg.Chat.Username + " | " + msg.Chat.FirstName;
                                await client.SendTextMessageAsync(
                                chatId: strangeId,
                                text: log,
                                replyMarkup: GetButtons(10));
                                await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                text: ShowInfo(),
                                replyMarkup: GetButtons(10));
                            }
                            break;
                        case "Уйти со смены":
                            {
                                log = "ушел: " + "@" + msg.Chat.Username + " | " + msg.Chat.FirstName;
                                await client.SendTextMessageAsync(
                                chatId: strangeId,
                                text: log,
                                replyMarkup: GetButtons(10));
                                int tempIndex = Contain(list1Turn, msg.From.Id.ToString(), 0);
                                if (tempIndex > -1)
                                {
                                    list1Turn.RemoveAt(tempIndex);
                                }
                                tempIndex = Contain(list2Orders, msg.From.Id.ToString(), 0);
                                if (tempIndex > -1)
                                {
                                    list2Orders.RemoveAt(tempIndex);
                                }
                                await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                text: "Вы ушли со смены",
                                replyMarkup: GetButtons(10));
                            }
                            break;
                        case "заблочить"://трабл в том, что некорректно отображает сообщения, админу неправильно и юзерам тоже
                            {
                                if (msg.From.Id == strangeId)
                                {
                                    isBlocked = true;
                                    await client.SendTextMessageAsync(
                                    chatId: strangeId,
                                    text: "заблочил",
                                    replyMarkup: GetButtons(11));
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: "неизвестная команда",
                                    replyMarkup: GetButtons(10));
                                }
                            }
                            break;
                        case "разблочить":
                            {
                                if (msg.From.Id == strangeId)
                                {
                                    isBlocked = false;
                                    await client.SendTextMessageAsync(
                                    chatId: strangeId,
                                    text: "разблочил",
                                    replyMarkup: GetButtons(11));
                                }
                                else
                                {
                                    await client.SendTextMessageAsync(
                                    chatId: msg.Chat.Id,
                                    text: "неизвестная команда",
                                    replyMarkup: GetButtons(10));
                                }
                            }
                            break;
                        default:
                            {
                                log = "написал: " + "@" + msg.Chat.Username + " | " + msg.Chat.FirstName + "\n" + "\"" + msg.Text + "\"";
                                await client.SendTextMessageAsync(
                                chatId: strangeId,
                                text: log,
                                replyMarkup: GetButtons(11));
                                await client.SendTextMessageAsync(
                                chatId: msg.Chat.Id,
                                text: "неизвестная команда",
                                replyMarkup: GetButtons(10));
                            }
                            break;
                    }
                }
                else
                {
                    await client.SendTextMessageAsync(
                    chatId: msg.Chat.Id,
                    text: "Приложение заблокировано...",
                    replyMarkup: GetButtons(100));
                }
                if (e.Message.From.Id == strangeId)
                {
                    await client.SendTextMessageAsync(
                    chatId: strangeId,
                    text: log,
                    replyMarkup: GetButtons(11));
                }
            }
        }

        private static IReplyMarkup GetButtons(int state)
        {
            switch (state)
            {
                case 10:
                    {
                        return new ReplyKeyboardMarkup
                        {
                            Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "В очередь"},new KeyboardButton { Text = "На заказ"}},
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Обновить"},new KeyboardButton { Text = "Уйти со смены"}}
                            }
                        };
                    }
                case 100:
                    {
                        return new ReplyKeyboardMarkup
                        {
                            Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Заблокировано"},new KeyboardButton { Text = "Заблокировано" } },
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Заблокировано" },new KeyboardButton { Text = "Заблокировано" } }
                            }
                        };
                    }
                case 11:
                    {
                        return new ReplyKeyboardMarkup
                        {
                            Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "заблочить"},new KeyboardButton { Text = "разблочить"}},
                                new List<KeyboardButton>{ new KeyboardButton { Text = "В очередь"},new KeyboardButton { Text = "На заказ"}},
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Обновить"},new KeyboardButton { Text = "Уйти со смены"}}
                            }
                        };
                    }
                default:
                    return null;
            }
        }
    }
}
