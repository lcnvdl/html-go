namespace HtmlRun.Interfaces;

public interface IExternalJumpWithMemory : IJumpWithMemory
{
  string CallApplicationId { get; set; }
  
  string ApplicationId { get; set; }
}