using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Providers;

public class MathProviderTests : BaseProviderTests
{
  public MathProviderTests() : base(new MathProvider())
  {
  }

  [Fact]
  public void MathProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void MathProvider_Clamp_Positive_ShouldWorkFine()
  {
    int result = Convert.ToInt32(this.CallJsInstruction(MathInstructionsSet.Clamp, 100, 0, 1) ?? throw new NullReferenceException());
    Assert.Equal(1, result);
  }

  [Fact]
  public void MathProvider_Clamp_Negative_ShouldWorkFine()
  {
    int result = Convert.ToInt32(this.CallJsInstruction(MathInstructionsSet.Clamp, -100, 0, 1) ?? throw new NullReferenceException());
    Assert.Equal(0, result);
  }

  [Fact]
  public void MathProvider_IncrementValue_ShouldWorkFine()
  {
    int result = Convert.ToInt32(this.CallEvalInstruction(MathInstructionsSet.IncrementValue, 0) ?? throw new NullReferenceException());
    Assert.Equal(1, result);
  }

  [Fact]
  public void MathProvider_DecrementValue_ShouldWorkFine()
  {
    int result = Convert.ToInt32(this.CallEvalInstruction(MathInstructionsSet.DecrementValue, 0) ?? throw new NullReferenceException());
    Assert.Equal(-1, result);
  }
}
