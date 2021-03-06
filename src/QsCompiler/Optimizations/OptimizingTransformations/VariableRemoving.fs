// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.Quantum.QsCompiler.Experimental

open System.Collections.Immutable
open Microsoft.Quantum.QsCompiler.Experimental.OptimizationTools
open Microsoft.Quantum.QsCompiler.Experimental.Utils
open Microsoft.Quantum.QsCompiler.SyntaxTree
open Microsoft.Quantum.QsCompiler.Transformations.Core


/// The SyntaxTreeTransformation used to remove useless statements
type VariableRemoval() =
    inherit OptimizingTransformation()

    let mutable referenceCounter = None

    override __.onProvidedImplementation (argTuple, body) =
        let r = ReferenceCounter()
        r.Transform body |> ignore
        referenceCounter <- Some r
        base.onProvidedImplementation (argTuple, body)

    override __.Scope = { new ScopeTransformation() with
        override this.StatementKind = { new StatementKindTransformation() with
            override __.ExpressionTransformation x = x
            override __.LocationTransformation x = x
            override __.ScopeTransformation x = this.Transform x
            override __.TypeTransformation x = x

            override stmtKind.onSymbolTuple syms =
                match syms with
                | VariableName item ->
                    maybe {
                        let! r = referenceCounter
                        let uses = r.getNumUses item
                        do! check (uses = 0)
                        return DiscardedItem
                    } |? syms
                | VariableNameTuple items -> Seq.map stmtKind.onSymbolTuple items |> ImmutableArray.CreateRange |> VariableNameTuple
                | InvalidItem | DiscardedItem -> syms
        }
    }
