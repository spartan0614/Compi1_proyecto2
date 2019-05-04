using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Ast;
using Irony.Parsing;

namespace Interpreter.analizador
{
    class Gramatica : Grammar
    {
        public Gramatica() : base(caseSensitive: false) {
            #region EXPRESION REGULAR
            RegexBasedTerminal numero = new RegexBasedTerminal("numero", "[0-9]+");
            RegexBasedTerminal Decimal = new RegexBasedTerminal("Decimal", "[0-9]+\\.[0-9]+");
            StringLiteral cadena = new StringLiteral("cadena", "\"");
            StringLiteral caracter = TerminalFactory.CreateCSharpChar("caracter");
            IdentifierTerminal id = new IdentifierTerminal("id");
            CommentTerminal comentarioUnaLinea = new CommentTerminal("comentarioUnaLinea", ">>", "\n", "\r\n");
            CommentTerminal comentarioMultilinea = new CommentTerminal("comentarioMultilinea", "<-", "->");
            NonGrammarTerminals.Add(comentarioUnaLinea);
            NonGrammarTerminals.Add(comentarioMultilinea);
            #endregion

            #region TERMINALES
            var punto = ToTerm(".");
            var coma = ToTerm(",");
            //operadores aritméticos y lógicos
            var mayor = ToTerm(">");
            var menor = ToTerm("<");
            var igual = ToTerm("=");
            var mayorigual = ToTerm(">=");
            var menorigual = ToTerm("<=");
            var igualigual = ToTerm("==");
            var not = ToTerm("!");
            var diferente = ToTerm("!=");
            var and = ToTerm("&&");
            var or = ToTerm("||");
            var potencia = ToTerm("^");
            var mult = ToTerm("*");
            var div = ToTerm("/");
            var mas = ToTerm("+");
            var menos = ToTerm("-");
            var parAbre = ToTerm("(");
            var parCierra = ToTerm(")");
            var llavAbre = ToTerm("{");
            var llavCierra = ToTerm("{");
            //tipos de dato
            var Int = ToTerm("int");
            var String = ToTerm("string");
            var Double = ToTerm("double");
            var Char = ToTerm("char");
            var Bool = ToTerm("bool");
            //tipos de acceso
            var publico = ToTerm("publico");
            var privado = ToTerm("privado");
            //palabras reservadas
            var clase = ToTerm("clase");
            var importar = ToTerm("importar");
            var Void = ToTerm("void");
            var array = ToTerm("array");
            var True = ToTerm("true");
            var False = ToTerm("false");
            var verdadero = ToTerm("verdadero");
            var falso = ToTerm("falso");
            var New = ToTerm("new");
            var main = ToTerm("main");
            var Override = ToTerm("Override");
            var If = ToTerm("if");
            var Else = ToTerm("else");
            var For = ToTerm("for");
            var repeat = ToTerm("repeat");
            var While = ToTerm("while");
            var comprobar = ToTerm("comprobar");
            var caso = ToTerm("caso");
            var salir = ToTerm("salir");
            var defecto = ToTerm("defecto");
            var hacer = ToTerm("hacer");
            var mientras = ToTerm("mientras");
            var Return = ToTerm("return");
            var print = ToTerm("print");
            var show = ToTerm("show");
            var continuar = ToTerm("continuar");

            var addFigure = ToTerm("addfigure");
            var circle = ToTerm("circle");
            var triangle = ToTerm("triangle");
            var square = ToTerm("square");
            var line = ToTerm("line");
            #endregion

            #region NO TERMINALES
            NonTerminal S = new NonTerminal("S");
            NonTerminal LISTA_CLASES = new NonTerminal("LISTA_CLASES");
            NonTerminal CLASE = new NonTerminal("CLASE");
            NonTerminal IMPORTAR = new NonTerminal("IMPORTAR");
            NonTerminal LISTA_CUERPO = new NonTerminal("LISTA_CUERPO");
            NonTerminal CUERPO = new NonTerminal("CUERPO");
            NonTerminal MAIN = new NonTerminal("MAIN");
            NonTerminal METODO = new NonTerminal("METODO");
            NonTerminal FUNCION = new NonTerminal("FUNCION");
            NonTerminal FUNCION_HEADER = new NonTerminal("FUNCION_HEADER");
            NonTerminal FUNCION_PARAM = new NonTerminal("FUNCION_PARAM");
            NonTerminal DECLARACION = new NonTerminal("DECLARACION");
            NonTerminal DECLA_VARIABLES = new NonTerminal("DECLA_VARIABLES");
            NonTerminal VAR = new NonTerminal("VAR");
            NonTerminal VAR_ID = new NonTerminal("VAR_ID");
            NonTerminal ACCESO = new NonTerminal("ACCESO");
            NonTerminal LISTA_ID = new NonTerminal("LISTA_ID");
            NonTerminal DIMENSION = new NonTerminal("DIMENSION");
            NonTerminal ARRAY_INIT = new NonTerminal("ARRAY_INIT");
            NonTerminal LISTA_LLAV = new NonTerminal("LISTA_LLAV");
            NonTerminal LLAVE = new NonTerminal("LLAVE");
            NonTerminal LISTA_EXP = new NonTerminal("LISTA_EXP");
            NonTerminal TIPO = new NonTerminal("TIPO");
            NonTerminal EXP = new NonTerminal("EXP");
            NonTerminal VALOR = new NonTerminal("VALOR");
            NonTerminal LISTA_PARAMETROS = new NonTerminal("LISTA_PARAMETROS");
            NonTerminal PARAMETRO = new NonTerminal("PARAMETRO");
            NonTerminal RETORNO = new NonTerminal("RETORNO");
            NonTerminal L_SENTENCIAS = new NonTerminal("L_SENTENCIAS");
            NonTerminal SENTENCIA = new NonTerminal("SENTENCIA");
            NonTerminal INIT = new NonTerminal("INIT");
            NonTerminal IF = new NonTerminal("IF");
            NonTerminal IF_LIST = new NonTerminal("IF_LIST");
            NonTerminal FOR = new NonTerminal("FOR");
            NonTerminal FOR_INIT = new NonTerminal("FOR_INIT");
            NonTerminal REPEAT = new NonTerminal("REPEAT");
            NonTerminal WHILE = new NonTerminal("WHILE");
            NonTerminal COMPROBAR = new NonTerminal("COMPROBAR");
            NonTerminal CASOS = new NonTerminal("CASOS");
            NonTerminal CASO = new NonTerminal("CASO");
            NonTerminal HACER = new NonTerminal("HACER");
            NonTerminal PASOS = new NonTerminal("PASOS");
            NonTerminal FIGURA = new NonTerminal("FIGURA");
            #endregion

            #region GRAMATICA
            S.Rule = LISTA_CLASES
                              ;

            LISTA_CLASES.Rule = LISTA_CLASES + CLASE
                              | CLASE
                              ;

            CLASE.Rule = clase + id + IMPORTAR + "{" + LISTA_CUERPO + "}"
                       | clase + id + "{" + LISTA_CUERPO +"}"
                       ;

            IMPORTAR.Rule = importar + LISTA_ID
                        ;

            LISTA_ID.Rule = LISTA_ID + coma + id
                        | id
                        ;

            LISTA_CUERPO.Rule = LISTA_CUERPO + CUERPO
                              | CUERPO
                              ;

            CUERPO.Rule = FUNCION
                        | MAIN
                        | DECLARACION + ";"
                        //| EXP + ";"
                        ;

            DECLARACION.Rule = ACCESO + TIPO + LISTA_ID + igual + EXP                               //5
                             | ACCESO + TIPO + array + LISTA_ID + DIMENSION + igual + ARRAY_INIT    //7
                             | TIPO + LISTA_ID + igual + EXP                                        //4 **
                             | TIPO + array + LISTA_ID + DIMENSION + igual + ARRAY_INIT             //6
                             | ACCESO + TIPO + LISTA_ID                                             //3
                             | ACCESO + TIPO + array + LISTA_ID + DIMENSION                         //5
                             | TIPO + LISTA_ID                                                      //2
                             | TIPO + array + LISTA_ID + DIMENSION                                  //4 **
                             //De tipo id
                             | ACCESO + id + LISTA_ID + igual + EXP                                 //5
                             | id + LISTA_ID + igual + EXP                                          //4 **
                             | ACCESO + id + LISTA_ID                                               //3
                             | id + LISTA_ID                                                        //2
                             
                             ;

            FUNCION.Rule = FUNCION_HEADER + "{" + L_SENTENCIAS + "}"
                         ;

            //FUNCION_PARAM.Rule = FUNCION_HEADER + "(" + LISTA_PARAMETROS + ")"
            //                   | FUNCION_HEADER + "(" + ")"
            //                   ;

            FUNCION_HEADER.Rule = //CON PARÁMETROS
                                  ACCESO + id + Void + "(" + LISTA_PARAMETROS + ")"
                                | ACCESO + id + Void + Override + "(" + LISTA_PARAMETROS + ")"
                                | id + Void + Override + "(" + LISTA_PARAMETROS + ")"
                                | id + Void + "(" + LISTA_PARAMETROS + ")"
                                //Funciones con retorno
                                | ACCESO + id + TIPO + "(" + LISTA_PARAMETROS + ")"
                                | ACCESO + id + TIPO + Override + "(" + LISTA_PARAMETROS + ")"
                                | id + TIPO + Override + "(" + LISTA_PARAMETROS + ")"
                                | id + TIPO + "(" + LISTA_PARAMETROS + ")"
                                //Funciones con retorno de arreglos
                                | ACCESO + id + array + TIPO + DIMENSION + "(" + LISTA_PARAMETROS + ")"
                                | id + array + TIPO + DIMENSION + "(" + LISTA_PARAMETROS + ")"
                                //SIN PARÁMETROS
                                | ACCESO + id + Void + "(" + ")"
                                | ACCESO + id + Void + Override + "(" + ")"
                                | id + Void + Override + "(" + ")"
                                | id + Void + "(" + ")"
                                //Funciones con retorno
                                | ACCESO + id + TIPO + "(" + ")"
                                | ACCESO + id + TIPO + Override + "(" + ")"
                                | id + TIPO + Override + "(" + ")"
                                | id + TIPO + "(" + ")"
                                //Funciones con retorno de arreglos
                                | ACCESO + id + array + TIPO + DIMENSION + "(" + ")"
                                | id + array + TIPO + DIMENSION + "(" + ")"
                                //###########   DE TIPO ID
                                | ACCESO + id + id + "(" + LISTA_PARAMETROS + ")"
                                | ACCESO + id + id + Override + "(" + LISTA_PARAMETROS + ")"
                                | id + id + Override + "(" + LISTA_PARAMETROS + ")"
                                | id + id + "(" + LISTA_PARAMETROS + ")"
                                | ACCESO + id + id + "(" + ")"
                                | ACCESO + id + id + Override + "(" + ")"
                                | id + id + Override + "(" + ")"
                                | id + id + "(" + ")"
                                ;

            MAIN.Rule = main + "(" + ")" + "{" + L_SENTENCIAS + "}"
                      ;

            L_SENTENCIAS.Rule = MakeStarRule(L_SENTENCIAS, SENTENCIA);

            //L_SENTENCIAS.Rule = L_SENTENCIAS + SENTENCIA
            //                | SENTENCIA
            //                ;

            SENTENCIA.Rule = DECLARACION + ";"                          //1*
                           | Return + EXP + ";"                         //2*
                           | Return + ";"                               //1
                           | EXP + ";"                                  //1*
                           | continuar + ";"                            //1
                           | salir + ";"                                //1
                           | print + "(" + EXP + ")" + ";"              //2*
                           | show + "(" + EXP + coma + EXP + ")" + ";"  //4
                           | addFigure + "(" + FIGURA + ")"             //2
                           | IF                                         //1*
                           | FOR                                        //1**
                           | REPEAT                                     //1**
                           | WHILE                                      //1**
                           | HACER                                      //1*
                           | COMPROBAR                                  //1*
                           ;

            IF.Rule = IF_LIST + Else + "{" + L_SENTENCIAS + "}"
                    | IF_LIST
                    ;

            IF_LIST.Rule = IF_LIST + Else + If + "(" + EXP + ")" + "{" + L_SENTENCIAS + "}"
                         | If + "(" + EXP + ")" + "{" + L_SENTENCIAS + "}"
                         ;

            FOR.Rule = For + "(" + FOR_INIT + ";" + EXP + ";" + id + mas + mas + ")" + "{" + L_SENTENCIAS + "}"
                     | For + "(" + FOR_INIT + ";" + EXP + ";" + id + menos + menos + ")" + "{" + L_SENTENCIAS + "}"
                     ;

            FOR_INIT.Rule = DECLARACION
                          | EXP
                          ;

            REPEAT.Rule = repeat + "(" + EXP + ")" + "{" + L_SENTENCIAS + "}"
                        ;

            WHILE.Rule = While + "(" + EXP + ")" + "{" + L_SENTENCIAS + "}"
                       ;

            HACER.Rule = hacer + "{" + L_SENTENCIAS + "}" + mientras + "(" + EXP + ")" + ";"
                     ;

            COMPROBAR.Rule = comprobar + "(" + EXP + ")" + "{" + CASOS + "}"
                           | comprobar + "(" + EXP + ")" + "{" + CASOS + defecto + ":" + L_SENTENCIAS + "}"
                           ;

            //CASOS.Rule = CASOS + caso + EXP + ":" + L_SENTENCIAS + salir + ";"
            //           | caso + EXP + ":" + L_SENTENCIAS + salir + ";"
            //           ;

            CASOS.Rule = MakePlusRule(CASOS, CASO);

            CASO.Rule = caso + EXP + ":" + L_SENTENCIAS
                      ;

            FIGURA.Rule = circle + "(" + LISTA_EXP + ")"        //color, radio, solido, posx centro, posy centro
                        | triangle + "(" + LISTA_EXP + ")"      //color, solido, puntox v1, puntoy v1, puntox v2, puntoy v2, puntox v3, puntoy v3
                        | square + "(" + LISTA_EXP + ")"        //color, solido, posx centro, posy centro, alto, ancho
                        | line + "(" + LISTA_EXP + ")"          //color, posx inicio, posy inicio, posx final, posy final, grosor
                        ;


            ACCESO.Rule = publico
                             | privado
                             ;

            LISTA_PARAMETROS.Rule = LISTA_PARAMETROS + coma + PARAMETRO
                                  | PARAMETRO
                                  ;

            PARAMETRO.Rule = TIPO + id
                           | id + id
                           ;

            TIPO.Rule = Int
                      | String
                      | Double
                      | Char
                      | Bool
                      ;

            DIMENSION.Rule = DIMENSION + "[" + EXP + "]"
                           | "[" + EXP + "]"
                           ;

            ARRAY_INIT.Rule = "{" + LISTA_LLAV + "}"
                            ;

            LISTA_LLAV.Rule = LISTA_LLAV + coma + LLAVE
                            | LLAVE
                            ;

            LLAVE.Rule = "{" + LISTA_LLAV + "}"
                       | LISTA_EXP
                       ;

            LISTA_EXP.Rule = LISTA_EXP + coma + EXP
                           | EXP
                           ;

            EXP.Rule = EXP + igual + EXP            //3
                     | EXP + or + EXP               
                     | EXP + and + EXP              
                     | EXP + potencia + EXP         
                     | not + EXP                    //2 
                     | EXP + mayor + EXP            
                     | EXP + menor + EXP            
                     | EXP + mayorigual + EXP       
                     | EXP + menorigual + EXP       
                     | EXP + igualigual + EXP       
                     | EXP + diferente + EXP        
                     | EXP + mas + EXP              
                     | EXP + menos + EXP            
                     | EXP + mult + EXP             
                     | EXP + div + EXP              
                     | menos + EXP                  //2 
                     | EXP + mas + mas              //3
                     | EXP + menos + menos          //3
                     | "(" + EXP + ")"    //3    
                     | New + id + "(" + ")"         //2       
                     | Decimal                      
                     | numero                       
                     | id                                      
                     | id + punto + EXP             //3
                     | id + "(" + LISTA_EXP + ")"   //2 
                     | id + parAbre + parCierra     //3                 
                     | cadena                       
                     | caracter                     
                     | verdadero                    
                     | falso                        
                     | True                         
                     | False                        
                     | id + DIMENSION               //2          
                     ;

            #endregion

            #region PREFERENCIAS
            this.Root = S;
            MarkPunctuation("{", "}", "(", ")", ";", "[", "]", ":");

            RegisterOperators(7, Associativity.Right,"^");
            RegisterOperators(6, Associativity.Left, "*", "/");
            RegisterOperators(5, Associativity.Left, "+", "-");
            RegisterOperators(4, Associativity.Left, ">", "<", ">=", "<=", "==", "!=", "=");
            RegisterOperators(3, Associativity.Right, "!");
            RegisterOperators(2, Associativity.Left, "&&");
            RegisterOperators(1, Associativity.Left, "||");
            #endregion
        }
    }
}
