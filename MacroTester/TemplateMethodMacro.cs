using Macro.Net;

namespace MacroRulesTester;

public interface IMessage
{
   static abstract IMessage? Parse(string foo);
}

public partial class HelloMessage
{
    public static IMessage? Parse(string foo) => null;
}

public partial class GoodbyeMessage
{
    public static IMessage? Parse(string foo) => null;
}

[Macro("public partial class {TImp} : {TInterface};")]
public static class ImplementInterfaceMacro
{
    [MacroArgument("TImp", nameof(HelloMessage))]
    [MacroArgument("TInterface", nameof(IMessage))]
    public static MacroInstance Hello;
}


[Macro("""
       public partial class MessageParser
       {
            public static IMessage Parse{MessageType}(string foo) => {MessageType}.Parse(foo);
       }
       """)]
public static class TemplateMethodMacro
{
    [MacroArgument("MessageType", $"{nameof(Hello)}Message")]
    public static MacroInstance Hello;

    
    [MacroArgument("MessageType", $"{nameof(Goodbye)}Message")]
    public static MacroInstance Goodbye;

    public static void X()
    {
    }
}

