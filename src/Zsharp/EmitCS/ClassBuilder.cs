using System;
using Zsharp.AST;

namespace Zsharp.EmitCS
{
    public sealed class ClassBuilder : IDisposable
    {
        private readonly EmitContext _context;

        private ClassBuilder(EmitContext context)
        {
            _context = context;
        }

        internal CsBuilder CsBuilder => _context.CsBuilder;

        public static ClassBuilder Create(EmitContext context, AstModulePublic module)
        {
            var builder = new ClassBuilder(context);

            var access = module.HasExports ? AccessModifiers.Public : AccessModifiers.Internal;
            var modifiers = ClassModifiers.Static;
            builder.CsBuilder.StartClass(access, modifiers, module.Identifier!.Name);

            return builder;
        }

        public void AddField(AstVariableDefinition variable)
        {
            var access = AccessModifiers.Private;
            var modifiers = FieldModifiers.Static;
            CsBuilder.StartField(access, modifiers, variable.Identifier!.CanonicalName, variable.TypeReference.Identifier.CanonicalName);
            CsBuilder.EndLine();
        }

        public void Dispose()
        {
            CsBuilder.EndScope();
        }
    }
}
