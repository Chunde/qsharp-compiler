namespace Microsoft.Quantum.Demo {
    open Microsoft.Quantum.Intrinsic;
    
    operation SampleProgram () : Int {

        let range = 1 .. 10;
        mutable tot = 0;
        for (value in range){
            set tot += value;
        }
        return tot;
    }
}
