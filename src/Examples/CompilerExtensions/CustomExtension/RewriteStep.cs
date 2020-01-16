// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.Quantum.QsCompiler;
using Microsoft.Quantum.QsCompiler.SyntaxTree;


namespace Microsoft.Quantum.Demos.CompilerExtensions.Demo
{
    public class CustomCompilerExtension : IRewriteStep
    {
        private List<IRewriteStep.Diagnostic> Diagnostics;

        public CustomCompilerExtension()
        {
            this.AssemblyConstants = new Dictionary<string, string>();
            this.Diagnostics = new List<IRewriteStep.Diagnostic>();
        }

        public string Name =>
            "CustomCompilerExtension";
        
        public int Priority => 
            10; // compared only against other rewrite steps implemented in this project

        public IDictionary<string, string> AssemblyConstants { get; }
        public IEnumerable<IRewriteStep.Diagnostic> GeneratedDiagnostics => this.Diagnostics;

        public bool ImplementsTransformation => true;
        public bool ImplementsPreconditionVerification => true;
        public bool ImplementsPostconditionVerification => true;

        public bool Transformation(QsCompilation compilation, out QsCompilation transformed)
        {
            //var script = new[]
            //{
            //    new VariableRemoval(),
            //    new StatementRemoval(removeFunctions),
            //    new ConstantPropagation(callables),
            //    new LoopUnrolling(callables, maxSize),
            //    new CallableInlining(callables),
            //    new StatementGrouping(),
            //    new PureCircuitFinder(callables),
            //};
            //Microsoft.Quantum.QsCompiler.Optimizations.PreEvaluation.

            transformed = compilation; // todo
            return true;
        }

        public bool PreconditionVerification(QsCompilation compilation)
        {
            var requiredNamespace = "Microsoft.Quantum";
            var execute = compilation.Namespaces.Select(ns => ns.Name.Value).Contains(requiredNamespace);
            if (!execute) this.Diagnostics.Add(new IRewriteStep.Diagnostic
            {
                Severity = DiagnosticSeverity.Warning,
                Message = $"Required namespace {requiredNamespace} does not exist.",
                Source = Assembly.GetExecutingAssembly().Location,
                Stage = IRewriteStep.Stage.PreconditionVerification
            });
            return true;
        }

        public bool PostconditionVerification(QsCompilation compilation)
        {

            var display = new Display(compilation);
            //display.Show();
            return true;
        }
    }
}
