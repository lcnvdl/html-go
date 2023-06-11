using HtmlRun.Runtime;

public class ContextTests
{
  private Context ctx;

  public ContextTests()
  {
    this.ctx = new Context(null, new Stack<Context>());
  }

  [Fact]
  public void Context_DeclareAndSetConst_ShouldWorkFine()
  {
    Assert.Null(this.ctx.GetVariable("pi"));

    this.ctx.DeclareAndSetConst("pi", Math.PI.ToString());

    var variable = this.ctx.GetVariable("pi");

    Assert.NotNull(variable);
    Assert.NotNull(variable!.Value);
    Assert.Equal(Math.PI.ToString(), variable!.Value);
    Assert.Equal("pi", variable!.Name);
    Assert.True(variable!.IsConst);
    Assert.False(variable!.IsUnset);
  }

  [Fact]
  public void Context_SetVariable_ShouldFail_IfIsNotDeclared()
  {
    Assert.Throws<InvalidOperationException>(() => this.ctx.SetVariable("notDeclared", "true"));
  }

  [Fact]
  public void Context_SetVariable_ShouldWorkFine()
  {
    Assert.Null(this.ctx.GetVariable("test"));

    this.ctx.DeclareVariable("test");

    var variable = this.ctx.GetVariable("test");

    Assert.NotNull(variable);
    Assert.Null(variable!.Value);
    Assert.Equal("test", variable!.Name);
    Assert.False(variable!.IsConst);
    Assert.True(variable!.IsUnset);

    this.ctx.SetVariable("test", "true");

    variable = this.ctx.GetVariable("test");

    Assert.NotNull(variable);
    Assert.NotNull(variable!.Value);
    Assert.Equal("true", variable!.Value);
    Assert.Equal("test", variable!.Name);
    Assert.False(variable!.IsConst);
    Assert.False(variable!.IsUnset);
  }
}