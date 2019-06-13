using Akavache;
using NodaTime;
using System;
using System.Reactive.Linq;

namespace AkavacheNodaTimeCore
{
    class Program
    {
        static TestModel BeforeModel;
        static TestModel AfterModel;
        static void Main(string[] args)
        {
            // Note that we're using Akavache 6.0.27, to match the version we're using in our live system.
            BlobCache.ApplicationName = "AkavacheNodaTimeCore";
            BlobCache.EnsureInitialized();

            BeforeModel = new TestModel()
            {
                StartLocalDateTime = LocalDateTime.FromDateTime(DateTime.Now),
                StartDateTime = DateTime.UtcNow,
            };

            Console.WriteLine($"Before:LocalDateTime='{BeforeModel.StartLocalDateTime}' DateTime='{BeforeModel.StartDateTime}'");

            CycleTheModels();

            Console.WriteLine($"After: LocalDateTime='{AfterModel.StartLocalDateTime}' DateTime='{AfterModel.StartDateTime}'");
            Console.WriteLine("Note that Akavache retrieves DateTimes as DateTimeKind.Local, so DateTime before and after above will differ.");

            Console.WriteLine("Press any key to continue.");
            var y = Console.ReadKey();

        }
        /// <summary>
        /// Puts a model into Akavache and retrieves a new one so we can compare.
        /// </summary>
        static async void CycleTheModels()
        {
            await BlobCache.InMemory.Invalidate("model");
            await BlobCache.InMemory.InsertObject("model", BeforeModel);

            AfterModel = await BlobCache.InMemory.GetObject<TestModel>("model");
        }
    }
}
