using TaskVsValueTask.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Threading.Tasks;

namespace TaskVsValueTask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FoodController : ControllerBase
    {
        private readonly FoodService _foodService;

        public FoodController(FoodService foodService)
        {
            _foodService = foodService;
        }

        [HttpGet("task/{foodType}")]
        public async Task<IActionResult> GetFoodWithTask(string foodType)
        {
            var food = await _foodService.GetFoodWithTaskAsync(foodType);
            return Ok(food);
        }

        [HttpGet("valuetask/{foodType}")]
        public async Task<IActionResult> GetFoodWithValueTask(string foodType)
        {
            var food = await _foodService.GetFoodWithValueTaskAsync(foodType);
            return Ok(food);
        }

        // Endpoint to get food and performance data for Task
        [HttpGet("benchmark/task/{foodType}")]
        public async Task<IActionResult> GetFoodAndPerformanceWithTaskAsync(string foodType)
        {
            var stopwatch = Stopwatch.StartNew();
            var foodList = await _foodService.GetFoodWithTaskAsync(foodType);
            stopwatch.Stop();

            var performanceMetrics = new
            {
                speedMs = stopwatch.Elapsed.TotalMilliseconds.ToString("0.00"),
                simulatedMemoryMb = "41.2" // Simulated based on benchmark results
            };

            var responseData = new
            {
                foods = foodList,
                performance = performanceMetrics
            };

            return Ok(responseData);
        }

        // Endpoint to get food and performance data for ValueTask
        [HttpGet("benchmark/valuetask/{foodType}")]
        public async Task<IActionResult> GetFoodAndPerformanceWithValueTaskAsync(string foodType)
        {
            var stopwatch = Stopwatch.StartNew();
            var foodList = await _foodService.GetFoodWithValueTaskAsync(foodType);
            stopwatch.Stop();

            var performanceMetrics = new
            {
                speedMs = stopwatch.Elapsed.TotalMilliseconds.ToString("0.00"),
                simulatedMemoryMb = "20.6" // Simulated based on benchmark results
            };

            var responseData = new
            {
                foods = foodList,
                performance = performanceMetrics
            };

            return Ok(responseData);
        }



        // Endpoint to demonstrate the misuse of ValueTask by a client
        [HttpGet("valuetask/broken")]
        public async Task<IActionResult> GetBrokenValueTaskAsync()
        {
            try
            {
                // This is the correct place to show the misuse.
                // We call the service method which returns a ValueTask
                var brokenValueTask = _foodService.GetBrokenValueTaskAsync();

                // Await the same ValueTask twice. This will cause an exception.
                var result1 = await brokenValueTask;
                var result2 = await brokenValueTask; // This is where the exception will occur

                return Ok("This should not be reached.");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest($"InvalidOperationException: {ex.Message}");
            }
        }
    }
    }
