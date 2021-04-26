using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Media;

namespace TetrisCSharp
{
    class Program
    {

        public static int[,] area = new int[23, 10];
        public static int[,] posicionCaidaPieza = new int[32, 10];
        public static string fragmentoFigura = "#";
        public static Stopwatch temporizador = new Stopwatch();
        public static Stopwatch tempoFall = new Stopwatch();
        public static Stopwatch tempoIn = new Stopwatch();
        public static int tiempoFall, veloFall = 300;
        public static bool letFall = false;
        static tetraPieza pieza;
        static tetraPieza piezaSig;
        public static ConsoleKeyInfo tecla;
        public static bool teclaPulsada = false;
        public static int lineas = 0, puntos = 0, nivel = 1;

        static void Main()
        {

            Pantalla();

            Console.SetCursorPosition(4, 5);
            Console.WriteLine("Pulse una tecla");
            Console.ReadKey(true);
            temporizador.Start();
            tempoFall.Start();
            long time = temporizador.ElapsedMilliseconds;
            Console.SetCursorPosition(25, 0);
            Console.WriteLine("Nivel: " + nivel);
            Console.SetCursorPosition(25, 1);
            Console.WriteLine("Puntos: " + puntos);
            Console.SetCursorPosition(25, 2);
            Console.WriteLine("Líneas: " + lineas);
            piezaSig = new tetraPieza();
            pieza = piezaSig;
            pieza.Aparecer();
            piezaSig = new tetraPieza();

            Actualizar();

            Console.SetCursorPosition(0, 0);
            Console.WriteLine("Game Over \n Otra partida? (Y/N)");
            string teclaPulsada = Console.ReadLine();

            if (teclaPulsada.ToUpper() == "Y")
            {
                int[,] areaJuego = new int[23, 10];
                posicionCaidaPieza  = new int[23, 10];
                temporizador = new Stopwatch();
                tempoFall = new Stopwatch();
                tempoIn = new Stopwatch();
                veloFall = 300;
                letFall = false;
                Program.teclaPulsada = false;
                lineas = 0;
                puntos = 0;
                nivel = 1;
                GC.Collect();
                Console.Clear();
                Main();
            }
            else return;

        }

        public static void Pantalla()
        {

            Console.ForegroundColor = ConsoleColor.DarkGray;

            for (int length = 0; length <= 22; ++ length)
            {

                Console.SetCursorPosition(0, length);
                Console.Write("|");
                Console.SetCursorPosition(21, length);
                Console.Write("|");

            }

            Console.SetCursorPosition(0, 23);

            for (int width = 0; width <= 10; ++ width)
            {

                Console.Write("__");

            }

            Console.ForegroundColor = ConsoleColor.White;

        }
                
        public static void Actualizar()
        {

            while (true)
            {

                tiempoFall = (int)tempoFall.ElapsedMilliseconds;
                
                if (tiempoFall > veloFall)
                {

                    tiempoFall = 0;
                    tempoFall.Restart();
                    pieza.Soltar();

                }

                if (letFall == true)
                {

                    pieza = piezaSig;
                    piezaSig = new tetraPieza();
                    pieza.Aparecer();

                    letFall = false;

                }

                int i;
                for (i = 0; i < 10; i++)
                {

                    if (posicionCaidaPieza[0, i] == 1)
                        
                        return;

                }

                Pulso();
                Mover();

            }

        }

        private static void Pulso()
        {
            if (Console.KeyAvailable)
            {
                tecla = Console.ReadKey();
                teclaPulsada = true;
            }
            else
                teclaPulsada = false;

            if (Program.tecla.Key == ConsoleKey.LeftArrow & !pieza.HayAlgoIzquierda() & teclaPulsada)
            {
                for (int i = 0; i < 4; i++)
                {
                    pieza.posicion[i][1] -= 1;
                }
                pieza.Actualizar();

            }
            else if (Program.tecla.Key == ConsoleKey.RightArrow & !pieza.HayAlgoDerecha() & teclaPulsada)
            {
                for (int i = 0; i < 4; i++)
                {
                    pieza.posicion[i][1] += 1;
                }
                pieza.Actualizar();
            }
            if (Program.tecla.Key == ConsoleKey.DownArrow & teclaPulsada)
            {
                pieza.Soltar();
            }
            if (Program.tecla.Key == ConsoleKey.DownArrow & teclaPulsada)
            {
                for (; pieza.HayAlgoDebajo() != true;)
                {
                    pieza.Soltar();
                }
            }
            if (Program.tecla.Key == ConsoleKey.UpArrow & teclaPulsada)
            {

                pieza.Rotar();
                pieza.Actualizar();
            }
        }

        private static void Mover()
        {

            int combo = 0;

            for (int i = 0; i < 23; i++)
            {

                int j;

                for (j = 0; j < 10; j++)
                {

                    if (posicionCaidaPieza[i, j] == 0)
                        break;

                }

                if (j == 10)
                {

                    lineas++;
                    combo++;

                    for (j = 0; j < 10; j++)
                    {

                        posicionCaidaPieza[i, j] = 0;

                    }

                    int[,] newdroppedtetrominoeLocationGrid = new int[23, 10];

                    for (int k = 1; k < i; k++)
                    {

                        for (int l = 0; l < 10; l++)
                        {

                            newdroppedtetrominoeLocationGrid[k + 1, l] = posicionCaidaPieza[k, l];

                        }

                    }

                    for (int k = 1; k < i; k++)
                    {

                        for (int l = 0; l < 10; l++)
                        {

                            posicionCaidaPieza[k, l] = 0;

                        }

                    }

                    for (int k = 0; k < 23; k++)
                        for (int l = 0; l < 10; l++)
                            if (newdroppedtetrominoeLocationGrid[k, l] == 1)
                                posicionCaidaPieza[k, l] = 1;
                    Dibujar();

                }

            }
            if (combo == 1)
                puntos += 40 * nivel;
            else if (combo == 2)
                puntos += 100 * nivel;
            else if (combo == 3)
                puntos += 300 * nivel;
            else if (combo > 3)
                puntos += 300 * combo * nivel;

            if (lineas < 5) nivel = 1;
            else if (lineas < 10) nivel = 2;
            else if (lineas < 15) nivel = 3;
            else if (lineas < 25) nivel = 4;
            else if (lineas < 35) nivel = 5;
            else if (lineas < 50) nivel = 6;
            else if (lineas < 70) nivel = 7;
            else if (lineas < 90) nivel = 8;
            else if (lineas < 110) nivel = 9;
            else if (lineas < 150) nivel = 10;


            if (combo > 0)
            {

                Console.SetCursorPosition(25, 0);
                Console.WriteLine("Nivel: " + nivel);
                Console.SetCursorPosition(25, 1);
                Console.WriteLine("Puntos: " + puntos);
                Console.SetCursorPosition(25, 2);
                Console.WriteLine("Lineas: " + lineas);

            }

            veloFall = 300 - 22 * nivel;

        }

        public static void Dibujar()
        {

            for (int i = 0; i < 23; ++i)
            {

                for (int j = 0; j < 10; j++)
                {

                    Console.SetCursorPosition(1 + 2 * j, i);
                    if (area[i, j] == 1 | posicionCaidaPieza[i, j] == 1)
                    {

                        Console.SetCursorPosition(1 + 2 * j, i);

                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write(fragmentoFigura);
                        Console.ForegroundColor = ConsoleColor.White;

                    }
                    else
                    {

                        Console.Write("  ");

                    }

                }

            }

        }

    }

}
