namespace HtmlRun.WebApi;

class ProgramArgs
{
  internal bool ShowVersionAndFinish { get; set; } = false;

  internal bool UseSwagger { get; set; } = false;

  internal bool Https { get; set; } = false;
  
  internal string File { get; set; } = "";
}