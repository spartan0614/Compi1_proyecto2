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

        /*--------------------------------------------------RECORRIDO 1--------------------------------------------------------------
                              Almacenado métodos y sus cuerpos y variables globales del archivo actual                              
        ---------------------------------------------------------------------------------------------------------------------------*/

        

        public static String expresion(ParseTreeNode root) {
            switch ((String)root.Term.Name) {
                case "S":
                    
                    expresion(root.ChildNodes[0]);   //LISTA_CLASES
                    break;
                case "LISTA_CLASES":
                    if (root.ChildNodes.Count == 2) {
                        
                        expresion(root.ChildNodes[0]);    //LISTA_CLASES
                        expresion(root.ChildNodes[1]);    //CLASE
                    } else if (root.ChildNodes.Count == 1) {
                        expresion(root.ChildNodes[0]);    //CLASE
                    }
                    break;
                case "CLASE":
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
                IMPORTAR(root.ChildNodes[2]);                       //IMPORTAR
                LISTA_CUERPO(root.ChildNodes[3], entornos);         //LISTA_CUERPO
            }
            else if(root.ChildNodes.Count == 3)
            {
                LISTA_CUERPO(root.ChildNodes[2], entornos);         //LISTA_CUERPO
            }
            MostrarVariables(entornos);
        }

        public static void IMPORTAR(ParseTreeNode root) {
            String listado = "";
            listado += LISTA_ID(root.ChildNodes[1], listado);
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
                    MAIN(root.ChildNodes[0], entornos);
                    break;
                case "DECLARACION":
                    DECLARACION(root.ChildNodes[0], entornos);
                    break;
            }
        }

        private static void MAIN(ParseTreeNode root, Stack<Ambito> entornos) {
            Ambito main_ = new Ambito();
            entornos.Push(main_);
            L_SENTENCIAS(root.ChildNodes[1], entornos);
            entornos.Pop();
        }

        private static void DECLARACION(ParseTreeNode root, Stack<Ambito> entornos) {
            switch (root.ChildNodes.Count)
            {
                case 2:
                    if (root.ChildNodes[0].Token == null)
                    {
                        //DECLARACION = TIPO + LISTA_ID
                        String listado = "";
                        listado += LISTA_ID(root.ChildNodes[1], listado);
                        string[] identificadores = listado.Split(',');
                        for (int i = 0; i < identificadores.Count(); i++)
                        {
                            Variable v = new Variable(1, "publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), identificadores[i], "", "", null);
                            entornos.Peek().Insertar(identificadores[i], v);
                        }
                    }
                    else {
                        //DECLARACION = id + LISTA_ID

                    }
                    break;
                case 3:
                    if (root.ChildNodes[1].Token == null) {
                        //DECLARACION = ACCESO + TIPO + LISTA_ID
                        String listado = "";
                        listado += LISTA_ID(root.ChildNodes[2], listado);
                        string[] identificadores = listado.Split(',');
                        for (int i = 0; i < identificadores.Count(); i++)
                        {
                            Variable v = new Variable(1, root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), root.ChildNodes[1].ChildNodes[0].Token.Value.ToString(), identificadores[i], "", "", null);
                            entornos.Peek().Insertar(identificadores[i], v);
                        }
                    }
                    else {
                        //DECLARACION = ACCESO + id + LISTA_ID
                    }
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
                                Variable v = new Variable(1,"publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), identificadores[i],"","", EXP(root.ChildNodes[3], entornos).ToString() );
                                entornos.Peek().Insertar(identificadores[i], v);
                            }
                        }
                        else
                        {
                            //declaracion objeto
                        }
                    }
                    else {
                        //DECLARACION = TIPO + array + LISTA_ID + DIMENSION
                        //Obteniendo lista de Ids
                        String listado3 = "";
                        listado3 += LISTA_ID(root.ChildNodes[2], listado3);
                        string[] identificadores3 = listado3.Split(',');

                        //Obteniendo dimensiones de arreglo
                        List<object> dimensiones2 = new List<object>();
                        dimensiones2 = (List<object>)DIMENSION(root.ChildNodes[3], dimensiones2, entornos);

                        //Almacenando arreglos
                        for (int i = 0; i < identificadores3.Count(); i++)
                        {
                            Variable v = new Variable(2, "publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), identificadores3[i], dimensiones2.Count.ToString(), dimensiones2, null);
                            entornos.Peek().Insertar(identificadores3[i], v);
                        }
                    }
                    break;
                case 5:
                    if (root.ChildNodes[2].Token == null)
                    {
                        if (root.ChildNodes[1].Token == null)
                        {
                            //DECLARACION = ACCESO + TIPO + LISTA_ID + igual + EXP
                            String listado = "";
                            listado += LISTA_ID(root.ChildNodes[1], listado);
                            string[] identificadores = listado.Split(',');
                            for (int i = 0; i < identificadores.Count(); i++)
                            {
                                Variable v = new Variable(1, root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), root.ChildNodes[1].ChildNodes[0].Token.Value.ToString(), identificadores[i], "", "", EXP(root.ChildNodes[3], entornos).ToString());
                                entornos.Peek().Insertar(identificadores[i], v);
                            }
                        }
                        else {
                            //DECLARACION = ACCESO + id + LISTA_ID + igual + EXP
                        }
                    }
                    else {
                        //DECLARACION = ACCESO + TIPO + array + LISTA_ID + DIMENSION
                        //Obteniendo lista de Ids
                        String listado = "";
                        listado += LISTA_ID(root.ChildNodes[3], listado);
                        string[] identificadores = listado.Split(',');

                        //Obteniendo dimensiones de arreglo
                        List<object> dimensiones4 = new List<object>();
                        dimensiones4 = (List<object>)DIMENSION(root.ChildNodes[4], dimensiones4, entornos);

                        //Almacenando arreglos
                        for (int i = 0; i < identificadores.Count(); i++)
                        {
                            Variable v = new Variable(2, root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), root.ChildNodes[1].ChildNodes[0].Token.Value.ToString(), identificadores[i], dimensiones4.Count.ToString(), dimensiones4, null);
                            entornos.Peek().Insertar(identificadores[i], v);
                        }
                    }
                    break;
                case 6:
                    //DECLARACION = TIPO + array + LISTA_ID + DIMENSION + igual + ARRAY_INIT
                    //Obteniendo lista de Ids
                    String listado2 = "";
                    listado2 += LISTA_ID(root.ChildNodes[2], listado2);
                    string[] identificadores2 = listado2.Split(',');

                    //Obteniendo dimensiones de arreglo
                    List<object> dimensiones = new List<object>();
                    dimensiones = (List<object>)DIMENSION(root.ChildNodes[3],dimensiones, entornos);

                    //Obteniendo valores del arreglo
                    List<object> arreglo = new List<object>();
                    if (dimensiones.Count == 2)
                    {
                       
                        arreglo = (List<object>)ARRAY_INIT(root.ChildNodes[5], arreglo, entornos);
                    }
                    else if (dimensiones.Count == 3) {
                        
                        List<object> arreglo_aux = new List<object>();
                        arreglo_aux = (List<object>)ARRAY_INIT(root.ChildNodes[5], arreglo_aux, entornos);  //Obteniendo filas
                        Int32 primeraDimension = Int32.Parse(dimensiones.ElementAt(0).ToString());           //Tamaño de la primera dimension
                        int div = (arreglo_aux.Count / primeraDimension);                                   //Filas / primera dimension
                        int contador = 0; 
                        for (int i = 0; i < primeraDimension; i++) {
                            List<object> rowFirstDimension = new List<object>();
                            for (int j = 0; j < div; j++)
                            {
                                rowFirstDimension.Add(arreglo_aux[contador]);
                                contador++;
                            }
                            arreglo.Add(rowFirstDimension);
                        }
                    }
                    //Almacenando arreglo
                    for (int i = 0; i < identificadores2.Count(); i++)
                    {
                        Variable v = new Variable(2,"publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), identificadores2[i],dimensiones.Count.ToString(), dimensiones, arreglo);
                        entornos.Peek().Insertar(identificadores2[i], v);
                    }
                    break;
                case 7:

                    break;
            }
        }

        private static object DIMENSION(ParseTreeNode root, List<object> dimension, Stack<Ambito> entornos) {
            if (root.ChildNodes.Count == 2) {
                DIMENSION(root.ChildNodes[0], dimension, entornos);
                dimension.Add(EXP(root.ChildNodes[1],entornos));
            } else if (root.ChildNodes.Count == 1) {
                dimension.Add(EXP(root.ChildNodes[0], entornos));
            }
            return dimension;
        }

        private static object ARRAY_INIT(ParseTreeNode root, List<object> val, Stack<Ambito> entornos) {
            //ARRAY_INIT = LISTA_LLAV
             return LISTA_LLAV(root.ChildNodes[0], val, entornos);
        }

        private static object LISTA_LLAV(ParseTreeNode root, List<object> val, Stack<Ambito> entornos) {
            if (root.ChildNodes.Count == 3) {
                //LISTA_LLAV = LISTA_LLAV + coma + LLAVE
                LISTA_LLAV(root.ChildNodes[0], val, entornos);
                List<object> serie_valores = new List<object>();
                serie_valores = (List<object>)LLAVE(root.ChildNodes[2], val, entornos);
                if (serie_valores != null) { val.Add(serie_valores); }
            }
            else if (root.ChildNodes.Count == 1) {
                //LISTA_LLAV = LLAVE
                List<object> serie_valores = new List<object>();
                serie_valores = (List<object>)LLAVE(root.ChildNodes[0], val, entornos);
                if (serie_valores != null){ val.Add(serie_valores); }
            }
            return val;
        }

        private static object LLAVE(ParseTreeNode root, List<object> val, Stack<Ambito> entornos) {
            //List<object> valores_arreglo = null;
            if (root.ChildNodes[0].Term.Name.ToString() == "LISTA_LLAV") {
                LISTA_LLAV(root.ChildNodes[0], val, entornos);
            } else if (root.ChildNodes[0].Term.Name.ToString() == "LISTA_EXP") {
                List<object> valores_arreglo = new List<object>();
                valores_arreglo = (List<object>)LISTA_EXP(root.ChildNodes[0], valores_arreglo, entornos);
                return valores_arreglo;
            }
            return null;
        }

        private static object LISTA_EXP(ParseTreeNode root, List<object> valores_arreglo, Stack<Ambito> entornos) {
            if (root.ChildNodes.Count == 3) {
                LISTA_EXP(root.ChildNodes[0], valores_arreglo, entornos);
                valores_arreglo.Add(EXP(root.ChildNodes[2], entornos));
            } else if (root.ChildNodes.Count == 1) {
                valores_arreglo.Add(EXP(root.ChildNodes[0], entornos));
            }
            return valores_arreglo;
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
                                    ParseTreeNode raiz_if = SENTENCIA.ChildNodes[0];
                                    Ambito if1 = new Ambito();
                                    List<Condicion> condiciones_if = new List<Condicion>();
                                    bool cumplio = false;
                                    switch (raiz_if.ChildNodes.Count) {
                                        case 1: //NO TIENE ELSE
                                            RECORRER_CONDICIONES(raiz_if.ChildNodes[0], condiciones_if);
                                            for (int i = 0; i < condiciones_if.Count; i++) {
                                                if ((bool)EXP(condiciones_if.ElementAt(i).restriccion, entornos)) {
                                                    cumplio = true;  //Si entró en este if
                                                    entornos.Push(if1);
                                                    var res = L_SENTENCIAS(condiciones_if.ElementAt(i).cuerpo_condicion, entornos);
                                                    entornos.Pop();
                                                    if (!res.Equals("@FINALLY@")) {
                                                        return res;
                                                    }
                                                }
                                                if (cumplio == true) {
                                                    break;
                                                }
                                            }
                                            break;
                                        case 3: //TIENE ELSE
                                            RECORRER_CONDICIONES(raiz_if.ChildNodes[0], condiciones_if);
                                            ParseTreeNode cuerpo_else = raiz_if.ChildNodes[2];
                                            for (int i = 0; i < condiciones_if.Count; i++) {
                                                if ((bool)EXP(condiciones_if.ElementAt(i).restriccion, entornos)) {
                                                    cumplio = true;
                                                    entornos.Push(if1);
                                                    var res = L_SENTENCIAS(condiciones_if.ElementAt(i).cuerpo_condicion, entornos);
                                                    entornos.Pop();
                                                    if (!res.Equals("@FINALLY@")) {
                                                        return res;
                                                    }
                                                }
                                                if (cumplio == true) {
                                                    break;
                                                }
                                            }
                                            if (cumplio == false) {  //EJECUTAR EL ELSE
                                                entornos.Push(if1);
                                                var res = L_SENTENCIAS(cuerpo_else, entornos);
                                                entornos.Pop();
                                                if (!res.Equals("@FINALLY@"))
                                                {
                                                    return res;
                                                }
                                            }
                                            break;
                                    }
                                    break;
                                case "FOR":
                                    ParseTreeNode raiz_for = SENTENCIA.ChildNodes[0];
                                    Ambito for1 = new Ambito();
                                    //Int32 inicio;
                                    Variable valor_inicial = null;
                                    //OBTENIENDO VALOR INICIAL
                                    if (raiz_for.ChildNodes[1].ChildNodes[0].Term.Name.ToString() == "DECLARACION") {
                                        valor_inicial = INIT_DECLARACION_FOR(raiz_for.ChildNodes[1].ChildNodes[0], entornos);
                                    } else if (raiz_for.ChildNodes[1].ChildNodes[0].Term.Name.ToString() == "EXP") {

                                    }
                                    //EJECUTANDO FOR
                                    entornos.Push(for1);
                                    entornos.Peek().Insertar(valor_inicial.nombre, valor_inicial);
                                    while ((bool)EXP(raiz_for.ChildNodes[2], entornos)) {
                                        var res = L_SENTENCIAS(raiz_for.ChildNodes[6], entornos);
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
                                        }

                                        if (raiz_for.ChildNodes[4].Token.Value.ToString() == "+") {
                                            Variable var = getValorVariable(raiz_for.ChildNodes[3].Token.Value.ToString(), entornos);
                                            switch (var.tipo)
                                            {
                                                case "int":
                                                    var.valor = (Convert.ToInt32(var.valor) + 1).ToString();
                                                    break;
                                                case "double":
                                                    var.valor = (Convert.ToDouble(var.valor) + 1).ToString();
                                                    break;
                                                case "char":
                                                    var.valor = (Convert.ToInt32(Convert.ToChar(var.valor)) + 1).ToString();
                                                    break;
                                            }

                                        } else if (raiz_for.ChildNodes[4].Token.Value.ToString() == "-") {
                                            Variable var = getValorVariable(raiz_for.ChildNodes[3].Token.Value.ToString(), entornos);
                                            switch (var.tipo)
                                            {
                                                case "int":
                                                    var.valor = (Convert.ToInt32(var.valor) - 1).ToString();
                                                    break;
                                                case "double":
                                                    var.valor = (Convert.ToDouble(var.valor) - 1).ToString();
                                                    break;
                                                case "char":
                                                    var.valor = (Convert.ToInt32(Convert.ToChar(var.valor)) - 1).ToString();
                                                    break;
                                            }
                                        }
                                    }
                                    entornos.Pop();
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
                                    ParseTreeNode raiz_switch = SENTENCIA.ChildNodes[0];
                                    Ambito switch1 = new Ambito();
                                    bool entro = false, done = false;
                                    var sw = EXP(raiz_switch.ChildNodes[1],entornos);    //Variable que se va a comparar
                                    ParseTreeNode casos = raiz_switch.ChildNodes[2];

                                    switch (raiz_switch.ChildNodes.Count)
                                    {
                                        case 3: //NO TIENE DEFAULT
                                            entornos.Push(switch1);
                                            object res = null;
                                            if (casos.ChildNodes.Count > 0)
                                            {
                                                foreach (var caso in casos.ChildNodes)
                                                {
                                                    if (EXP(caso.ChildNodes[1],entornos).Equals(sw) || entro)
                                                    {
                                                        res = L_SENTENCIAS(caso.ChildNodes[2], entornos);
                                                        entro = true;
                                                        if (!res.Equals("@FINALLY@"))
                                                        {
                                                            if (res.Equals("@SALIR@"))
                                                            {
                                                                done = true;
                                                                res = "@FINALLY@";
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            entornos.Pop();
                                            if (res == null) {
                                                res = "@FINALLY@";
                                            }

                                            if (!res.Equals("@FINALLY@")) {
                                                return res;
                                            }
                                            break;
                                        case 5: //TIENE DEFAULT
                                            ParseTreeNode def = raiz_switch.ChildNodes[4];
                                            entornos.Push(switch1);
                                            object res1 = null;
                                            if (casos.ChildNodes.Count > 0)
                                            {
                                                foreach (var caso in casos.ChildNodes)
                                                {
                                                    if (EXP(caso.ChildNodes[1], entornos).Equals(sw) || entro)
                                                    {
                                                        res1 = L_SENTENCIAS(caso.ChildNodes[2], entornos);
                                                        entro = true;
                                                        if (!res1.Equals("@FINALLY@"))
                                                        {
                                                            if (res1.Equals("@SALIR@"))
                                                            {
                                                                done = true;
                                                                res1 = "@FINALLY@";
                                                                break;
                                                            }
                                                        }
                                                    }
                                                }

                                                if (def.ChildNodes.Count > 0 && !done) {
                                                    res1 = L_SENTENCIAS(def, entornos);
                                                }
                                            }

                                            entornos.Pop();
                                            if (!res1.Equals("@FINALLY@"))
                                            {
                                                return res1;
                                            }
                                            break;
                                    }

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

        private static void RECORRER_CONDICIONES(ParseTreeNode root, List<Condicion> condiciones) {
            if (root.ChildNodes.Count == 5) {
                RECORRER_CONDICIONES(root.ChildNodes[0], condiciones);
                Condicion c = new Condicion(root.ChildNodes[3], root.ChildNodes[4]);
                condiciones.Add(c);
            } else if (root.ChildNodes.Count == 3) {
                Condicion c = new Condicion(root.ChildNodes[1], root.ChildNodes[2]);
                condiciones.Add(c);
            }
        }

        private static Variable INIT_DECLARACION_FOR(ParseTreeNode root, Stack<Ambito> entornos) {
            if (root.ChildNodes.Count == 4)
            {
                if (root.ChildNodes[0].Token == null)  //No es rama
                {
                    if (root.ChildNodes[1].ChildNodes[0].ToString().Contains(" (id)")) //ERROR
                    {
                        String nombre = root.ChildNodes[1].ChildNodes[0].Token.Value.ToString();
                        Variable v = new Variable(1,"publico", root.ChildNodes[0].ChildNodes[0].Token.Value.ToString(), nombre,"","", EXP(root.ChildNodes[3], entornos).ToString());
                        return v;
                    }
                    else {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
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
                                                return Math.Pow((Int32)e1, (Int32)e2);
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow((Int32)e1, (Double)e2);
                                            case char _:
                                                return Math.Pow((Int32)e1, ((Int32)((Char)e2)));
                                            case bool _:
                                                return Math.Pow((Int32)e1, ((bool)e2 ? 1 : 0));
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
                                                return Math.Pow(((Int32)((Char)e1)), (Int32)e2);
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow(((Int32)((Char)e1)), (Double)e2);
                                            case char _:
                                                return Math.Pow(((Int32)((Char)e1)), ((Int32)((Char)e2)));
                                            case bool _:
                                                return Math.Pow(((Int32)((Char)e1)), ((bool)e2 ? 1 : 0));
                                        }
                                        break;
                                    case bool _:
                                        switch (e2)
                                        {
                                            case int _:
                                                return Math.Pow(((bool)e1 ? 1 : 0), (Int32)e2);
                                            case string _:
                                                return "Valores incompatibles";
                                            case double _:
                                                return Math.Pow(((bool)e1 ? 1 : 0), (Double)e2);
                                            case char _:
                                                return Math.Pow(((bool)e1 ? 1 : 0), ((Int32)((Char)e2)));
                                            case bool _:
                                                return "Valores incompatibles";
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    break;
                case 2:
                    if (root.ChildNodes[0].Token.Value.ToString().Equals("!"))
                    {
                        object e = EXP(root.ChildNodes[1], entornos);
                        if (e is bool b) {
                            return !b;
                        }
                    }
                    else if (root.ChildNodes[0].Token.Value.ToString().Equals("-"))
                    {
                        object e = EXP(root.ChildNodes[1], entornos);
                        switch (e)
                        {
                            case int _:
                                return (-1 * (Int32)e);
                            case string _:
                                return "Valores incompatibles";
                            case double _:
                                return (-1 * (Double)e);
                            case char _:
                                return (-1 * ((Int32)((Char)e)));
                            case bool _:
                                return (-1 * ((bool)e ? 1 : 0));
                        }
                    }
                    else if (root.ChildNodes[0].ToString().ToLower().Equals("new"))
                    {

                    }
                    else if (root.ChildNodes[0].ToString().Contains(" (id)"))
                    {
                        if (root.ChildNodes[1].Term.Name == "DIMENSION")
                        {
                            //Nombre del arreglo
                            String nombre = root.ChildNodes[0].Token.Value.ToString();
                            //Obteniendo dimensiones de arreglo
                            List<object> dimensiones = new List<object>();
                            dimensiones = (List<object>)DIMENSION(root.ChildNodes[1], dimensiones, entornos);
                            //Obteniendo variable
                            Variable var = getValorVariable(root.ChildNodes[0].Token.Value.ToString(), entornos);

                            if ((Int32.Parse(var.dimensiones) == 2) && (dimensiones.Count == 2))
                            {
                                //Ingresando al arreglo
                                Int32 x = (Int32)dimensiones.ElementAt(0);
                                Int32 y = (Int32)dimensiones.ElementAt(1);
                                List<object> array = (List<object>)var.valor;
                                List<object> fila = (List<object>)array.ElementAt(x);
                                object val = fila.ElementAt(y);
                                return val;

                            } else if ((Int32.Parse(var.dimensiones) == 3) && (dimensiones.Count == 3)) {
                                //Ingresando al arreglo
                                Int32 x = (Int32)dimensiones.ElementAt(0);
                                Int32 y = (Int32)dimensiones.ElementAt(1);
                                Int32 z = (Int32)dimensiones.ElementAt(2);
                                List<object> array = (List<object>)var.valor;
                                List<object> fila = (List<object>)array.ElementAt(x);
                                List<object> columna = (List<object>)fila.ElementAt(y);
                                object val = columna.ElementAt(z);
                                return val;
                            }
                            else {
                                return "Dimensiones incorrectas";

                            }
                        }
                        else if (root.ChildNodes[1].Term.Name == "CALL") {

                        }
                    }
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
                                return Int32.Parse(var.valor.ToString());
                            case "string":
                                return var.valor;
                            case "double":
                                return Double.Parse(var.valor.ToString());
                            case "char":
                                return var.valor.ToString().ToCharArray()[0];
                            case "bool":
                                return Boolean.Parse(var.valor.ToString());
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
