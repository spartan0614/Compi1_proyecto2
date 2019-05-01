using Interpreter.models;
using Irony.Parsing;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//System.Collections.Generic.Stack<T>

namespace Interpreter.analizador
{
    class Recorrido
    {

        //Lista que almacenará todas las variables globales de todas las clases que trae el archivo
        List<Ambito> entornos_globales = new List<Ambito>();

        public static Variable getValorVariable(String nombre, Stack<Ambito> entornos) {
            foreach (Ambito ambito in entornos) {
                Variable var = ambito.Buscar(nombre);
                if (var != null) {
                    return var;
                }
            }
            return null;
        }

        public static void MostrarVariables(Stack<Ambito> entornos) {
            foreach (Ambito ambito in entornos) {
                Console.WriteLine("-----------------------------------");
                ambito.MostrarVariables();
            }
        }


    public static String expresion(ParseTreeNode root) {
            switch ((String)root.Term.Name) {
                case "S":
                    //Console.WriteLine("Entro a S");
                    expresion(root.ChildNodes[0]);   //LISTA_CLASES
                    break;
                case "LISTA_CLASES":
                    //Console.WriteLine("Entro a LISTA_CLASES");
                    if (root.ChildNodes.Count == 2) {
                        //Console.WriteLine("LISTA_CLASES");
                        expresion(root.ChildNodes[0]);    //LISTA_CLASES
                        //Console.WriteLine("CLASE");
                        expresion(root.ChildNodes[1]);    //CLASE

                    } else if (root.ChildNodes.Count == 1) {
                        //Console.WriteLine("CLASE");
                        expresion(root.ChildNodes[0]);    //CLASE
                    }
                    break;
                case "CLASE":
                    //Console.WriteLine("Entro a CLASE");
                    CLASE(root);
                    
                    break;
            }
            return "";
        }

        public static void CLASE(ParseTreeNode root) {
            //Stack entornos = new Stack();
            Stack<Ambito> entornos = new Stack<Ambito>();

            Ambito global = new Ambito();
            entornos.Push(global);
       
            if (root.ChildNodes.Count == 4)
            {
                //Console.WriteLine("CLASE trae imports");
                IMPORTAR(root.ChildNodes[2]);                       //IMPORTAR
                LISTA_CUERPO(root.ChildNodes[3], entornos);         //LISTA_CUERPO

                
            }
            else if(root.ChildNodes.Count == 3)
            {
                //Console.WriteLine("CLASE sin imports");
                LISTA_CUERPO(root.ChildNodes[2], entornos);         //LISTA_CUERPO
            }


            MostrarVariables(entornos);
        }

        public static void IMPORTAR(ParseTreeNode root) {
            String listado = "";
            listado += LISTA_ID(root.ChildNodes[1], listado);
            //Console.WriteLine(listado);
        }

        public static string LISTA_ID(ParseTreeNode root, String listado) {
            if (root.ChildNodes.Count == 3)
            {
                listado += LISTA_ID(root.ChildNodes[0], listado);
                listado += ",";
                listado += root.ChildNodes[2].Token.Value.ToString();
                
            }
            else if (root.ChildNodes.Count == 1) {
                listado += root.ChildNodes[0].Token.Value.ToString();
            }
            return listado;
        }

        public static void LISTA_CUERPO(ParseTreeNode root, Stack<Ambito> entornos) {
           

            if (root.ChildNodes.Count == 2) {
                
                LISTA_CUERPO(root.ChildNodes[0], entornos);

                
                CUERPO(root.ChildNodes[1], entornos);

            }
            else if (root.ChildNodes.Count == 1) {
                
                CUERPO(root.ChildNodes[0], entornos);
            }
            
        }

        public static void CUERPO(ParseTreeNode root, Stack<Ambito> entornos) {
            switch ((String)root.ChildNodes[0].Term.Name)
            {
                case "FUNCION":
                    FUNCION(root.ChildNodes[0], entornos);
                    break;
                case "MAIN":

                    break;
                case "DECLARACION":
                    
                    DECLARACION(root.ChildNodes[0], entornos);
                    
                    break;
                
            }
            
        }

