using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interpreter.models
{
    class Variable
    {
        public int clase;
        public String acceso;
        public String tipo;
        public String nombre;
        public String dimensiones;
        public object sizes;
        public object valor;

        public Variable(int clase,  String acceso, String tipo, String nombre, String dimensiones, object sizes ,object valor) {
            this.clase = clase;
            this.acceso = acceso;
            this.tipo = tipo;
            this.nombre = nombre;
            this.dimensiones = dimensiones;
            this.sizes = sizes;
            this.valor = valor;
        }

        public int Clase {
            get { return clase; }
            set { clase = value; }
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

        public string Dimensiones
        {
            get { return dimensiones; }
            set { dimensiones = value; }
        }

        public object Sizes
        {
            get { return sizes; }
            set { sizes = value; }
        }

        public object Valor
        {
            get { return valor; }
            set { valor = value; }
        }
    }
}
