using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Maja.Compiler.Diagnostics;
using Maja.Compiler.Symbol;
using Maja.Compiler.Syntax;

namespace Maja.Compiler.IR;

/// <summary>
/// Resolves all UnresolvedXxxxSymbol instances after IrBuilder is done.
/// UnResolved symbols are inserted when the declaration has not been seen yet during IrBuilder.
/// This is post-processing to fixup those loose ends.
/// </summary>
internal sealed class IrResolveSymbolsRewriter : IrRewriter
{
    // TODO: Add code validation logic? Or a separate walker?

    private readonly Stack<IrScope> _scopes = new();
    public IrResolveSymbolsRewriter(IrScope parentScope)
    {
        _scopes.Push(parentScope);
    }
    private IrScope CurrentScope
        => _scopes.Peek();

    public IrModule FixUnresolvedSymbols(IrModule module)
        => RewriteModule(module);

    // fixup function invocation of a forward declared function
    protected override IrExpression RewriteExpressionInvocation(IrExpressionInvocation expression)
    {
        if (expression.Symbol.IsUnresolved)
        {
            if (expression.Symbol is UnresolvedDeclaredFunctionSymbol)
            {
                var argumentTypes = expression.Arguments.Select(a => a.Expression.TypeSymbol);
                if (!CurrentScope.TryLookupFunctionSymbol(expression.Symbol.Name, argumentTypes, out var functionSymbol))
                {
                    Diagnostics.FunctionNotFound(expression.Syntax.Location, expression.Symbol.Name.FullOriginalName);
                    return expression;
                }

                var typeArguments = RewriteTypeArguments(expression.TypeArguments);
                var arguments = RewriteArguments(expression.Arguments);
                var typeSymbol = ResolveTypeSymbol(expression.TypeSymbol, functionSymbol.ReturnType, expression.Syntax.Location);
                return new IrExpressionInvocation(expression.Syntax, functionSymbol, typeArguments, arguments, typeSymbol);
            }

            return base.RewriteExpressionInvocation(expression);
        }

        // nothing to process, shortcut hierarchy (don't call base impl)
        return expression;
    }

    // fixup unresolved types
    [return: NotNullIfNotNull(nameof(type))]
    protected override IEnumerable<IrType> RewriteType(IrType? type)
    {
        if (type is not null)
        {
            // these have to be fixed higher up the hierarchy with more context info
            Debug.Assert(type.Symbol != TypeSymbol.Unknown);

            if (type.Symbol.IsUnresolved)
            {
                var typeSymbol = ResolveTypeSymbol(type.Symbol, null, type.Syntax.Location);
                return [new IrType(type.Syntax, typeSymbol)];
            }
        }
        return base.RewriteType(type);
    }

    protected override IrExpression RewriteExpressionTypeInitializer(IrExpressionTypeInitializer initializer)
    {
        if (initializer.TypeSymbol.IsUnresolved)
        {
            var typeSymbol = ResolveTypeSymbol((DeclaredTypeSymbol)initializer.TypeSymbol, initializer.Syntax.Location);

            // TODO: process type-arg type references
            var typeArguments = RewriteTypeArguments(initializer.TypeArguments);

            var fields = initializer.Fields.Select(f =>
            {
                if (f.Field.IsUnresolved)
                {
                    if (!CurrentScope.TryLookupMemberType(typeSymbol, f.Field.Name, out var fieldType))
                    {
                        Diagnostics.FieldNotFoundOnType(f.Syntax.Location, typeSymbol.Name.FullOriginalName, f.Field.Name.FullOriginalName);
                        fieldType = TypeSymbol.Unresolved;
                    }

                    var fieldSymbol = new FieldSymbol(f.Field.Name, fieldType);
                    var expr = RewriteExpression(f.Expression);
                    return new IrTypeInitializerField(f.Syntax, fieldSymbol, expr);
                }

                return f;
            });

            return new IrExpressionTypeInitializer(initializer.Syntax, typeSymbol, typeArguments, fields);
        }
        return base.RewriteExpressionTypeInitializer(initializer);
    }

    private DeclaredTypeSymbol ResolveTypeSymbol(DeclaredTypeSymbol typeSymbol, SyntaxLocation location)
    {
        if (typeSymbol.TryIsUnresolved(out UnresolvedDeclaredTypeSymbol? unresolvedSymbol))
        {
            if (!CurrentScope.TryLookupSymbol<DeclaredTypeSymbol>(unresolvedSymbol.Name, out var resolvedSymbol))
                Diagnostics.TypeNotFound(location, unresolvedSymbol.Name.FullOriginalName);
            else
                typeSymbol = resolvedSymbol;
        }
        return typeSymbol;
    }

    private TypeSymbol ResolveTypeSymbol(TypeSymbol typeSymbol, TypeSymbol? defaultTypeSymbol, SyntaxLocation location)
    {
        if (typeSymbol.TryIsUnresolved(out UnresolvedTypeSymbol? unresolvedSymbol))
        {
            // CS8600: is not a problem
            if (!CurrentScope.TryLookupSymbol<TypeSymbol>(unresolvedSymbol.Name, out typeSymbol))
            {
                Diagnostics.TypeNotFound(location, unresolvedSymbol.Name.FullOriginalName);
                typeSymbol = TypeSymbol.Unresolved;
            }
        }
        else if (typeSymbol.IsUnresolved && defaultTypeSymbol is not null)
        {
            typeSymbol = defaultTypeSymbol;
        }

        if (typeSymbol.IsUnresolved)
        {
            Diagnostics.TypeNotFound(location, typeSymbol.Name.OriginalName);
        }

        return typeSymbol;
    }

    //-------------------------------------------------------------------------
    // keeping track of scopes
    protected override IrModule RewriteModule(IrModule module)
    {
        // module scope
        _scopes.Push(module.Scope);
        var newModule = base.RewriteModule(module);
        _ = _scopes.Pop();
        return newModule;
    }
    protected override IEnumerable<IrDeclarationFunction> RewriteDeclarationFunction(IrDeclarationFunction function)
    {
        // function scope
        _scopes.Push(function.Scope);
        var decls = base.RewriteDeclarationFunction(function);
        _ = _scopes.Pop();
        return decls;
    }
    protected override IEnumerable<IrDeclarationType> RewriteDeclarationType(IrDeclarationType type)
    {
        // type scope
        _scopes.Push(type.Scope);
        var decls = base.RewriteDeclarationType(type);
        _ = _scopes.Pop();
        return decls;
    }
}
