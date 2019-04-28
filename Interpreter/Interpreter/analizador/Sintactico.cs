using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interpreter.grafica;
using Irony.Ast;
using Irony.Parsing;

namespace Interpreter.analizador
{
    class Sintactico : Grammar
    {
        public static bool Analizar(String entrada) {
            Gramatica gramatica = new Gramatica();
            LanguageData lenguaje = new LanguageData(gramatica);
            Parser parser = new Parser(lenguaje);
            ParseTree arbol = parser.Parse(entrada);
            ParseTreeNode raiz = arbol.Root;

            if (raiz == null) { //No sé generó un anális de analisis sintáctico
                return false;
            }

            //GenerarImagen(raiz);
            Recorrido.expresion(raiz);

            return true;
        }

        public static void GenerarImagen(ParseTreeNode raiz)
        {
            String grafoDOT = ControlDot.getDot(raiz);
            File.Create("ArbolGramatica.dot").Dispose();
            TextWriter tw = new StreamWriter("ArbolGramatica.dot");
            tw.WriteLine(grafoDOT);
            tw.Close();

            //ejecutar graphviz
            ProcessStartInfo startinfo = new ProcessStartInfo("C:\\Program Files (x86)\\Graphviz2.38\\bin\\dot.exe");
            Process process;
            startinfo.RedirectStandardOutput = true;
            startinfo.UseShellExecute = false;
            startinfo.Arguments = "-Tpng ArbolGramatica.dot -o ArbolGramatica.png";
            process = Process.Start(startinfo);
            process.Close();

        }

    }
}
