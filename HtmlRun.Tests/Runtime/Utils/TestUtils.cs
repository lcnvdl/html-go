static class TestUtils
{
  internal static void SaveTestFailure(string testName, string message, IEnumerable<object>? extraData)
  {
    var logs = new List<string>();
    logs.Add("Test failed: " + testName);
    logs.Add("\tException:");
    logs.Add("\t\t" + message.ReplaceLineEndings("\n\t\t"));
    logs.Add("\tExtra data:");
    if (extraData != null)
    {
      foreach (var item in extraData)
      {
        logs.Add("\t\t" + (item ?? "").ToString()!.ReplaceLineEndings("\n\t\t"));
      }
    }
    else
    {
      logs.Add("\t\tNone");
    }
    logs.Add("");
    logs.Add("");

    if (!Directory.Exists(@"C:\html-go"))
    {
      Directory.CreateDirectory(@"C:\html-go");
    }
    
    File.WriteAllText($@"C:\html-go\failed_test_{testName}.txt", string.Join("\n", logs));
  }
}