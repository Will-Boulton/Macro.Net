using JetBrains.Annotations;

namespace Macro.Net;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class MacroAttribute(string Template) : Attribute;