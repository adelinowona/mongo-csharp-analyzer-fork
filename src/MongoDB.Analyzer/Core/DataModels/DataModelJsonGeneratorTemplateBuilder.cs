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

internal sealed class DataModelJsonGeneratorTemplateBuilder
{
    private readonly SyntaxNode _root;
    private readonly ClassDeclarationSyntax _jsonGeneratorDeclarationSyntax;
    private readonly MethodDeclarationSyntax _mainTestMethodNode;
    private readonly SyntaxToken classNameToken;

    private ClassDeclarationSyntax _jsonGeneratorDeclarationSyntaxNew;

    private int _nextTestMethodIndex;

    public DataModelJsonGeneratorTemplateBuilder(SyntaxTree jsonGeneratorSyntaxTree)
    {
        _root = jsonGeneratorSyntaxTree.GetRoot();

        _jsonGeneratorDeclarationSyntax = _root.GetSingleClassDeclaration(JsonGeneratorSyntaxElements.JsonGenerator);
        _jsonGeneratorDeclarationSyntaxNew = _jsonGeneratorDeclarationSyntax;
        _mainTestMethodNode = _jsonGeneratorDeclarationSyntax.GetSingleMethod(JsonGeneratorSyntaxElements.JsonGeneratorMainMethodName);

        classNameToken = _mainTestMethodNode.DescendantTokens()
            .Single(n => n.IsKind(SyntaxKind.IdentifierToken) && n.Text == "TemplateType" && n.ValueText == "TemplateType");
    }

    public string AddClassDeclaration(string typeArgumentName, SyntaxNode classDeclaration)
    {
        var newToken = SyntaxFactory.Identifier(typeArgumentName);

        var newMethodDeclaration = _mainTestMethodNode.ReplaceToken(classNameToken, newToken);

        var newJsonGeneratorMethodName = $"{_mainTestMethodNode.Identifier.Value}_{ _nextTestMethodIndex++}";
        newMethodDeclaration = newMethodDeclaration.WithIdentifier(SyntaxFactory.Identifier(newJsonGeneratorMethodName));

        _jsonGeneratorDeclarationSyntaxNew = _jsonGeneratorDeclarationSyntaxNew.AddMembers(newMethodDeclaration);

        return newJsonGeneratorMethodName;
    }

    public SyntaxTree GenerateSyntaxTree() =>
        _root.ReplaceNode(_jsonGeneratorDeclarationSyntax, _jsonGeneratorDeclarationSyntaxNew).SyntaxTree;
}
