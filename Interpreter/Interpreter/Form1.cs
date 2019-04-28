using Interpreter.analizador;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            bool resultado = Sintactico.Analizar(txtEntrada.Text);
            if (resultado) {
                txtConsola.Text = "La cadena es Correcta.";
            }
            else {
                txtConsola.Text = "La cadena es Incorrecta.";
            }

        }
    }
}
