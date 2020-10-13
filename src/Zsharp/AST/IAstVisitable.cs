namespace Zsharp.AST
{
    public interface IAstVisitable
    {
        void Accept(AstVisitor visitor);
        void VisitChildren(AstVisitor visitor);
    }
}