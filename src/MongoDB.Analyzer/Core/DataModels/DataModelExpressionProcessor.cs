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

namespace MongoDB.Analyzer.Core.DataModels;

internal static class DataModelExpressionProcessor
{
    public static ExpressionsAnalysis ProcessSemanticModel(MongoAnalyzerContext context)
    {
        var semanticModel = context.SemanticModelAnalysisContext.SemanticModel;
        var syntaxTree = semanticModel.SyntaxTree;
        var root = syntaxTree.GetRoot();

        var analysisContexts = new List<ExpressionAnalysisContext>();
        var declarationContexts = new List<ClassAnalysisContext>();
        var invalidExpressionNodes = new List<InvalidExpressionAnalysisNode>();

        var typesProcessor = new TypesProcessor();

        var nodesProcessed = new HashSet<SyntaxNode>();
    
        // Find class declarations
        foreach (var node in root.DescendantNodes().Where(n => n.IsKind(SyntaxKind.ClassDeclaration) &&
                                                          n.DescendantNodes().OfType<ClassDeclarationSyntax>().Count() == 0))
        {
            var namedType = (INamedTypeSymbol) semanticModel.GetDeclaredSymbol(node);

            nodesProcessed.Add(node);

            try
            {
                typesProcessor.ProcessTypeSymbol(namedType);

                var declarationContext = new ClassAnalysisContext(node, typesProcessor.GetTypeSymbolToGeneratedTypeMapping(namedType));
                declarationContexts.Add(declarationContext);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed analyzing {node.NormalizeWhitespace()} with {ex.Message}");
            }
        }

        var DataModelAnalysis = new ExpressionsAnalysis()
        {
            AnalysisNodeContexts = analysisContexts.ToArray(),
            InvalidExpressionNodes = invalidExpressionNodes.ToArray(),
            TypesDeclarations = typesProcessor.TypesDeclarations,
            DeclarationNodeContexts = declarationContexts.ToArray()
        };

        context.Logger.Log($"DataModel: Found {nodesProcessed.Count} Class declarations.");

        return DataModelAnalysis;
    }
}