// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.Quantum.QsCompiler;
using Microsoft.Quantum.QsCompiler.Experimental;
using Microsoft.Quantum.QsCompiler.SyntaxTree;


namespace Microsoft.Quantum.Demos.CompilerExtensions.Demo
{
    public class CustomCompilerExtension : IRewriteStep
    {
        private readonly List<IRewriteStep.Diagnostic> Diagnostics;

        public CustomCompilerExtension()
        {
            this.AssemblyConstants = new Dictionary<string, string>();
            this.Diagnostics = new List<IRewriteStep.Diagnostic>();
        }


        public string Name => "CustomCompilerExtension";
        public int Priority => 10; 

        public IDictionary<string, string> AssemblyConstants { get; }
        public IEnumerable<IRewriteStep.Diagnostic> GeneratedDiagnostics => this.Diagnostics;

        public bool ImplementsPreconditionVerification => true;
        public bool ImplementsTransformation => true;
        public bool ImplementsPostconditionVerification => false;


        public bool PreconditionVerification(QsCompilation compilation)
        {
            var requiredNamespace = "Microsoft.Quantum";
            var containsRequired = compilation.Namespaces.Select(ns => ns.Name.Value).Contains(requiredNamespace);

            if (!containsRequired) this.Diagnostics.Add(new IRewriteStep.Diagnostic
            {
                Severity = DiagnosticSeverity.Warning,
                Message = $"Required namespace {requiredNamespace} does not exist.",
                Source = Path.GetFullPath(Path.Combine("..","CustomExtension","Optimize.cs")),
                Stage = IRewriteStep.Stage.PreconditionVerification
            });

            return true;
        }

        public bool Transformation(QsCompilation compilation, out QsCompilation transformed)
        {
            static OptimizingTransformation[] Script(ImmutableDictionary<QsQualifiedName, QsCallable> callables) => new OptimizingTransformation[]
            {
                new ConstantPropagation(callables),
                new VariableRemoval(),
                new StatementRemoval(true),
            };

            transformed = PreEvaluation.WithScript(Script, compilation);
            return true;
        }

        public bool PostconditionVerification(QsCompilation compilation) =>
            throw new NotImplementedException();
    }
}
