using Macro.Net;

namespace MacroRulesTester;

[Macro("""
       public unsafe struct FixedBuffer{Size}
       {
           public const int Size = {Size};
           private fixed byte _data[Size];
       }
       """)]
public static class FixedBufferMacro
{
    [MacroArgument("Size","1")]
    public static MacroInstance M1;
    
    [MacroArgument("Size","10")]
    public static MacroInstance M10;
    
    [MacroArgument("Size","20")]
    public static MacroInstance M20;
}


public class FixedBufferTester
{
    // these fields types are auto generated from the macro 
    public FixedBuffer1 FixedBuffer1;
    public FixedBuffer10 FixedBuffer10;
    public FixedBuffer20 FixedBuffer20;
}