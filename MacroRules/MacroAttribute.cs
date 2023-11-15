namespace MacroRules;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MacroAttribute(string Template) : Attribute;