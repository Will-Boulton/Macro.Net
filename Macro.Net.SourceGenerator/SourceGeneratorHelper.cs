namespace MacroRules.SourceGenerator;

internal static class SourceGeneratorHelper
{
    public const string GeneratedCodeFormatString =
        """
        namespace {0}
        {{
        {1}
        }}
        """;
}