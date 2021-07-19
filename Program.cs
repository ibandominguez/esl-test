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
            ShowWelcomeMessage();
            SendTestDemo();

            Console.ReadKey();
            Console.ReadKey();
            Console.ReadKey();
        }

        private static void ShowWelcomeMessage()
        {
            Console.Clear();
            Console.WriteLine(">>>>>> ESL Test");
            Console.WriteLine(">>>>>> Test out your demo kit.\n");
            Console.WriteLine("This is the default configuration:");
            Console.WriteLine("Access Point ID: {0}\nShop ID: {1}\nESL ID: {2}\n", AP_ID, SHOP_ID, ESL_ID);
            Console.WriteLine("> Press Enter to update the ESL:");
            Console.ReadKey();
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
                            Data = "Hello World! ?? ABC123",    // Text data
                            InvertColor = true,                 // Yes, invert color
                            TextSize = TextSize.u16px           // Text size is 16px
                        },
                        new PriceEntity
                        {
                            ID = 1,
                            Top = 20,
                            Left = 10,
                            Data = "$9.87",
                            Color = FontColor.Red,
                            PriceSize = PriceSize.p32_16px
                        },
                        new BarcodeEntity
                        {
                            ID = 2,
                            Top = 60,
                            Left = 5,
                            Data = "1234567890",
                            BarcodeType = BarcodeType.Code128,
                            Height = 30
                        }
                    },
                },
                true,
                true
            );

            Console.Clear();
            Console.WriteLine(">>>>>> ESL Test Result:\n{0}", result);
        }

        private static void Instance_ResultEventHandler(Object sender, ResultEventArgs e)
        {
            Console.WriteLine(
                "Shop Code:{0}, AP:{1}, Result Type:{2}, Count:{3}",
                e.ShopCode,
                e.StationID,
                e.ResultType,
                e.ResultList.Count
            );

            foreach (var item in e.ResultList)
            {
                Console.WriteLine(
                    ">> Tag ID:{0}, Status:{1}, Temperature:{2}, Power:{3}, Signal:{4}, Token:{5}",
                    item.TagID,
                    item.TagStatus,
                    item.Temperature,
                    item.PowerValue,
                    item.Signal,
                    item.Token
                );
            }
        }

        private static void Instance_StationEventHandler(Object sender, StationEventArgs e)
        {
            Console.WriteLine("Shop Code:{0} AP: {1} IP:{2} Online:{3}", e.ShopCode, e.StationID, e.IP, e.Online);
        }
    }
}
