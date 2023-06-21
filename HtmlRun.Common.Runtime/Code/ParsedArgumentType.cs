namespace HtmlRun.Runtime.Code;

[Flags]
public enum ParsedArgumentType
{
  Null = 0,
  Native = 1,
  Number = 3,
  String = 5,
  Reference = 128
}