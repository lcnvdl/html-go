using HtmlRun.Common.Models;
using HtmlRun.Runtime;
using HtmlRun.Runtime.Code;
using HtmlRun.Runtime.Constants;
using HtmlRun.Runtime.Models;
using HtmlRun.Runtime.Providers;
using HtmlRun.Runtime.RuntimeContext;
using HtmlRun.Tests.Stubs;
using HtmlRun.Tests.Stubs.Instructions;

public class OOPProviderTests : BaseProviderTests
{
  private AppModel application;

  private InstructionsGroup ig;

  private EntityModel UserEntity
  {
    get
    {
      var entity = new EntityModel();
      entity.Name = "User";
      entity.Attributes.Add(new EntityAttributeModel() { Name = "Nombre", SqlType = "Text", DefaultValue = "John Doe" });
      return entity;
    }
  }

  public OOPProviderTests() : base(new OOPProvider())
  {
    this.ig = InstructionsGroup.Main;

    this.application = new AppModel();
    this.application.Entities.Add(this.UserEntity);
    this.application.InstructionGroups.Add(this.ig);
    this.Runtime.Run(this.application, null);
  }

  [Fact]
  public void OOPProvider_GetInstructions_ShouldWorkFine()
  {
    base.TestGetInstructions();
  }

  [Fact]
  public void OOPProvider_ShouldInterpretEntity()
  {
    var instruction = this.GetInstruction(BasicInstructionsSet.New);
    Assert.NotNull(instruction);

    instruction.Action.Invoke(this.Ctx.Fork(this.Runtime, instruction.Key, new[] { ParsedArgument.String("Entities.User"), ParsedArgument.String("usuario") }));

    var userVar = Ctx.GetVariable("usuario");
    Assert.NotNull(userVar);
  }

  [Fact]
  public void OOPProvider_ShouldInterpretEntityVariables()
  {
    var instruction = this.GetInstruction(BasicInstructionsSet.New);
    Assert.NotNull(instruction);

    instruction.Action.Invoke(this.Ctx.Fork(this.Runtime, instruction.Key, new[] { ParsedArgument.String("Entities.User"), ParsedArgument.String("usuario") }));

    Assert.Single(Ctx.Heap.ItemsRef);
    Assert.Equal(0, Ctx.Heap.ItemsRef[0].Index);
    Assert.NotNull(Ctx.Heap.ItemsRef[0].Data);
    Assert.NotNull(Ctx.Heap.ItemsRef[0].AdditionalData);
    Assert.NotNull(Ctx.Heap.ItemsRef[0].AdditionalData!["Nombre"]);

    var userVar = Ctx.GetVariable("usuario");
    Assert.NotNull(userVar);
    Assert.Equal("0", userVar!.Value);

    var userNameVar = Ctx.GetVariable("usuario.Nombre");
    Assert.NotNull(userNameVar);
    Assert.Equal("John Doe", userNameVar!.Value);
  }
}