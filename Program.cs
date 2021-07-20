using System;
using eTag.SDK.Core;
using eTag.SDK.Core.Entity;
using eTag.SDK.Core.Enum;
using eTag.SDK.Core.Event;
using System.Collections.Generic;

namespace esl_test
{
    class Program
    {
        static String AP_ID = "01";
        static String SHOP_ID = "0001";
        static String ESL_ID = "040A4251";

        static void Main(String[] args)
        {
            SetupServer();
            RenderConsole("Configure your ESL");
            SendTestDemo();

            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void RenderConsole(String append)
        {
            Console.Clear();
            Console.WriteLine(">>>>>> Barnapes ESL Test");
            Console.WriteLine(">>>>>> Test out your demo kit.\n");
            Console.WriteLine("Current configuration:");
            Console.WriteLine("Access Point ID: {0}\nShop ID: {1}\nESL ID: {2}\n", AP_ID, SHOP_ID, ESL_ID);

            if (!String.IsNullOrEmpty(append)) {
                Console.WriteLine(append);
            }
        }

        private static void SetupServer()
        {
            Server.Instance.StationEventHandler += Instance_StationEventHandler;
            Server.Instance.ResultEventHandler += Instance_ResultEventHandler;
            Server.Instance.Start();
        }

        private static void SendTestDemo()
        {
            Random random = new Random(DateTime.Now.Millisecond);
            String title = GetUserString("Title");
            String price = GetUserString("Price");
            String barcode = GetUserString("Barcode");

            var result = Server.Instance.Send(
                SHOP_ID,
                AP_ID,
                new TagEntity
                {
                    TagID = ESL_ID,                             // Tag ID,
                    Token = random.Next(65535),                 // Token
                    G = true,                                   // Green color LED light turn on
                    Before = false,                             // After refresh screen, flashing LED light
                    Times = 2,                                  // LED light flashing times
                    DataList = new List<DataEntity>             // Data list
                    {
                        new TextEntity                          // Add a text entity
                        {
                            ID = 0,
                            Top = 1,                            // Location, top 1px
                            Left = 1,                           // Location, left 1px
                            Data = title,                       // Text data
                            InvertColor = true,                 // Yes, invert color
                            TextSize = TextSize.u16px           // Text size is 16px
                        },
                        new PriceEntity
                        {
                            ID = 1,
                            Top = 20,
                            Left = 10,
                            Data = price,
                            Color = FontColor.Red,
                            PriceSize = PriceSize.p32_16px
                        },
                        new BarcodeEntity
                        {
                            ID = 2,
                            Top = 60,
                            Left = 5,
                            Data = barcode,
                            BarcodeType = BarcodeType.Code128,
                            Height = 30
                        }
                    },
                },
                true,
                true
            );

            RenderConsole(">> AP Response: " + result);
        }

        private static String GetUserString(String key)
        {
            Console.Write("{0}: ", key);
            String userInput = Console.ReadLine();
            return String.IsNullOrEmpty(userInput) ? GetUserString(key) : userInput;
        }

        private static void Instance_ResultEventHandler(Object sender, ResultEventArgs e)
        {
            RenderConsole(
                ">> Shop Code: " + e.ShopCode +
                ", AP: " + e.StationID +
                ", Result Type: " + e.ResultType +
                ", Count: " + e.ResultList.Count
            );

            foreach (var item in e.ResultList)
            {
                RenderConsole(
                    ">> Tag ID: " + item.TagID +
                    ", Status: " + item.TagStatus +
                    ", Temperature: " + item.Temperature +
                    ", Power: " + item.PowerValue +
                    ", Signal: " + item.Signal +
                    ", Token: " + item.Token
                );
            }
        }

        private static void Instance_StationEventHandler(Object sender, StationEventArgs e)
        {
            RenderConsole("Shop Code: " + e.ShopCode + " AP: " + e.StationID + " IP: " + e.IP + " Online: " + e.Online);
        }
    }
}
