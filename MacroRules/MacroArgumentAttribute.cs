namespace MacroRules;

[AttributeUsage(AttributeTargets.Field)]
public class MacroArgumentAttribute(string ArgumentName, string value): Attribute;