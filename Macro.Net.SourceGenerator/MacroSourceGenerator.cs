using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MacroRules.SourceGenerator;

[Generator]
public class MacroSourceGenerator : IIncrementalGenerator
{
    private const string MacroArgumentAttribute = "Macro.Net.MacroArgumentAttribute";
    private const string MacroAttribute = "Macro.Net.MacroAttribute";
    private const string MacroInstanceTypeMetadataName = "Macro.Net.MacroInstance";
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
       var macros = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                MacroAttribute,
                predicate: static (s,_ ) => s is ClassDeclarationSyntax,
                transform: GetMacroDefinition)
            .Where(static m => m is not null);

        context.RegisterSourceOutput(macros,
            static (spc, macroToGenerate) => Execute(in macroToGenerate, spc));
    }
    
    static void Execute(in MacroTemplate? macroTemplate, SourceProductionContext context)
    {
        if (macroTemplate is { Instances: { Length: > 0 }} eg)
        {

            foreach (var macroInstance in eg.Instances)
            {
                var sb = new StringBuilder(eg.Template);
                foreach (var argument in macroInstance.Arguments)
                {
                    sb.Replace($"{{{argument.Name}}}", argument.Value);
                    
                }
                context.AddSource($"{eg.ParentClassName}.{macroInstance.Name}.macro.g.cs", SourceText.From(string.Format(SourceGeneratorHelper.GeneratedCodeFormatString,eg.Namespace, sb.ToString()), Encoding.UTF8));    
            }
        }
    }

    private MacroTemplate? GetMacroDefinition(GeneratorAttributeSyntaxContext context, CancellationToken ct)
    {
        var macroInstanceType = context.SemanticModel.Compilation.GetTypeByMetadataName(MacroInstanceTypeMetadataName);
        if (context.TargetSymbol is not INamedTypeSymbol type)
        {
            // nothing to do if the method isn't available
            return null;
        }

        if (!type.IsStatic)
        {
            // todo error only allowed on static classes
            return null;
        }

        var macroAttribute = type
            .GetAttributes()
            .Single(x => x.AttributeClass?.ToDisplayString() == MacroAttribute);


        if (macroAttribute.ConstructorArguments.First() is not { Kind: TypedConstantKind.Primitive, Type.SpecialType: SpecialType.System_String } template )
        {
            // todo error if the template argument isn't a string
            return null;
        }

        if (template.Value is not string templateValue)
        {
            // todo error if the template argument is null
            return null;
        }

        var instances = type.GetMembers()
            .Where(m => m is IFieldSymbol f && SymbolEqualityComparer.Default.Equals(f.Type, macroInstanceType))
            .Cast<IFieldSymbol>()
            .Select(instance =>
            {
                ct.ThrowIfCancellationRequested();
                var args = instance.GetAttributes()
                    .Where(x => x.AttributeClass?.ToDisplayString() == MacroArgumentAttribute)
                    .Select(at => new MacroArgument(
                        (string)at.ConstructorArguments[0].Value!,
                        (string)at.ConstructorArguments[1].Value!))
                    .ToImmutableArray();
                return new MacroInstance(instance.Name,args);
            }).ToImmutableArray();

        if (instances.Length == 0)
        {
            // todo warn if the template argument is null
            return null;
        }
        
        return new MacroTemplate(type.Name,templateValue, instances, type.ContainingNamespace.ToDisplayString());
    }
}

internal record struct MacroTemplate(string ParentClassName, string Template, ImmutableArray<MacroInstance> Instances, string Namespace);

internal record struct MacroInstance(string Name,ImmutableArray<MacroArgument> Arguments);

internal record struct MacroArgument(string Name, string Value); 