        private static void DECLARACION(ParseTreeNode root, Stack<Ambito> entornos) {
            switch (root.ChildNodes.Count)
            {
                case 2:

                    break;
                case 3:

                    break;
                case 4:
                    if (root.ChildNodes[1].Token == null)  //No es rama
                    {  
                        if (root.ChildNodes[0].Token == null)
                        {
                            String listado = "";
                            listado += LISTA_ID(root.ChildNodes[1], listado);
                            string[] identificadores = listado.Split(',');
                            for (int i = 0; i < identificadores.Count(); i++)
                            {
                                Variable v = new Variable("publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), identificadores[i], EXP(root.ChildNodes[3], entornos).ToString() );
                                entornos.Peek().Insertar(identificadores[i], v);
                            }
                        }
                        else
                        {
                            //declaracion objeto
                        }
                    }
                    else {
                        //array
                    }
                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
            }
        }

        private static void FUNCION(ParseTreeNode root, Stack<Ambito> entornos) {
            Ambito funcion = new Ambito();
            entornos.Push(funcion);
            L_SENTENCIAS(root.ChildNodes[1], entornos);
            entornos.Pop();
        }

        private static object L_SENTENCIAS(ParseTreeNode root, Stack<Ambito> entornos) {
            foreach (ParseTreeNode SENTENCIA in root.ChildNodes) {
                switch (SENTENCIA.ChildNodes.Count)
                {
                    case 1:
                        if (SENTENCIA.ChildNodes[0].Token == null)
                        {  //No es rama
                            switch ((String)SENTENCIA.ChildNodes[0].Term.Name)
                            {
                                case "DECLARACION":
                                    DECLARACION(SENTENCIA.ChildNodes[0], entornos);
                                    break;
                                case "EXP":
                                    EXP(SENTENCIA.ChildNodes[0], entornos);
                                    break;
                                case "IF":

                                    break;
                                case "FOR":
                                    ParseTreeNode raiz_for = SENTENCIA.ChildNodes[0];
                                    Ambito for1 = new Ambito();
                                    if (raiz_for.ChildNodes[1].ChildNodes[0].Term.Name.ToString() == "DECLARACION") {
                                        Object valor_inicial = INIT_DECLARACION_FOR(raiz_for.ChildNodes[1].ChildNodes[0], entornos);
                                    } else if (raiz_for.ChildNodes[1].ChildNodes[0].Term.Name.ToString() == "EXP") {

                                    }
                                    break;
                                case "REPEAT":
                                    ParseTreeNode raiz_repeat = SENTENCIA.ChildNodes[0];
                                    Ambito repeat1 = new Ambito();
                                    object e = EXP(raiz_repeat.ChildNodes[1], entornos);
                                    Int32 valor;
                                    switch (e)
                                    {
                                        case int _:
                                            valor = (Int32)e;
                                            break;
                                        default:
                                            return "Valor no valido";
                                    }

                                    for (int i = 0; i < valor; i++) {
                                        entornos.Push(repeat1);
                                        var res = L_SENTENCIAS(raiz_repeat.ChildNodes[2], entornos);
                                        entornos.Pop();

                                        if (!res.Equals("@FINALLY@"))
                                        {
                                            if (res.ToString().Equals("@SALIR@"))
                                            {
                                                res = "@FINALLY@";
                                                break;
                                            }

                                            return res;
                                        }
                                    }

                                    break;
                                case "WHILE":
                                    ParseTreeNode raiz_while = SENTENCIA.ChildNodes[0];
                                    Ambito while1 = new Ambito();
                                    while ((bool)EXP(raiz_while.ChildNodes[1], entornos))
                                    {
                                        entornos.Push(while1);
                                        var res = L_SENTENCIAS(raiz_while.ChildNodes[2], entornos);
                                        entornos.Pop();

                                        if (!res.Equals("@FINALLY@"))
                                        {
                                            if (res.ToString().Equals("@SALIR@"))
                                            {
                                                res = "@FINALLY@";
                                                break;
                                            }

                                            if (res.ToString().Equals("@CONTINUAR@"))
                                            {
                                                continue;
                                            }

                                            return res;
                                        }
                                    }
                                   
                                    break;
                                case "HACER":

                                    break;
                                case "COMPROBAR":

                                    break;
                            }
                        }
                        else
                        {
                            switch (SENTENCIA.ChildNodes[0].Token.Value.ToString())
                            {
                                case "continuar":
                                    return "@CONTINUAR@";
                                case "return":
                                    return "@RETORNAR@";
                                case "salir":
                                    return "@SALIR@";
                            }
                        }
                        break;
                    case 2:
                        if (SENTENCIA.ChildNodes[0].Token.Value.ToString() == "print")
                        {
                            Console.WriteLine(EXP(SENTENCIA.ChildNodes[1], entornos).ToString());
                        }
                        else if (SENTENCIA.ChildNodes[0].Token.Value.ToString() == "return")
                        {

                        }
                        else if (SENTENCIA.ChildNodes[0].Token.Value.ToString() == "addfigure")
                        {

                        }
                        break;
                }
            }
            return "@FINALLY@";
        }

        private static object INIT_DECLARACION_FOR(ParseTreeNode root, Stack<Ambito> entornos) {
            if (root.ChildNodes.Count == 4)
            {
                if (root.ChildNodes[0].Token == null)  //No es rama
                {
                    if (root.ChildNodes[1].ToString().Contains(" (id)"))
                    {
                        String nombre = root.ChildNodes[0].Token.Value.ToString();

                        Variable v = new Variable("publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), nombre, EXP(root.ChildNodes[3], entornos).ToString());
                        entornos.Peek().Insertar(nombre, v);
                        return EXP(root.ChildNodes[3], entornos);
                    }
                    else {
                        return "error";
                    }
                }
                else
                {
                    return "error";
                }
            }
            return "error";
        }

        private static object EXP(ParseTreeNode root, Stack<Ambito> entornos) {
            switch (root.ChildNodes.Count) {
                case 3:
                    if (root.ChildNodes[0].Token != null)  //   id + punto + EXP  |   id + parAbre + parCierra   |   
                    {

                    }
                    else if ((root.ChildNodes[1].Token.Value.ToString() == "+") && (root.ChildNodes[2].Token != null))
                    {
                        if ((root.ChildNodes[2].Token.Value.ToString() == "+"))
                        {
                            if (root.ChildNodes[0].ChildNodes.Count == 1) {            //EXP++   --> id++  ó  int++  ó   double++  ó   char++
                                if (root.ChildNodes[0].ChildNodes[0].ToString().Contains(" (id)"))
                                {
                                    Variable var = getValorVariable(root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), entornos);
                                    switch (var.tipo)
                                    {
                                        case "int":
                                            var.valor = (Convert.ToInt32(var.valor) + 1).ToString();
                                            return Convert.ToInt32(var.valor);
                                            //break;
                                        case "double":
                                            var.valor = (Convert.ToDouble(var.valor) + 1).ToString();
                                            return Convert.ToDouble(var.valor);
                                            //break;
                                        case "char":
                                            var.valor = (Convert.ToInt32(Convert.ToChar(var.valor)) + 1).ToString();
                                            return Convert.ToInt32(Convert.ToChar(var.valor));
                                            //break;
                                    }
                                }
                                else
                                {
                                    Object e = EXP(root.ChildNodes[0], entornos);
                                    switch (e)
                                    {
                                        case int _:
                                            return (Int32)e + 1;
                                        case double _:
                                            return (Double)e + 1;
                                        case char _:
                                            return ((Int32)((Char)e)) + 1;
                                        default:
                                            return "Valores incompatibles";
                                    }
                                }
                            } else if (root.ChildNodes[0].ChildNodes.Count == 3) {     // EXP++  --> id.EXP++

                            }
                        }
                    }
                    else if ((root.ChildNodes[1].Token.Value.ToString() == "-") && (root.ChildNodes[2].Token != null))
                    {
                        if ((root.ChildNodes[2].Token.Value.ToString() == "-"))
                        {
                            if (root.ChildNodes[0].ChildNodes.Count == 1)
                            {            //EXP--   --> id--  ó  int--  ó   double--  ó   char--
                                if (root.ChildNodes[0].ChildNodes[0].ToString().Contains(" (id)"))
                                {
                                    Variable var = getValorVariable(root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), entornos);
                                    switch (var.tipo)
                                    {
                                        case "int":
                                            var.valor = (Convert.ToInt32(var.valor) - 1).ToString();
                                            return Convert.ToInt32(var.valor);
                                        //break;
                                        case "double":
                                            var.valor = (Convert.ToDouble(var.valor) - 1).ToString();
                                            return Convert.ToDouble(var.valor);
                                        //break;
                                        case "char":
                                            var.valor = (Convert.ToInt32(Convert.ToChar(var.valor)) - 1).ToString();
                                            return Convert.ToInt32(Convert.ToChar(var.valor));
                                            //break;
                                    }
                                }
                                else
                                {
                                    Object e = EXP(root.ChildNodes[0], entornos);
                                    switch (e)
                                    {
                                        case int _:
                                            return (Int32)e - 1;
                                        case double _:
                                            return (Double)e - 1;
                                        case char _:
                                            return ((Int32)((Char)e)) - 1;
                                        default:
                                            return "Valores incompatibles";
                                    }
                                }
                            }
                            else if (root.ChildNodes[0].ChildNodes.Count == 3)
                            {     // EXP--  --> id.EXP--

                            }
                        }
                    }
                    else
                    {
                        Object e1 = EXP(root.ChildNodes[0], entornos);
                        Object e2 = EXP(root.ChildNodes[2], entornos);
                        String signo = root.ChildNodes[1].Token.Value.ToString();
                        switch (signo)
                        {
                            case "=":
                                break;
                            case "||":
                                if (e1 is bool a && e2 is bool b)
                                {
                                    return (a || b);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case "&&":
                                if (e1 is bool && e2 is bool)
                                {
                                    return ((bool)e1 && (bool)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case ">":
                                if (e1 is Double && e2 is Double)
                                {
                                    return ((Double)e1 > (Double)e2);
                                }
                                else if (e1 is char && e2 is char)
                                {
                                    return ((char)e1) > ((char)e2);
                                }
                                else if (e1 is string && e2 is string)
                                {
                                    return string.CompareOrdinal((string)e1, (string)e2) > 0;
                                }
                                else if (e1 is Int32 && e2 is Int32)
                                {
                                    return ((Int32)e1 > (Int32)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case "<":
                                if (e1 is Double && e2 is Double)
                                {
                                    return ((Double)e1 < (Double)e2);
                                }
                                else if (e1 is char && e2 is char)
                                {
                                    return ((char)e1) < ((char)e2);
                                }
                                else if (e1 is string && e2 is string)
                                {
                                    return string.CompareOrdinal((string)e1, (string)e2) < 0;
                                }
                                else if (e1 is Int32 && e2 is Int32)
                                {
                                    return ((Int32)e1 < (Int32)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case ">=":
                                if (e1 is Double && e2 is Double)
                                {
                                    return ((Double)e1 >= (Double)e2);
                                }
                                else if (e1 is char && e2 is char)
                                {
                                    return ((char)e1) >= ((char)e2);
                                }
                                else if (e1 is string && e2 is string)
                                {
                                    return string.CompareOrdinal((string)e1, (string)e2) >= 0;
                                }
                                else if (e1 is Int32 && e2 is Int32)
                                {
                                    return ((Int32)e1 >= (Int32)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case "<=":
                                if (e1 is Double && e2 is Double)
                                {
                                    return ((Double)e1 <= (Double)e2);
                                }
                                else if (e1 is char && e2 is char)
                                {
                                    return ((char)e1) <= ((char)e2);
                                }
                                else if (e1 is string && e2 is string)
                                {
                                    return string.CompareOrdinal((string)e1, (string)e2) <= 0;
                                }
                                else if (e1 is Int32 && e2 is Int32)
                                {
                                    return ((Int32)e1 <= (Int32)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case "==":
                                if (e1 is Double && e2 is Double)
                                {
                                    return ((Double)e1 == (Double)e2);
                                }
                                else if (e1 is char && e2 is char)
                                {
                                    return ((char)e1) == ((char)e2);
                                }
                                else if (e1 is string && e2 is string)
                                {
                                    return String.CompareOrdinal((string)e1, (string)e2) == 0;
                                }
                                else if (e1 is Int32 && e2 is Int32)
                                {
                                    return ((Int32)e1 == (Int32)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case "!=":
                                if (e1 is Double && e2 is Double)
                                {
                                    return ((Double)e1 != (Double)e2);
                                }
                                else if (e1 is char && e2 is char)
                                {
                                    return ((char)e1) != ((char)e2);
                                }
                                else if (e1 is string && e2 is string)
                                {
                                    return String.CompareOrdinal((string)e1, (string)e2) != 0;
                                }
                                else if (e1 is Int32 && e2 is Int32)
                                {
                                    return ((Int32)e1 != (Int32)e2);
                                }
                                else
                                {
                                    //Error: valores incompatibles
                                }
                                break;
                            case "+":
                                switch (e1)
                                {
                                    case string _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (String)e1 + (Int32)e2;
                                            case string _:
                                                return (String)e1 + (String)e2;
                                            case double _:
                                                return (String)e1 + (Double)e2;
                                            case char _:
                                                return (String)e1 + (char)e2;
                                            case bool _:
                                                return "Valores incompatibles";

                                        }
                                        break;
                                    case int _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Int32)e1 + (Int32)e2;
                                            case string _:
                                                return (Int32)e1 + (String)e2;
                                            case double _:
                                                return (Int32)e1 + (Double)e2;
                                            case char _:
                                                return (Int32)e1 + ((Int32)((Char)e2));
                                            case bool _:
                                                return (Int32)e1 + ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case double _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Double)e1 + (Int32)e2;
                                            case string _:
                                                return (Double)e1 + (String)e2;
                                            case double _:
                                                return (Double)e1 + (Double)e2;
                                            case char _:
                                                return (Double)e1 + ((Int32)((Char)e2));
                                            case bool _:
                                                return (double)e1 + ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case char _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((Int32)((Char)e1)) + (Int32)e2;
                                            case string _:
                                                return ((Int32)((Char)e1)) + (String)e2;
                                            case double _:
                                                return ((Int32)((Char)e1)) + (Double)e2;
                                            case char _:
                                                return ((Int32)((Char)e1)) + ((Int32)((Char)e2));
                                            case bool _:
                                                return ((Int32)((Char)e1)) + ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case bool _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((bool)e1 ? 1 : 0) + (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return ((bool)e1 ? 1 : 0) + (Double)e2;
                                            case char _:
                                                return ((bool)e1 ? 1 : 0) + ((Int32)((Char)e2));
                                            case bool _:
                                                return (bool)e1 || (bool)e2;
                                        }
                                        break;
                                }
                                break;
                            case "-":
                                switch (e1)
                                {
                                    case string _:
                                        return "Valores incompatibles";
                                    case int _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Int32)e1 - (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return (Int32)e1 - (Double)e2;
                                            case char _:
                                                return (Int32)e1 - ((Int32)((Char)e2));
                                            case bool _:
                                                return (Int32)e1 - ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case double _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Double)e1 - (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return (Double)e1 - (Double)e2;
                                            case char _:
                                                return (Double)e1 - ((Int32)((Char)e2));
                                            case bool _:
                                                return (double)e1 - ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case char _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((Int32)((Char)e1)) - (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return ((Int32)((Char)e1)) - (Double)e2;
                                            case char _:
                                                return ((Int32)((Char)e1)) - ((Int32)((Char)e2));
                                            case bool _:
                                                return "Valores incompatibles";
                                        }
                                        break;
                                    case bool _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((bool)e1 ? 1 : 0) - (Int32)e2;
                                            case double _:
                                                return ((bool)e1 ? 1 : 0) - (Double)e2;
                                            default:
                                                return "Valores incompatibles";
                                        }
                                }
                                break;
                            case "*":
                                switch (e1)
                                {
                                    case string _:
                                        return "Valores incompatibles";
                                    case int _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Int32)e1 * (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return (Int32)e1 * (Double)e2;
                                            case char _:
                                                return (Int32)e1 * ((Int32)((Char)e2));
                                            case bool _:
                                                return (Int32)e1 * ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case double _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Double)e1 * (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return (Double)e1 * (Double)e2;
                                            case char _:
                                                return (Double)e1 * ((Int32)((Char)e2));
                                            case bool _:
                                                return (double)e1 * ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case char _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((Int32)((Char)e1)) * (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return ((Int32)((Char)e1)) * (Double)e2;
                                            case char _:
                                                return ((Int32)((Char)e1)) * ((Int32)((Char)e2));
                                            case bool _:
                                                return ((Int32)((Char)e1)) * ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case bool _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((bool)e1 ? 1 : 0) * (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return ((bool)e1 ? 1 : 0) * (Double)e2;
                                            case char _:
                                                return ((bool)e1 ? 1 : 0) * ((Int32)((Char)e2));
                                            case bool _:
                                                return (bool)e1 && (bool)e2;
                                        }
                                        break;
                                }
                                break;
                            case "/":
                                switch (e1)
                                {
                                    case string _:
                                        return "Valores incompatibles";
                                    case int _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Int32)e1 / (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return (Int32)e1 / (Double)e2;
                                            case char _:
                                                return (Int32)e1 / ((Int32)((Char)e2));
                                            case bool _:
                                                return (Int32)e1 / ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case double _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Double)e1 / (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return (Double)e1 / (Double)e2;
                                            case char _:
                                                return (Double)e1 / ((Int32)((Char)e2));
                                            case bool _:
                                                return (double)e1 / ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case char _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((Int32)((Char)e1)) / (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return ((Int32)((Char)e1)) / (Double)e2;
                                            case char _:
                                                return ((Int32)((Char)e1)) / ((Int32)((Char)e2));
                                            case bool _:
                                                return ((Int32)((Char)e1)) / ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case bool _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((bool)e1 ? 1 : 0) / (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return ((bool)e1 ? 1 : 0) / (Double)e2;
                                            case char _:
                                                return ((bool)e1 ? 1 : 0) / ((Int32)((Char)e2));
                                            case bool _:
                                                return "Valores incompatibles";
                                        }
                                        break;
                                }
                                break;
                            case "^":
                                switch (e1)
                                {
                                    case string _:
                                        return "Valores incompatibles";
                                    case int _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return (Int32)e1 ^ (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow((Int32)e1, (Double)e2);
                                            case char _:
                                                return (Int32)e1 ^ ((Int32)((Char)e2));
                                            case bool _:
                                                return (Int32)e1 ^ ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case double _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return Math.Pow((Double)e1, (Int32)e2);
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow((Double)e1, (Double)e2);
                                            case char _:
                                                return Math.Pow((Double)e1, ((Int32)((Char)e2)));
                                            case bool _:
                                                return Math.Pow((Double)e1, ((bool)e2 ? 1 : 0));
                                        }
                                        break;
                                    case char _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((Int32)((Char)e1)) ^ (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow(((Int32)((Char)e1)), (Double)e2);
                                            case char _:
                                                return ((Int32)((Char)e1)) ^ ((Int32)((Char)e2));
                                            case bool _:
                                                return ((Int32)((Char)e1)) ^ ((bool)e2 ? 1 : 0);
                                        }
                                        break;
                                    case bool _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return ((bool)e1 ? 1 : 0) ^ (Int32)e2;
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow(((bool)e1 ? 1 : 0), (Double)e2);
                                            case char _:
                                                return ((bool)e1 ? 1 : 0) ^ ((Int32)((Char)e2));
                                            case bool _:
                                                return (bool)e1 ^ (bool)e2;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case 2:

                    break;
                case 1: 
                    String valor = root.ChildNodes[0].ToString(); 

                    if (valor.Contains(" (numero)")) {
                        return Int32.Parse(root.ChildNodes[0].Token.Value.ToString());
                    } else if (valor.Contains(" (Decimal)")) {
                        return Double.Parse(root.ChildNodes[0].Token.Value.ToString());
                    } else if (valor.Contains(" (cadena)")) {
                        return root.ChildNodes[0].Token.Value.ToString();
                    } else if (valor.Contains(" (caracter)")) {
                        return root.ChildNodes[0].Token.Value.ToString().ToCharArray()[0];
                    } else if (valor.Contains(" (id)")) {
                        Variable var = getValorVariable(root.ChildNodes[0].Token.Value.ToString(), entornos);
                        switch (var.tipo) {
                            case "int":
                                return Int32.Parse(var.valor);
                            case "string":
                                return var.valor;
                            case "double":
                                return Double.Parse(var.valor);
                            case "char":
                                return var.valor.ToCharArray()[0];
                            case "bool":
                                return Boolean.Parse(var.valor);
                        }
                    } else if (valor.Contains("false (Keyword)")) {
                        return false;
                    } else if (valor.Contains("true (Keyword)")) {
                        return true;
                    } else if (valor.Contains("verdadero (Keyword)")) {
                        return true;
                    } else if (valor.Contains("falso (Keyword)")) {
                        return false;
                    } else if (valor.Equals("EXP")) {
                        return EXP(root.ChildNodes[0], entornos);
                    }
                    break;
            }
            return null;
        }
    }
}
