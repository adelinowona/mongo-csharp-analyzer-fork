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

namespace MongoDB.Analyzer.Core.DataModel;

internal static class DataModelAnalyzer
{
    public static bool AnalyzeDataModels(MongoAnalyzerContext context)
    {
        SyntaxNode[] DataModelAnalysis = null;

        try
        {
            context.Logger.Log("Started DataModel analysis");

            DataModelAnalysis = DataModelExpressionProcessor.ProcessSemanticModel(context);

            ReportJson(context, DataModelAnalysis);

            context.Logger.Log("DataModel analysis ended");
        }
        catch (Exception ex)
        {
            context.Logger.Log($"DataModel analysis ended with exception {ex}");
        }

        return false;
    }

    private static void ReportJson(MongoAnalyzerContext context, SyntaxNode[] DataModelAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;
        if (DataModelAnalysis.EmptyOrNull())
        {
            return;
        }

        var settings = context.Settings;

        foreach (var classDeclarationNode in DataModelAnalysis)
        {
            var location = classDeclarationNode.GetLocation();

            var diagnostics = Diagnostic.Create(
                DataModelDiagnosticsRules.DiagnosticRulePoco2Json,
                location,
                "testing2");

            semanticContext.ReportDiagnostic(diagnostics);
        }
    }

    private static string DecorateMessage(string message, int length) =>
        $"{message}_num:{length}";
}
