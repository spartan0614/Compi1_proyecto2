using Interpreter.analizador;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Interpreter
{
    public partial class Form1 : Form
    {
        RichTextBox editor;

        public Form1()
        {
            InitializeComponent();
        }

        public void CrearPestaniaTabControl(String titulo, String cadena) {
            //----------------- Nueva Pestaña(TabPage)  -------------
            TabPage nuevaPestania = new TabPage(titulo);
            //------------------ Creando RichTextBox ----------------
            editor = new RichTextBox();
            editor.Name = "texto";
            editor.Text = cadena;
            editor.Multiline = true;
            editor.SetBounds(0, 0, tabControl.Width - 10, tabControl.Height - 28);
            //-----------Agregando a la pestaña RichTextBox-----------
            nuevaPestania.Controls.Add(editor);
            //----------Agregando al TabControl un RichTextBox--------
            tabControl.TabPages.Add(nuevaPestania);
            tabControl.SelectedTab = nuevaPestania;
            tabControl.ResetText();
        }

        private void agregarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String nombreArchivo = "pestaña " + (tabControl.TabPages.Count + 1).ToString();
            CrearPestaniaTabControl(nombreArchivo, "");
        }

        private void btnCompilar_Click(object sender, EventArgs e)
        {
            if (tabControl.TabCount != 0)
            {
                String ruta = tabControl.SelectedTab.Text;
                String texto = tabControl.SelectedTab.Controls["texto"].Text;
                String busqueda = "Pestaña";
                StringComparison comparasion = StringComparison.InvariantCultureIgnoreCase;
                if (!ruta.StartsWith(busqueda, comparasion)) {
                    String path = Path.GetDirectoryName(ruta);
                    String archivo = Path.GetFileName(ruta);
                    Sintactico sintactico = new Sintactico();
                    Sintactico.Analizar(texto);
                }
            }
            //bool resultado = Sintactico.Analizar(txtEntrada.Text);
            //if (resultado) {
            //    txtConsola.Text = "La cadena es Correcta.";
            //}
            //else {
            //    txtConsola.Text = "La cadena es Incorrecta.";
            //}
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_file = new OpenFileDialog();
            open_file.Filter = "Text Files|*.txt";
            open_file.Title = "Seleccione un archivo";
            if (open_file.ShowDialog() == DialogResult.OK) {
                CrearPestaniaTabControl(open_file.FileName, File.ReadAllText(open_file.FileName));
            }
        }
    }
}
