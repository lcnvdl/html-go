namespace HtmlRun.Runtime.Exceptions;

public class VariableNotFoundException : Exception
{
  public VariableNotFoundException(string variableName)
  : base($"Variable {variableName} not found.")
  {
  }
}
