using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stryker.Core.Mutants;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Stryker.Core.Mutators
{
    /// <summary>
    /// Mutator that will mutate the access to <c>string.IsNullOrEmpty</c> to a string that is not empty.
    /// </summary>
    /// <remarks>
    /// Will only apply the mutation to the lowercase <c>string</c> since that is a reserved keyword in c# and can be distinguished from any variable or member access.
    /// </remarks>
    public class StringNullOrEmptyMutator : MutatorBase<ExpressionStatementSyntax>, IMutator
    {
        public override MutationLevel MutationLevel => MutationLevel.Standard;

        /// <inheritdoc />
        public override IEnumerable<Mutation> ApplyMutations(ExpressionStatementSyntax node)
        {
            if (node.Expression is InvocationExpressionSyntax invocationExpression
                && invocationExpression.Expression
                    .DescendantNodes()
                    .OfType<IdentifierNameSyntax>()
                    .Any(x=> x.Identifier.ValueText == nameof(string.IsNullOrEmpty)))
            {
                var invocationParameter = invocationExpression.ArgumentList.Arguments.First().Expression;
                var notEqualsToken = SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken);
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode =
                        SyntaxFactory.ParenthesizedExpression(
                            SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.NotEqualsExpression,
                                invocationParameter,
                                notEqualsToken,
                                SyntaxFactory.MemberAccessExpression(
                                    SyntaxKind.SimpleMemberAccessExpression,
                                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                    SyntaxFactory.Token(SyntaxKind.DotToken),
                                    SyntaxFactory.IdentifierName(nameof(string.Empty)))),
                            SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                    DisplayName = "String mutation",
                    Type = Mutator.String
                };
                yield return new Mutation
                {
                    OriginalNode = node,
                    ReplacementNode =
                        SyntaxFactory.ParenthesizedExpression(
                            SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                            SyntaxFactory.BinaryExpression(
                                SyntaxKind.NotEqualsExpression,
                                invocationParameter,
                                notEqualsToken,
                                SyntaxFactory.LiteralExpression(SyntaxKind.NullLiteralExpression,
                                    SyntaxFactory.Token(SyntaxKind.NullKeyword))),
                            SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                    DisplayName = "String mutation",
                    Type = Mutator.String
                };
            }
        }
    }
}
