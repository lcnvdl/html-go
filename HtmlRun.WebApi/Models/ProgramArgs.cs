namespace HtmlRun.WebApi;

class ProgramArgs
{
  internal bool ShowVersionAndFinish { get; set; } = false;

  internal bool UseSwagger { get; set; } = false;

  internal bool Https { get; set; } = false;

  internal bool UseCors { get; set; } = true;

  internal string CorsOrigin { get; set; } = "*";
  
  internal string CorsMethods { get; set; } = "*";
  
  internal string CorsHeaders { get; set; } = "*";

  internal string File { get; set; } = "";
}