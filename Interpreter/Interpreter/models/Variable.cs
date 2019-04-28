using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.models
{
    class Variable
    {
        public String acceso;
        public String tipo;
        public String nombre;
        public String valor;

        public Variable(String acceso, String tipo, String nombre, String valor) {
            this.acceso = acceso;
            this.tipo = tipo;
            this.nombre = nombre;
            this.valor = valor;
        }

        public string Acceso {
            get { return acceso; }
            set { acceso = value; }
        }

        public string Tipo
        {
            get { return tipo; }
            set { tipo = value; }
        }

        public string Nombre
        {
            get { return nombre; }
            set { nombre = value; }
        }

        public string Valor
        {
            get { return valor; }
            set { valor = value; }
        }
    }
}
