using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace compiler.frontend
{
    public Token t;
    public Lexer s;

    class Parser
    {
        public void next() {
            t = s.getNextToken();
        }

        public void Designator(){
            if (t == Token.IDENTIFIER) {
                next();
            }
            else error();
        }
        public void Factor(){
            
        }
        public void Term(){
            
        }
        public void Expression(){
            
        }
        public void Relation(){
            
        }

    }
}
