// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Quantum.QsCompiler;
using Microsoft.Quantum.QsCompiler.SyntaxTree;


namespace Microsoft.Quantum.Demos.CompilerExtensions.Demo
{
    public class CustomCompilerExtension : IRewriteStep
    {
        public CustomCompilerExtension()
        {
            this.AssemblyConstants = new Dictionary<string, string>();
        }

        public string Name => 
            "CustomCompilerExtension";
        
        public int Priority => 
            10; // compared only against other rewrite steps implemented in this project

        public IDictionary<string, string> AssemblyConstants { get; }

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
            var display = new Display(transformed);
            display.Show();
            return true;
        }

        public bool PreconditionVerification(QsCompilation compilation) =>
            compilation.Namespaces.Select(ns => ns.Name.Value).Contains("Microsoft.Quantum.Demo");

        public bool PostconditionVerification(QsCompilation compilation)
        {
            var display = new Display(compilation);
            display.Show();
            return true;
        }
    }
}
