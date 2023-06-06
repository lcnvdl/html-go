using HtmlRun.Common.Models;

public interface IParser
{
  Task Load(string content);
  
  string HeadTitle { get; }
}