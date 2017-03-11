namespace compiler
{
    public struct CompilerOptions
    {
        public bool CopyProp;
        public bool Cse;
        public bool DeadCode;
        public bool PruneCfg;
        public bool RegAlloc;
        public bool InstSched;
        public bool CodeGen;
        public bool GraphOutput;
        public string CfgFilename;
        public string DomFilename;
        public string InputFilename;
        public string OutputFilename;
    }
}