using Mono.Cecil;

namespace Zsharp.AST
{
    public class AstModuleExternal : AstModule
    {
        private readonly AssemblyDefinition _assemblyDef;

        public AstModuleExternal(AssemblyDefinition assemblyDef)
            : base(assemblyDef.Name.Name, AstModuleLocality.External)
        {
            _assemblyDef = assemblyDef;
        }

        /// <summary>
        /// Unit Tests only!
        /// </summary>
        public AstModuleExternal(string moduleName)
            : base(moduleName, AstModuleLocality.External)
        { }

        public override void Accept(AstVisitor visitor)
        {
            // no-op
        }
    }
}
