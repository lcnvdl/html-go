using HtmlRun.Runtime.Utils;
using Jurassic.Library;

class Dog
{
  public string Name { get; set; } = "";

  public bool IsMale { get; set; }

  public string Race { get; set; } = "";

  public int Age { get; set; }
}

public class JurassicUtilsTests
{
  public JurassicUtilsTests()
  {
  }

  [Fact]
  public void JurassicUtils_ToDictionary_ShouldWorkFine()
  {
    var engine = new Jurassic.ScriptEngine();

    var instance = engine.Object.Construct();
    instance["Name"] = "Lehia";
    instance["Race"] = "Chihuahua";
    instance["IsMale"] = false;
    instance["Age"] = DateTime.Now.Year - 2013;

    var dog = JurassicUtils.ToDictionary(instance);

    Assert.NotNull(dog);
    Assert.Equal("Lehia", dog["Name"]);
    Assert.Equal("Chihuahua", dog["Race"]);
    Assert.NotNull(dog["IsMale"]);
    Assert.False((bool)dog["IsMale"]!);
    Assert.IsAssignableFrom<int>(dog["Age"]);
  }

  [Fact]
  public void JurassicUtils_ToObject_ShouldWorkFine()
  {
    var engine = new Jurassic.ScriptEngine();

    var instance = engine.Object.Construct();
    instance["Name"] = "Lehia";
    instance["Race"] = "Chihuahua";
    instance["IsMale"] = false;
    instance["Age"] = DateTime.Now.Year - 2013;

    Dog? dog = JurassicUtils.ToObject<Dog>(instance);

    Assert.NotNull(dog);
    Assert.Equal("Lehia", dog!.Name);
    Assert.Equal("Chihuahua", dog!.Race);
    Assert.False(dog!.IsMale);
  }
}