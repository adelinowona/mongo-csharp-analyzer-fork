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

namespace MongoDB.Analyzer.Core.Builders;

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

    private static AnalysisStats ReportJson(MongoAnalyzerContext context, SyntaxNode[] DataModelAnalysis)
    {
        var semanticContext = context.SemanticModelAnalysisContext;
        if (DataModelAnalysis.)
        {
            return AnalysisStats.Empty;
        }

        var compilationResult = AnalysisCodeGenerator.Compile(context, buildersAnalysis);
        if (!compilationResult.Success)
        {
            return AnalysisStats.Empty;
        }

        var driverVersion = compilationResult.BuildersTestCodeExecutor.DriverVersion;
        var settings = context.Settings;
        int mqlCount = 0, internalExceptionsCount = 0, driverExceptionsCount = 0;

        foreach (var analysisContext in buildersAnalysis.AnalysisNodeContexts)
        {
            var mqlResult = compilationResult.BuildersTestCodeExecutor.GenerateMql(analysisContext.EvaluationMethodName);
            var location = analysisContext.Node.OriginalExpression.GetLocation();

            if (mqlResult.Mql != null)
            {
                var mql = analysisContext.Node.ConstantsRemapper.RemapConstants(mqlResult.Mql);

                var diagnostics = Diagnostic.Create(
                    BuidersDiagnosticsRules.DiagnosticRuleBuilder2MQL,
                    location,
                    DecorateMessage(mql, driverVersion, context.Settings));

                semanticContext.ReportDiagnostic(diagnostics);
                mqlCount++;
            }
            else if (mqlResult.Exception != null)
            {
                var isDriverException = mqlResult.Exception.InnerException?.Source?.Contains("MongoDB.Driver") == true;

                if (isDriverException || settings.OutputInternalExceptions)
                {
                    var diagnostics = Diagnostic.Create(
                        BuidersDiagnosticsRules.DiagnosticRuleNotSupportedBuilderExpression,
                        location,
                        DecorateMessage(mqlResult.Exception.InnerException?.Message ?? "Unsupported builders expression", driverVersion, context.Settings));

                    semanticContext.ReportDiagnostic(diagnostics);
                }

                if (!isDriverException)
                {
                    context.Logger.Log($"Exception while analyzing {analysisContext.Node}: {mqlResult.Exception.InnerException?.Message}");
                    internalExceptionsCount++;
                }
                else
                {
                    driverExceptionsCount++;
                }
            }
        }

        return new AnalysisStats(mqlCount, internalExceptionsCount, driverExceptionsCount, compilationResult.MongoDBDriverVersion.ToString(3), null);
    }

    private static string DecorateMessage(string message, string driverVersion, MongoDBAnalyzerSettings settings) =>
        settings.OutputDriverVersion ? $"{message}_v{driverVersion}" : message;
}
