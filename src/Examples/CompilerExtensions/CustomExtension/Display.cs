// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.IO;
using System.Diagnostics;
using System.Linq;
using Microsoft.Quantum.QsCompiler.SyntaxTree;
using Microsoft.Quantum.QsCompiler.Transformations.BasicTransformations;
using Microsoft.Quantum.QsCompiler.Transformations.QsCodeOutput;
using System.Collections.Immutable;
using Microsoft.Quantum.QsCompiler.DataTypes;
using System.Collections.Generic;
using System;

namespace Microsoft.Quantum.Demos.CompilerExtensions.Demo
{
    internal class Display
    {
        private readonly QsCompilation Compilation;
        private ImmutableHashSet<NonNullable<string>> SourceFiles;
        private ImmutableDictionary<NonNullable<string>, ImmutableArray<(NonNullable<string>, string)>> Imports;

        internal Display(QsCompilation compilation)
        {
            this.Compilation = compilation;
            this.SourceFiles = GetSourceFiles.Apply(compilation.Namespaces);
            this.Imports = compilation.Namespaces.ToImmutableDictionary(ns => ns.Name, _ => ImmutableArray<(NonNullable<string>, string)>.Empty);
        }

        public void Show(string file = null)
        {
            var filesToShow = file == null
                ? this.SourceFiles.Where(f => f.Value.EndsWith(".qs")).OrderBy(f => f).Select(f => (f, this.Imports)).ToArray()
                : new[] { (NonNullable<string>.New(file), this.Imports) };

            SyntaxTreeToQs.Apply(out List<ImmutableDictionary<NonNullable<string>, string>> generated, this.Compilation.Namespaces, filesToShow.First());
            var code = generated.Single().Values.Select(nsCode => $"{nsCode}{Environment.NewLine}");

            var tempFile = Path.GetTempFileName();
            File.WriteAllLines(tempFile, code);
            Process.Start("notepad.exe", tempFile);
        }

    }

}