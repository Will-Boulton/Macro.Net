namespace Macro.Net;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
public class MacroArgumentAttribute(string ArgumentName, string value): Attribute;