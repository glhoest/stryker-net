using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Shouldly;
using Stryker.Core.Mutators;
using System.Linq;
using Microsoft.CodeAnalysis;
using Xunit;

namespace Stryker.Core.UnitTest.Mutators
{
    public class StringNullOrEmptyMutatorTests : TestBase
    {
        [Fact]
        public void ShouldBeMutationLevelStandard()
        {
            var target = new StringNullOrEmptyMutator();
            target.MutationLevel.ShouldBe(MutationLevel.Standard);
        }

        [Fact]
        public void ShouldMutate_AndReturnTwoMutations()
        {
            var node = SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                    SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(nameof(string.IsNullOrEmpty))),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(new[]
                    {
                        SyntaxFactory.Argument(
                            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("test")))
                    }))));

            var mutator = new StringNullOrEmptyMutator();

            var result = mutator.ApplyMutations(node).ToList();

            result.Count.ShouldBe(2);
        }

        [Fact]
        public void ShouldMutate_AndReturn_EmptyStringCheckMutation()
        {
            var node = SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                    SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(nameof(string.IsNullOrEmpty))),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(new[]
                    {
                        SyntaxFactory.Argument(
                            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("test")))
                    }))));

            var mutator = new StringNullOrEmptyMutator();

            var result = mutator.ApplyMutations(node).ToList();

            result.Count.ShouldBe(2);

            var correctMutation = result.Single(mutation =>
            {
                return mutation
                    .ReplacementNode
                    .DescendantNodes()
                    .OfType<MemberAccessExpressionSyntax>()
                    .Any(syntax =>
                        syntax
                            .DescendantNodes()
                            .OfType<IdentifierNameSyntax>()
                            .First()
                            .Identifier
                            .Text == nameof(string.Empty));
            });

            correctMutation.ShouldNotBeNull();
        }

        [Fact]
        public void ShouldMutate_AndReturn_NullCheckMutation()
        {
            var node = SyntaxFactory.ExpressionStatement(SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(
                    SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.PredefinedType(SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                    SyntaxFactory.Token(SyntaxKind.DotToken),
                    SyntaxFactory.IdentifierName(nameof(string.IsNullOrEmpty))),
                SyntaxFactory.ArgumentList(
                    SyntaxFactory.SeparatedList(new[]
                    {
                        SyntaxFactory.Argument(
                            SyntaxFactory.LiteralExpression(SyntaxKind.StringLiteralExpression,
                                SyntaxFactory.Literal("test")))
                    }))));

            var mutator = new StringNullOrEmptyMutator();

            var result = mutator.ApplyMutations(node).ToList();

            result.Count.ShouldBe(2);

            var correctMutation = result.Single(mutation =>
            {
                return mutation
                    .ReplacementNode
                    .DescendantNodes()
                    .OfType<LiteralExpressionSyntax>()
                    .Any(syntax => syntax.Kind() == SyntaxKind.NullLiteralExpression);
            });

            correctMutation.ShouldNotBeNull();
        }
    }
}
