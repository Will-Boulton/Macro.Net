namespace MacroRules.Attributes;

[assembly:Macro("","")]


[AttributeUsage(AttributeTargets.Assembly)]
public class MacroAttribute(string macroName, string rules) : Attribute
{
    
}

[AttributeUsage(AttributeTargets.Method)]
public class MacroMethodAttribute(string macroName)
{
    
}