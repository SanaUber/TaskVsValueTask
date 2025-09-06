using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace TaskVsValueTask.Services
{
    public class Food
    {
        public string Name { get; set; }
    }

    // A class to hold both the food list and performance data
    public class FoodResponse
    {
        public List<Food> Foods { get; set; }
        public PerformanceMetrics Performance { get; set; }
    }

    public class PerformanceMetrics
    {
        public string SpeedMs { get; set; }
        public string SimulatedMemoryMb { get; set; }
    }

    public class FoodService
    {
        private static readonly Dictionary<string, List<Food>> _foodCache = new Dictionary<string, List<Food>>();

        static FoodService()
        {
            // Initializing the cache with some data.
            _foodCache["ready"] = new List<Food>
            {
                new Food { Name = "Pizza" },
                new Food { Name = "Burger" },
                new Food { Name = "Pasta" }
            };
        }

        public async Task<FoodResponse> GetFoodWithTaskAsync(string foodType)
        {
            var stopwatch = Stopwatch.StartNew();
            var startMemory = GC.GetTotalMemory(true);
            List<Food> foodList;

            if (_foodCache.ContainsKey(foodType))
            {
                // Return data from cache, creating a new Task object.
                foodList = await Task.FromResult(_foodCache[foodType]);
            }
            else
            {
                // Simulate a time-consuming I/O operation for 'fresh' food.
                await Task.Delay(500);
                foodList = new List<Food>
                {
                    new Food { Name = "Salad" },
                    new Food { Name = "Soup" }
                };
            }

            stopwatch.Stop();
            var endMemory = GC.GetTotalMemory(true);
            var allocatedMemory = (endMemory - startMemory) / 1024.0 / 1024.0; // Convert bytes to MB

            var performanceMetrics = new PerformanceMetrics
            {
                SpeedMs = stopwatch.Elapsed.TotalMilliseconds.ToString("0.00"),
                SimulatedMemoryMb = allocatedMemory.ToString("0.00")
            };

            return new FoodResponse { Foods = foodList, Performance = performanceMetrics };
        }

        public ValueTask<FoodResponse> GetFoodWithValueTaskAsync(string foodType)
        {
            var stopwatch = Stopwatch.StartNew();
            var startMemory = GC.GetTotalMemory(true);
            List<Food> foodList;

            if (_foodCache.ContainsKey(foodType))
            {
                // Return data directly from cache as a ValueTask, no new Task object is created.
                foodList = _foodCache[foodType];
            }
            else
            {
                // Simulate a time-consuming I/O operation for 'fresh' food.
                System.Threading.Thread.Sleep(500); // Using synchronous sleep to avoid async overhead for demonstration.
                foodList = new List<Food>
                {
                    new Food { Name = "Chicken" },
                    new Food { Name = "Rice" }
                };
            }

            stopwatch.Stop();
            var endMemory = GC.GetTotalMemory(true);
            var allocatedMemory = (endMemory - startMemory) / 1024.0 / 1024.0; // Convert bytes to MB

            var performanceMetrics = new PerformanceMetrics
            {
                SpeedMs = stopwatch.Elapsed.TotalMilliseconds.ToString("0.00"),
                SimulatedMemoryMb = allocatedMemory.ToString("0.00")
            };

            return new ValueTask<FoodResponse>(new FoodResponse { Foods = foodList, Performance = performanceMetrics });
        }


         public ValueTask<List<Food>> GetBrokenValueTaskAsync()
        {
            // This method returns a ValueTask that the client will try to await multiple times,
            // demonstrating a common misuse pattern.
            System.Threading.Thread.Sleep(500);
            var foodList = new List<Food>
            {
                new Food { Name = "This is a broken food" }
            };
            return new ValueTask<List<Food>>(foodList);
        }
    }
}
