namespace HtmlRun.Runtime.Interfaces;

public interface IRuntimeContext : IBaseContext
{
  List<string> Usings { get; }

  void AddUsing(string namesp);
}