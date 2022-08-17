// Copyright 2021-present MongoDB Inc.
//
// Licensed under the Apache License, Version 2.0 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using MongoDB.Analyzer.Core.Utilities;

namespace MongoDB.Analyzer.Core.DataModels;

internal static class AnalysisCodeGenerator
{
    private static readonly SyntaxTree s_jsonGeneratorSyntaxTree;

    static AnalysisCodeGenerator()
    {
        s_jsonGeneratorSyntaxTree = ResourcesUtilities.GetCodeResource(ResourceNames.JsonGenerator);
    }

    public static CompilationResult Compile(MongoAnalyzerContext context, ExpressionsAnalysis classDeclarationAnalysis)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var referencesContainer = ReferencesProvider.GetReferences(semanticModel.Compilation.References, context.Logger);
        if (referencesContainer == null)
        {
            return CompilationResult.Failure;
        }

        foreach (var node in classDeclarationAnalysis.DeclarationNodeContexts)
        {
            context.Logger.Log($"Class name: {node.ArgumentTypeName}");
        }

        var jsonGeneratorSyntaxTree = GenerateJsonGeneratorSyntaxTree(classDeclarationAnalysis);

        var compilation = CSharpCompilation.Create(
            DataModelAnalysisConstants.AnalysisAssemblyName,
            new[] { jsonGeneratorSyntaxTree },
            referencesContainer.References,
            new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        using var memoryStream = new MemoryStream();
        var emitResult = compilation.Emit(memoryStream);

        DataModelJsonGeneratorExecutor dataModelJsonCodeExecutor = null;

        if (emitResult.Success)
        {
            context.Logger.Log("Compilation successful");

            memoryStream.Seek(0, SeekOrigin.Begin);

            var jsonGeneratorType = DynamicTypeProvider.GetType(referencesContainer, memoryStream, JsonGeneratorSyntaxElements.JsonGeneratorFullName);

            dataModelJsonCodeExecutor = jsonGeneratorType != null ? new DataModelJsonGeneratorExecutor(jsonGeneratorType) : null;
        }
        else
        {
            context.Logger.Log($"Compilation failed with: {string.Join(Environment.NewLine, emitResult.Diagnostics)}");
        }

        var result = new CompilationResult(
            dataModelJsonCodeExecutor != null,
            dataModelJsonCodeExecutor,
            referencesContainer.Version);

        return result;
    }

    private static SyntaxTree GenerateJsonGeneratorSyntaxTree(ExpressionsAnalysis classDeclarationAnalysis)
    {
        var testCodeBuilder = new DataModelJsonGeneratorTemplateBuilder(s_jsonGeneratorSyntaxTree);

        foreach (var declarationContext in classDeclarationAnalysis.DeclarationNodeContexts)
        {
            var analysisNode = declarationContext.Node;

            declarationContext.EvaluationMethodName = testCodeBuilder.AddClassDeclaration(
                declarationContext.ArgumentTypeName,
                analysisNode);
        }

        return testCodeBuilder.GenerateSyntaxTree();
    }
}
