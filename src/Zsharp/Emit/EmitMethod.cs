using Zsharp.AST;
using Zsharp.Generation;

namespace Zsharp.Emit
{
    public class EmitMethod
    {
        private readonly EmitContext _context;

        public EmitMethod(EmitContext context)
        {
            _context = context;
        }

        public void EmitFunction(AstFunction function)
        {
            //var fn = new MethodDefinition(function.Identifier.Name, MethodAttributes.)
        }
    }
}
