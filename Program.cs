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


        class Worker
        {
            public typesOfWorker typeOfWorker;
            public string name;
            public string id;
            public statesOfCourier state;

            public void Clear()
            {
                typeOfWorker = typesOfWorker.None;
                name = "";
                id = "";
                state = statesOfCourier.Empty;
            }
        }
        public enum typesOfWorker
        {
            Admin,
            Courier,
            None
        }
        static string typesOfWorkerOnRus(typesOfWorker type)
        {
            string temp = "";
            switch (type)
            {
                case typesOfWorker.Admin:
                    temp = "Админ";
                    break;
                case typesOfWorker.Courier:
                    temp = "Курьер";
                    break;
                case typesOfWorker.None:
                    temp = "Никто";
                    break;
            }
            return temp;
        }
        private static TelegramBotClient client;
        static string lastChoiced = "empty";
        static string prevState = "empty";
        //static List<List<string>> Couriers;//[0][00]: 0 - ID / 00 - info about this courier;
        //[00] - chatId(Courier)
        //[01] - name
        //[02] - current state
        static List<string> log = new List<string>();//[0][00]: 0 - exactly message / 00 - info about message;
                                                     //[00] - name of author
                                                     //[01] - time of send
        static Worker worker { get; set; } = new Worker();
        static List<Worker> onShift { get; set; }//общий список людей на смене
        static List<string> turn = new List<string>();
        //static statesOfCurier stateOfCurier;
        enum statesOfCourier
        {
            waitTurn,
            inOrder,
            justWait,
            Empty
        }

        static Worker GetWorkerOnID(string id)
        {
            int index = -1;
            for (int i = 0; i < onShift.Count; i++)
            {
                if (onShift[i].id == id)
                {
                    index = i;
                }
            }
            if (index != -1)
            {
                worker = onShift[index];
                return worker;
            }
            else
            {
                return null;
            }
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Приложение запущено, значит бот тоже");
            Console.WriteLine("чтобы остановить бота - нажмите Enter...");
            //onShift = new List<Worker>();
            client = new TelegramBotClient("1892718242:AAFGZfp0AtP9nW71YSHeExHmc3_Gt32PDz4");
            client.StartReceiving();
            //ShowInfo();
            //if (prevState == "empty")
            //{
            //    Console.WriteLine("started");
            //}
            client.OnMessage += OnMessageHandler;
            Console.ReadLine();
            client.StopReceiving();
        }

        //static public void ShowInfo()
        //{
        //    //on after end 
        //    //Console.Clear(); 
        //    if (onShift.Count == 0)
        //    {
        //        Console.WriteLine("На смене никого нет, а кто тогда читает? Авторизируйтесь через бота");
        //    }
        //    else
        //    {
        //        Console.WriteLine("На смене:");
        //        for (int i = 0; i < onShift.Count; i++)
        //        {
        //            //ConsoleColor color = Console.ForegroundColor;
        //            Console.Write($"#{i + 1} | Имя: {onShift[i].name} | ");
        //            //Console.ForegroundColor = ConsoleColor.Blue;
        //            Console.WriteLine($"{typesOfWorkerOnRus(onShift[i].typeOfWorker)}");
        //            //Console.ForegroundColor = color;
        //        }
        //    }
        //    Console.WriteLine();
        //    if (turn.Count == 0)
        //    {
        //        Console.WriteLine("Очередь пуста");
        //    }
        //    else
        //    {
        //        Console.WriteLine("Очередь:");
        //        for (int i = 0; i < turn.Count; i++)
        //        {
        //            worker = GetWorkerOnID(turn[i]);
        //            if (worker != null)
        //            {
        //                Console.WriteLine($"#{i + 1} | Имя: {worker.name}");
        //            }
        //        }
        //    }
        //    Console.WriteLine();
        //    Console.WriteLine("Полный лог сообщений за смену:");
        //    for (int i = 0; i < log.Count; i++)
        //    {
        //        Console.WriteLine(log[i]);
        //    }
        //}


        //пришел на смену (записался в список "на смене")
        //встал в очередь (запись в список "очередь")
        //уехал на заказ (удаление из списка "очередь")
        //приехал с заказа (ничего)
        //ушел со смены (удаление из списка "на смене")

        //static public string Calculate(string val)
        //{
        //    string result, temp;
        //    temp = val + "\n\n" + "Очередь:\n";
        //    for (int i = 0; i < list1Turn.Count; i++)
        //    {
        //        temp += list1Turn;
        //        if (i < list1Turn.Count - 1)
        //        {
        //            temp += "\n";
        //        }
        //    }

        //    //for (int i = 0; i < length; i++)
        //    //{

        //    //}

        //    //result = temp + "";
        //    //result = val + "\n" + result;
        //    return result = "test";
        //}

        //public static bool Contain(List<string> list, string value)
        //{
        //    for (int i = 0; i < list.Count; i++)
        //    {
        //        if (list[i] == value)
        //        {
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        public static int Contain(List<List<string>> list, string value, int column)
        {
            int index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i][column] == value)
                {
                    index = i;
                    return index;
                }
            }
            return -1;
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
            if (e.Message.Text != null /*&& e.Message.Chat.Id.ToString() != null*/)
            {
                var msg = e.Message;
                //log.Add(new List<string>());
                //log[log.Count - 1].Add(msg.Text);                       //exactly text
                //log[log.Count - 1].Add(msg.From.FirstName);             //name of author
                //log[log.Count - 1].Add(System.DateTime.Now.ToString()); //time of send
                //Console.Clear();
                //for (int i = 0; i < log.Count; i++)
                //{
                //    Console.WriteLine($"{log[i][2]}| {log[i][1]}: {log[i][0]}");
                //}
                //lastChoiced = e.Message.Text;
                //Console.WriteLine($" prevstate {prevState} | lastState {lastChoiced}");
                string log = "empty";
                if (isBlocked==false)
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

                                //for (int i = 0; i < list1Turn.Count; i++)
                                //{
                                //    if (list1Turn[i][0] == msg.From.Id.ToString())
                                //    {
                                //        list1Turn.RemoveAt(i);
                                //    }
                                //}
                                //List<string> temp = new();
                                //temp.Add(msg.From.Id.ToString());
                                //temp.Add(msg.From.FirstName);
                                //bool isDoublicate = false;
                                //if (Contain(list2Orders, msg.From.Id.ToString()))
                                //{
                                //    Console.WriteLine($"contain");

                                //}
                                //else
                                //{
                                //    Console.WriteLine($"not contain");
                                //    list2Orders.Add(temp);

                                //}
                                //if (list2Orders.Contains(temp))
                                //{
                                //    Console.WriteLine($"contain");
                                //}
                                //else
                                //{
                                //    Console.WriteLine($"not contain");
                                //    list2Orders.Add(temp);
                                //}

                                //Console.WriteLine($"{list2Orders[0][0]} {list2Orders[0][1]} | {temp[0]} {temp[1]}");

                                //for (int i = 0; i < list2Orders.Count; i++)
                                //{
                                //    if (list2Orders[i][0] == msg.From.Id.ToString())
                                //    {
                                //        isDoublicate = true;
                                //        break;
                                //    }
                                //}
                                //if (!isDoublicate)
                                //{
                                //    list2Orders.Add(temp);
                                //}
                                //await client.SendTextMessageAsync(
                                //chatId: msg.Chat.Id,
                                //text: Calculate("Вы поехали на заказ"),
                                //replyMarkup: GetButtons(10));
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
                                //for (int i = 0; i < list2Orders.Count; i++)
                                //{
                                //    if (list2Orders[i][0] == msg.From.Id.ToString())
                                //    {
                                //        list2Orders.RemoveAt(i);
                                //    }
                                //}
                                //for (int i = 0; i < list1Turn.Count; i++)
                                //{
                                //    if (list1Turn[i][0] == msg.From.Id.ToString())
                                //    {
                                //        list1Turn.RemoveAt(i);
                                //    }
                                //}
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
                                //log.Add($"{msg.From.FirstName} пишет какую-то дичь: \"{msg.Text}\"");
                                //Console.WriteLine(log[log.Count - 1]);
                            }
                            break;
                            //case "/start":
                            //    {
                            //        await client.SendTextMessageAsync(
                            //            chatId: msg.Chat.Id,
                            //            text: "Ахой, работяга! кем будешь на нашем судне?",
                            //            replyMarkup: GetButtons(0)
                            //            );
                            //        log.Add($"{System.DateTime.Now} | Зашел {msg.From.FirstName}");
                            //        Console.WriteLine(log[log.Count - 1]);
                            //    }
                            //    break;
                            //case "Я курьер":
                            //    {
                            //        //if (prevState == "empty")
                            //        //{
                            //        //    await client.SendTextMessageAsync(
                            //        //        chatId: msg.Chat.Id,
                            //        //        text: "Начало смены? давай на борт",
                            //        //        replyMarkup: GetButtons(0)
                            //        //        );
                            //        //}
                            //        //else
                            //        //{
                            //        await client.SendTextMessageAsync(
                            //           chatId: msg.Chat.Id,
                            //           text: "Отлично, салага! скорее поднимайся на палубу, капитан уже ждёт!",
                            //           replyMarkup: GetButtons(1)
                            //           );
                            //        log.Add($"{System.DateTime.Now} | Новый курьер: {msg.From.FirstName}");
                            //        Console.WriteLine(log[log.Count - 1]);
                            //        //Couriers.Add(new List<string>());
                            //        //Couriers[Couriers.Count - 1].Add(msg.Chat.Id.ToString());
                            //        //Couriers[Couriers.Count - 1].Add(msg.From.FirstName);
                            //        //Couriers[Couriers.Count - 1].Add("");
                            //        //prevState = 1;
                            //        // if (!CouriersonShift.Contains(msg.From.FirstName))
                            //        // {

                            //        // }
                            //        //CouriersonShift.Add(msg.From.FirstName);
                            //        //worker.Clear();
                            //        worker.id = msg.From.Id.ToString();
                            //        worker.name = msg.From.FirstName;
                            //        worker.state = statesOfCourier.justWait;
                            //        worker.typeOfWorker = typesOfWorker.Courier;
                            //        if (onShift.Contains(worker))
                            //        {
                            //            Console.WriteLine($"already have {worker.name}");
                            //        }
                            //        else
                            //        {
                            //            onShift.Add(worker);
                            //        }
                            //        //}

                            //    }
                            //    break;
                            //case "Я админ":
                            //    {
                            //        if (prevState == "empty")
                            //        {
                            //            await client.SendTextMessageAsync(
                            //                chatId: msg.Chat.Id,
                            //                text: "Начало смены? давай на борт",
                            //                replyMarkup: GetButtons(0)
                            //                );
                            //        }
                            //        else
                            //        {
                            //            await client.SendTextMessageAsync(
                            //           chatId: msg.Chat.Id,
                            //           text: $"Добро пожаловать на борт, капитан {msg.From.FirstName}!",
                            //           replyMarkup: GetButtons(1)
                            //           );
                            //            //worker.Clear();
                            //            worker.id = msg.From.Id.ToString();
                            //            worker.name = msg.From.FirstName;
                            //            worker.state = statesOfCourier.justWait;
                            //            worker.typeOfWorker = typesOfWorker.Admin;
                            //            if (onShift.Contains(worker))
                            //            {
                            //                Console.WriteLine($"already have {worker.name}");

                            //            }
                            //            else
                            //            {
                            //                onShift.Add(worker);
                            //            }
                            //            log.Add($"{System.DateTime.Now} | новый админ {msg.From.FirstName}");
                            //            Console.WriteLine(log[log.Count - 1]);
                            //        }
                            //    }
                            //    break;
                            //case "Встать в очередь":
                            //    {
                            //        if (prevState == "empty")
                            //        {
                            //            await client.SendTextMessageAsync(
                            //                chatId: msg.Chat.Id,
                            //                text: "Начало смены? давай на борт",
                            //                replyMarkup: GetButtons(0)
                            //                );
                            //        }
                            //        else
                            //        {
                            //            await client.SendTextMessageAsync(
                            //            chatId: msg.Chat.Id,
                            //            text: "Поздравляю, теперь ты в строю!",
                            //            replyMarkup: GetButtons(2)
                            //            );
                            //            if (!turn.Contains(msg.From.Id.ToString()))
                            //            {
                            //                turn.Add(msg.From.Id.ToString());
                            //            }
                            //            log.Add($"{System.DateTime.Now} | {msg.From.FirstName} встал в очередь");
                            //            Console.WriteLine(log[log.Count - 1]);
                            //        }
                            //    }
                            //    break;
                            //case "Уйти со смены":
                            //    {
                            //        if (prevState == "empty")
                            //        {
                            //            await client.SendTextMessageAsync(
                            //                chatId: msg.Chat.Id,
                            //                text: "Начало смены? давай на борт",
                            //                replyMarkup: GetButtons(0)
                            //                );
                            //        }
                            //        else
                            //        {
                            //            await client.SendTextMessageAsync(
                            //            chatId: msg.Chat.Id,
                            //            text: "Отлично поработали, можно и отдохнуть :)",
                            //            replyMarkup: GetButtons(0)
                            //            );
                            //            //worker.Clear();
                            //            worker.id = msg.From.Id.ToString();
                            //            worker.name = msg.From.FirstName;
                            //            worker.state = statesOfCourier.justWait;
                            //            worker.typeOfWorker = typesOfWorker.Admin;
                            //            if (onShift.Contains(worker))
                            //            {
                            //                //Console.WriteLine($"already have {worker.name}");

                            //            }
                            //            else
                            //            {
                            //                //onShift.Add(worker);
                            //            }
                            //            log.Add($"{System.DateTime.Now} | {msg.From.FirstName} ушел со смены");
                            //            Console.WriteLine(log[log.Count - 1]);
                            //        }
                            //    }
                            //    break;
                            //case "Поехал на заказ":
                            //    {
                            //        if (prevState == "empty")
                            //        {
                            //            await client.SendTextMessageAsync(
                            //                chatId: msg.Chat.Id,
                            //                text: "Начало смены? давай на борт",
                            //                replyMarkup: GetButtons(0)
                            //                );
                            //        }
                            //        else
                            //        {
                            //            await client.SendTextMessageAsync(
                            //            chatId: msg.Chat.Id,
                            //            text: "Спустить паруса! Штурвал по ветру! Поднять якорь! Отчаливаем, господа!",
                            //            replyMarkup: GetButtons(3)
                            //            );
                            //            log.Add($"{System.DateTime.Now} | {msg.From.FirstName} поехал на заказ");
                            //            Console.WriteLine(log[log.Count - 1]);
                            //        }
                            //    }
                            //    break;
                            //case "Приехал с заказа":
                            //    {
                            //        if (prevState == "empty")
                            //        {
                            //            await client.SendTextMessageAsync(
                            //                chatId: msg.Chat.Id,
                            //                text: "Начало смены? давай на борт",
                            //                replyMarkup: GetButtons(0)
                            //                );
                            //        }
                            //        else
                            //        {
                            //            await client.SendTextMessageAsync(
                            //            chatId: msg.Chat.Id,
                            //            text: "Мы пробирались сквозь шторма и ледяные бури...но все-таки добрались до дома!",
                            //            replyMarkup: GetButtons(1)
                            //            );
                            //            log.Add($"{System.DateTime.Now} | {msg.From.FirstName} приехал с заказа");
                            //            Console.WriteLine(log[log.Count - 1]);
                            //        }
                            //    }
                            //    break;
                            //default:
                            //    {
                            //        await client.SendTextMessageAsync(
                            //        chatId: msg.Chat.Id,
                            //        text: $"{msg.From.FirstName}, Ты что мне тут пишешь? а если я обижусь? ",
                            //        replyMarkup: GetButtons(0)
                            //        );
                            //        log.Add($"{msg.From.FirstName} пишет какую-то дичь: \"{msg.Text}\"");
                            //        Console.WriteLine(log[log.Count - 1]);
                            //    }
                            //    break;
                    }
                    //prevState = e.Message.Text;
                    //ShowInfo();
                    //await client.SendTextMessageAsync(chatId: e.Message.Chat, text: "You said:\n" + e.Message.Text);

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
                    //if (log != "empty")
                    //{
                        await client.SendTextMessageAsync(
                        chatId: strangeId,
                        text: log,
                        replyMarkup: GetButtons(11));
                    //}
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
                                new List<KeyboardButton>{ new KeyboardButton { Text = "В очередь"},new KeyboardButton { Text = "На заказ"}},
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Обновить"},new KeyboardButton { Text = "Уйти со смены"}},
                                new List<KeyboardButton>{ new KeyboardButton { Text = "заблочить"},new KeyboardButton { Text = "разблочить"}}
                            }
                        };
                    }
                case 0:
                    {
                        return new ReplyKeyboardMarkup
                        {
                            Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Я курьер"}, new KeyboardButton { Text = "Я админ"} }
                            }
                        };
                    }
                case 1:
                    {
                        Console.WriteLine($"prev {prevState}");
                        if (lastChoiced == "Я курьер")
                        {
                            return new ReplyKeyboardMarkup
                            {
                                Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Встать в очередь"}, new KeyboardButton { Text = "Уйти со смены" } }
                            }
                            };

                        }
                        else
                        {
                            return new ReplyKeyboardMarkup
                            {
                                Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{new KeyboardButton { Text = "Уйти со смены" } }
                            }
                            };
                        }
                    }
                case 2:
                    {
                        return new ReplyKeyboardMarkup
                        {
                            Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Поехал на заказ"}}
                            }
                        };
                    }
                case 3:
                    {
                        return new ReplyKeyboardMarkup
                        {
                            Keyboard = new List<List<KeyboardButton>>
                            {
                                new List<KeyboardButton>{ new KeyboardButton { Text = "Приехал с заказа"}}
                            }
                        };
                    }
                default:
                    return null;
            }

        }
    }
}
