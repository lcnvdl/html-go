var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var runtime = HtmlRun.WebApi.Startup.GetRuntime(app);

app.UseHttpsRedirection();

// app.UseAuthorization();
// app.MapControllers();

app.MapGet("/", () => "Works");

app.Run();
