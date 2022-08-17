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

public static class DataModelDiagnosticsRules
{
    private const string DiagnosticIdPoco2Json = "MADataModel3001";
    private const string Category = "MongoDB.Analyzer.DataModels";

    public static readonly DiagnosticDescriptor DiagnosticRulePoco2Json = new(
        id: DiagnosticIdPoco2Json,
        title: "POCOs to JSON",
        messageFormat: "{0}",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public static DiagnosticDescriptor[] DiagnosticsRules { get; } = new[]
    {
        DiagnosticRulePoco2Json,
    };
}
