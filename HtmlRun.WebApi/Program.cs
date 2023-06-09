using HtmlRun.Common.Models;

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

var spider = new HtmlRun.Interpreter.Interpreters.SpiderInterpreter();

string file = Path.Combine(Environment.CurrentDirectory, "../Examples/WebApi/01-hello_world.html");

AppModel appModel = await spider.ParseString(File.ReadAllText(file));

app.UseHttpsRedirection();

// app.UseAuthorization();
// app.MapControllers();

runtime.Run(appModel, null);

// app.MapGet("/", () => "Works");

app.Run();
