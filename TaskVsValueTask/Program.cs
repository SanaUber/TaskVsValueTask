using TaskVsValueTask.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Add CORS services here.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder.AllowAnyOrigin()    // Allows requests from any origin.
                   .AllowAnyHeader()    // Allows all request headers.
                   .AllowAnyMethod();   // Allows all HTTP methods (GET, POST, etc.).
        });
});
builder.Services.AddScoped<FoodService>();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
    // app.UseDeveloperExceptionPage();
}

app.UseRouting();

// Add this line to enable the CORS policy.
// It must come after UseRouting and before MapControllers.
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();

app.Run();
