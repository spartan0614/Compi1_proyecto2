using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;


namespace Interpreter.models
{
    class Condicion
    {
        public ParseTreeNode restriccion;
        public ParseTreeNode cuerpo_condicion;

        public Condicion(ParseTreeNode restriccion, ParseTreeNode cuerpo_condicion) {
            this.restriccion = restriccion;
            this.cuerpo_condicion = cuerpo_condicion;
        }

        public ParseTreeNode Restriccion
        {
            get { return restriccion; }
            set { restriccion = value; }
        }

        public ParseTreeNode Cuerpo_condicion
        {
            get { return cuerpo_condicion; }
            set { cuerpo_condicion = value; }
        }

    }
}
