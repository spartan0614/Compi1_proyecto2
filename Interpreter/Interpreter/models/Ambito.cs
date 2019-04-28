using Irony.Parsing;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.models
{
    class Ambito
    {
        Dictionary<String, Variable> variables;

        public Ambito() {
            this.variables = new Dictionary<String, Variable>();
        }

        public void Insertar(String llave, Variable var) {
            this.variables.Add(llave, var);
        }

        public void MostrarVariables() {
            foreach (KeyValuePair<String, Variable> kvp in this.variables) {
                Console.WriteLine(kvp.Value.acceso + " - " + kvp.Value.tipo + " - " + kvp.Value.nombre + " - " + kvp.Value.valor);
            }
        }
        
    }
}
