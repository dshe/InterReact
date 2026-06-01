using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;

class Program
{
    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: AsyncRenamer <path-to-sln>");
            return 1;
        }

        string solutionPath = args[0];

        using var workspace = MSBuildWorkspace.Create();
        var solution = await workspace.OpenSolutionAsync(solutionPath);

        foreach (var project in solution.Projects)
        {
            var compilation = await project.GetCompilationAsync();
            if (compilation == null) continue;

            foreach (var document in project.Documents)
            {
                var model = await document.GetSyntaxRootAsync();
                if (model == null) continue;

                var semantic = await document.GetSemanticModelAsync();
                if (semantic == null) continue;

                var methods = model.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Where(m => m.Modifiers.Any(SyntaxKind.AsyncKeyword));

                foreach (var method in methods)
                {
                    var symbol = semantic.GetDeclaredSymbol(method) as IMethodSymbol;
                    if (symbol == null) continue;

                    if (!symbol.Name.EndsWith("Async", StringComparison.Ordinal))
                    {
                        var newName = symbol.Name + "Async";
                        Console.WriteLine($"Renaming {symbol.ToDisplayString()} -> {newName}");

                        var newSolution = await Renamer.RenameSymbolAsync(solution, symbol, newName, solution.Workspace.Options);
                        solution = newSolution;
                    }
                }
            }
        }

        if (workspace.TryApplyChanges(solution))
        {
            Console.WriteLine("Renames applied.");
            return 0;
        }

        Console.WriteLine("Failed to apply changes.");
        return 1;
    }
}
