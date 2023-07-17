using HtmlRun.Common.Models;
using HtmlRun.Interpreter.Interpreters;
using Microsoft.AspNetCore.Cors.Infrastructure;

namespace HtmlRun.WebApi;

static class Program
{
  async static Task Main(string[] args)
  {
    var builder = WebApplication.CreateBuilder(args);
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var corsArgs = ProgramArgsProcessor.Preprocess(args);

    SetupCors(corsArgs, builder);

    var app = builder.Build();

    //  HtmlGo

    var argsModel = ProgramArgsProcessor.ProcessInputAndGetModel(args, 0, app.Logger);

    if (argsModel.ShowVersionAndFinish)
    {
      ShowCurrentVersion(app.Logger);
      return;
    }

    var runtime = Startup.GetRuntime(app, builder);

    Environment.SetEnvironmentVariable("ENTRY_FILE", argsModel.File);
    Environment.SetEnvironmentVariable("ENTRY_DIRECTORY", Path.GetDirectoryName(argsModel.File));

    AppModel appModel = await ReadAppFromFile(argsModel.File);

    if (app.Environment.IsDevelopment() || argsModel.UseSwagger)
    {
      app.UseSwagger();
      app.UseSwaggerUI();
    }

    if (argsModel.UseCors)
    {
      app.UseCors();
    }

    if (argsModel.Https)
    {
      app.UseHttpsRedirection();
    }

    runtime.Run(appModel, null);

    app.Run();
  }

  private static void SetupCors(ProgramArgs corsArgs, WebApplicationBuilder builder)
  {
    if (corsArgs.UseCors)
    {
      builder.Services.AddCors(options =>
      {
        options.AddDefaultPolicy(
          builder =>
          {
            if (corsArgs.CorsOrigin != "*")
            {
              builder.WithOrigins(corsArgs.CorsOrigin.Split(","));
            }
            else
            {
              builder.AllowAnyOrigin();
            }

            if (corsArgs.CorsHeaders != "*")
            {
              builder.WithHeaders(corsArgs.CorsHeaders.Split(","));
            }
            else
            {
              builder.AllowAnyHeader();
            }

            if (corsArgs.CorsMethods != "*")
            {
              builder.WithMethods(corsArgs.CorsMethods.Split(","));
            }
            else
            {
              builder.AllowAnyMethod();
            }

            builder.SetIsOriginAllowedToAllowWildcardSubdomains();
          });
      });
    }
  }

  private static void ShowCurrentVersion(ILogger logger)
  {
    var assembly = typeof(Program).Assembly;
    string version = assembly.GetName().Version?.ToString() ?? "0.0.0.0";
    logger.LogInformation($"v{version}");
  }

  private static async Task<AppModel> ReadAppFromFile(string file)
  {
    IInterpreter spider = new SpiderInterpreter();

    AppModel app = await spider.ParseString(file, m => File.ReadAllText(m));

    return app;
  }
}

