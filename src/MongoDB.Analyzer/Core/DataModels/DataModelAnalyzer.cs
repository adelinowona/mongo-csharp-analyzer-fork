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

internal static class DataModelAnalyzer
{
    public static bool AnalyzeDataModels(MongoAnalyzerContext context)
    {
        ExpressionsAnalysis DataModelAnalysis = null;

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

    private static void ReportJson(MongoAnalyzerContext context, ExpressionsAnalysis dataModelAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;
        if (dataModelAnalysis.DeclarationNodeContexts.EmptyOrNull())
        {
            return;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, dataModelAnalysis);
        if (!compilationResult.Success)
        {
            return;
        }

        var settings = context.Settings;
        var driverVersion = compilationResult.DataModelsTestCodeExecutor.DriverVersion;

        foreach (var classDeclarationContext in dataModelAnalysis.DeclarationNodeContexts)
        {
            var jsonResult = compilationResult.DataModelsTestCodeExecutor.GenerateJson(classDeclarationContext.EvaluationMethodName);
            var location = classDeclarationContext.Node.GetLocation();

            if (jsonResult.json != null)
            {
                var diagnostics = Diagnostic.Create(
                    DataModelDiagnosticsRules.DiagnosticRulePoco2Json,
                    location,
                    DecorateMessage(jsonResult.json, driverVersion, settings));

                semanticContext.ReportDiagnostic(diagnostics);
            }
        }
    }

    private static string DecorateMessage(string message, string driverVersion, MongoDBAnalyzerSettings settings) =>
       settings.OutputDriverVersion ? $"{message}_v{driverVersion}" : message;
}
