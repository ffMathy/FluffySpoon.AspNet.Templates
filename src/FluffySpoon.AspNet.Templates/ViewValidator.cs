using FluffySpoon.AspNet.Templates.Exceptions;
using Microsoft.AspNetCore.Mvc.Razor.Extensions;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.AspNetCore.Razor.Language.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluffySpoon.AspNet.Templates
{
    class ViewValidator : IViewValidator
    {
        public async Task<ViewValidationException> ValidateAsync(string viewPath)
        {
            var fileSystem = RazorProjectFileSystem.Create(".");

            var engine = RazorProjectEngine.Create(
                RazorConfiguration.Default,
                fileSystem,
                (builder) =>
                {
                    InheritsDirective.Register(builder);
                    ModelDirective.Register(builder);
                });

            var item = fileSystem.GetItem(viewPath);
            var codeDocument = engine.Process(item);

            var csharpDocument = codeDocument.GetCSharpDocument();
            var csharpTree = CSharpSyntaxTree.ParseText(csharpDocument.GeneratedCode);

            var rootNode = await csharpTree.GetRootAsync();

            var memberAccesses = FetchChildrenRecursivelyFromNode(
                rootNode,
                SyntaxKind.SimpleMemberAccessExpression);
            foreach(var memberAccess in memberAccesses)
            {
                var simpleMemberAccess = memberAccess as MemberAccessExpressionSyntax;
                if(simpleMemberAccess == null)
                    continue;

                while(simpleMemberAccess.Expression is MemberAccessExpressionSyntax)
                    simpleMemberAccess = (MemberAccessExpressionSyntax)simpleMemberAccess.Expression;

                var identifier = simpleMemberAccess.Expression as IdentifierNameSyntax;
                if(identifier == null)
                    continue;

                var propertyName = identifier.Identifier.Text;
                if(propertyName != "Model")
                    return new ViewValidationException("The namespace " + propertyName + " is not allowed. You can only use Model when evaluating expressions.");

                var methodName = simpleMemberAccess.Name;
                var allowedMethodNames = new List<string>()
                {
                    nameof(ApiModel.Get),
                    nameof(ApiModel.GetCollection)
                };
                if(!allowedMethodNames.Contains(methodName.Identifier.Text))
                    return new ViewValidationException("The method " + methodName + " on the model object is not allowed. You can only call the following methods on that object: " + allowedMethodNames.Aggregate((a, b) => a + ", " + b) + ".");
            }

            return null;
        }

        private static List<SyntaxNode> FetchChildrenRecursivelyFromNode(
            SyntaxNode node,
            params SyntaxKind[] kinds)
        {
            var relevantNodes = node
                .ChildNodes()
                .Where(x => kinds.Contains(x.Kind()))
                .ToList();

            var nonRelevantNodes = node
                .ChildNodes()
                .Where(x => !kinds.Contains(x.Kind()));

            var nonExpressionStatementQueue = new Queue<SyntaxNode>();
            foreach (var nonExpressionStatement in nonRelevantNodes)
                nonExpressionStatementQueue.Enqueue(nonExpressionStatement);

            while (nonExpressionStatementQueue.Count > 0)
            {
                var children = nonExpressionStatementQueue
                    .Dequeue()
                    .ChildNodes();

                foreach (var child in children)
                {
                    if (kinds.Contains(child.Kind()))
                    {
                        relevantNodes.Add(child);
                    }
                    else
                    {
                        nonExpressionStatementQueue.Enqueue(child);
                    }
                }
            }

            return relevantNodes;
        }
    }
}
