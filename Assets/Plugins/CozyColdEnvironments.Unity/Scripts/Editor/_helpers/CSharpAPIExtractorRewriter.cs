#if UNITY_EDITOR
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

#pragma warning disable RS1034
#nullable enable
namespace CCEnvs.UnityX
{
    public sealed class CSharpAPIExtractorRewriter : CSharpSyntaxRewriter
    {
        // ─── Методы ───────────────────────────────────────────────────────────────
        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            if (node.Body != null)
                node = node.WithBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            else if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitMethodDeclaration(node);
        }

        // ─── Конструкторы ─────────────────────────────────────────────────────────
        public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            if (node.Body != null)
                node = node.WithBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            else if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitConstructorDeclaration(node);
        }

        // ─── Деструкторы ──────────────────────────────────────────────────────────
        public override SyntaxNode? VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            if (node.Body != null)
                node = node.WithBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            else if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitDestructorDeclaration(node);
        }

        // ─── Операторы ────────────────────────────────────────────────────────────
        public override SyntaxNode? VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            if (node.Body != null)
                node = node.WithBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            else if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitOperatorDeclaration(node);
        }

        public override SyntaxNode? VisitConversionOperatorDeclaration(
            ConversionOperatorDeclarationSyntax node)
        {
            if (node.Body != null)
                node = node.WithBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            else if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitConversionOperatorDeclaration(node);
        }

        // ─── Свойства ─────────────────────────────────────────────────────────────
        public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            // Убираем тела аксессоров (get { ... } → get; )
            if (node.AccessorList != null)
            {
                var cleaned = node.AccessorList.Accessors.Select(CleanAccessor);
                node = node.WithAccessorList(
                    node.AccessorList.WithAccessors(SyntaxFactory.List(cleaned)));
            }

            // Убираем expression-body у свойств (=> value; → ;)
            if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            // Убираем инициализатор auto-property (public int X { get; } = 5; → public int X { get; };)
            if (node.Initializer != null)
                node = node.WithInitializer(null);

            return base.VisitPropertyDeclaration(node);
        }

        // ─── Индексаторы ──────────────────────────────────────────────────────────
        public override SyntaxNode? VisitIndexerDeclaration(IndexerDeclarationSyntax node)
        {
            if (node.AccessorList != null)
            {
                var cleaned = node.AccessorList.Accessors.Select(CleanAccessor);
                node = node.WithAccessorList(
                    node.AccessorList.WithAccessors(SyntaxFactory.List(cleaned)));
            }

            if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitIndexerDeclaration(node);
        }

        // ─── Поля и события-поля ──────────────────────────────────────────────────
        public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            var cleaned = node.Declaration.Variables.Select(v => v.WithInitializer(null));
            return node.WithDeclaration(
                node.Declaration.WithVariables(SyntaxFactory.SeparatedList(cleaned)));
        }

        public override SyntaxNode? VisitEventFieldDeclaration(EventFieldDeclarationSyntax node)
        {
            var cleaned = node.Declaration.Variables.Select(v => v.WithInitializer(null));
            return node.WithDeclaration(
                node.Declaration.WithVariables(SyntaxFactory.SeparatedList(cleaned)));
        }

        // ─── События с accessor list ──────────────────────────────────────────────
        public override SyntaxNode? VisitEventDeclaration(EventDeclarationSyntax node)
        {
            if (node.AccessorList != null)
            {
                var cleaned = node.AccessorList.Accessors.Select(CleanAccessor);
                node = node.WithAccessorList(
                    node.AccessorList.WithAccessors(SyntaxFactory.List(cleaned)));
            }
            return base.VisitEventDeclaration(node);
        }

        // ─── Локальные функции ────────────────────────────────────────────────────
        public override SyntaxNode? VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            if (node.Body != null)
                node = node.WithBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));
            else if (node.ExpressionBody != null)
                node = node.WithExpressionBody(null)
                           .WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return base.VisitLocalFunctionStatement(node);
        }

        // ─── Helpers ──────────────────────────────────────────────────────────────
        private static AccessorDeclarationSyntax CleanAccessor(AccessorDeclarationSyntax accessor)
        {
            accessor = accessor.WithBody(null).WithExpressionBody(null);

            // Если у аксессора не было точки с запятой (было тело), добавляем её
            if (accessor.SemicolonToken.Kind() == SyntaxKind.None)
                accessor = accessor.WithSemicolonToken(
                    SyntaxFactory.Token(SyntaxKind.SemicolonToken));

            return accessor;
        }
    }
}
#endif
