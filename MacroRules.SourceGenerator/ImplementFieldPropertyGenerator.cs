using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MacroRules.SourceGenerator;

[Generator]
public class ImplementFieldPropertyGenerator : IIncrementalGenerator
{
    private const string MacroMethodAttribute = "MacroRules.Attributes.MacroAttribute";
    private const string MacroAttribute = "MacroRules.Attributes.MacroAttribute";
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute to the compilation
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "macro_rules_base.g.cs", 
            SourceText.From(SourceGenerationHelper.MacroRulesTemplate, Encoding.UTF8)));
        
        var macros = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                MacroAttribute,
                predicate: static (s,_ ) => s is TypeDeclarationSyntax,
                transform: GetMacroDefinition)
            .Where(static m => m is not null);

        context.RegisterSourceOutput(macros,
            static (spc, macroToGenerate) => Execute(in macroToGenerate, spc));
    }
    
    static void Execute(in Macro? macroMethod, SourceProductionContext context)
    {
        if (macroMethod is { } eg)
        {
            StringBuilder sb = new StringBuilder();
            context.AddSource(""+ ".macro.g.cs", SourceText.From(sb.ToString(), Encoding.UTF8));    
        }
    }

    private Macro? GetMacroDefinition(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        if (context.TargetSymbol is not INamedTypeSymbol type)
        {
            // nothing to do if the method isn't available
            return null;
        }


        foreach (var macroAttribute in type.GetAttributes()
                     .Where(x => x.AttributeClass?.ToDisplayString() == MacroAttribute))
        {
            
        }
        
        
        ct.ThrowIfCancellationRequested();
        return new Macro("","","");
    }
}

internal record struct Macro(string MacroName, string MacroDefinition, string MacroSourceName); 

