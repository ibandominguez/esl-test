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
        static private String apId = "01";

        static private String shopId = "0001";

        static private List<TagEntity> tags = new List<TagEntity> {
            new TagEntity
            {
                TagID = "040A4251",                         // Tag ID,
                Token = DateTime.Now.Millisecond,           // Token
                G = true,                                   // Green color LED light turn on
                Before = false,                             // After refresh screen, flashing LED light
                Times = 2,                                  // LED light flashing times
                DataList = new List<DataEntity>             // Data list
                {
                    new TextEntity                          // Add a text entity
                    {
                        ID = 0,                             // Unique DataEntity ID
                        Top = 1,                            // Location, top 1px
                        Left = 1,                           // Location, left 1px
                        Data = "Hello",                     // Text data
                        InvertColor = true,                 // Yes, invert color
                        TextSize = TextSize.u16px           // Text size is 16px
                    },
                    new PriceEntity
                    {
                        ID = 1,
                        Top = 20,
                        Left = 10,
                        Data = "€8.00",
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
        };

        static void Main(String[] args)
        {
            Server.Instance.StationEventHandler += Instance_StationEventHandler;
            Server.Instance.ResultEventHandler += Instance_ResultEventHandler;
            Server.Instance.Start();

            Result response = Server.Instance.Send(shopId, apId, tags, true, true);

            if (response != Result.OK) {
                Console.WriteLine($">> Error: {response}");
            }

            Console.ReadKey();
        }

        private static void Instance_ResultEventHandler(Object sender, ResultEventArgs e)
        {
            Console.WriteLine($">> Shop Code: {e.ShopCode}, AP: {e.StationID}, Result Type: {e.ResultType}, Count: {e.ResultList.Count}");

            foreach (var item in e.ResultList)
            {
                Console.WriteLine($">> Tag ID: {item.TagID}, Status: {item.TagStatus}, Temperature: {item.Temperature}, Power: {item.PowerValue}, Signal: {item.Signal}, Token: {item.Token}");
            }
        }

        private static void Instance_StationEventHandler(Object sender, StationEventArgs e)
        {
            Console.WriteLine($">> Shop Code: {e.ShopCode}, AP: {e.StationID}, IP: {e.IP}, Online: {e.Online}");
        }
    }
}
