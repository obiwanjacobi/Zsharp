using System.Linq;
using Zsharp.AST;

namespace Zsharp.Semantics
{
    public class ResolveOverloads : AstVisitorWithSymbols
    {
        private readonly AstErrorSite _errorSite;

        public ResolveOverloads(AstErrorSite errorSite)
        {
            _errorSite = errorSite;
        }

        public override void VisitFunctionReference(AstFunctionReference function)
        {
            if (function.Symbol!.FindOverloadDefinition(function) != null)
                return;

            var overloadDef = ResolveOverload(function);
            if (overloadDef == null)
            {
                _errorSite.OverloadNotFound(function);
                return;
            }

            function.Symbol.SetOverload(function, overloadDef);
        }

        private AstFunctionDefinition? ResolveOverload(AstFunctionReference function)
        {
            // TODO: more elaborate overload resolution here...
            return function.Symbol!.Overloads.SingleOrDefault(def => def.OverloadKey == function.OverloadKey);
        }
    }
}
