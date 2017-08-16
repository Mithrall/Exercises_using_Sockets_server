using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Threading;

namespace Exercises_using_Sockets {
    class Program {
        static void Main(string[] args) {
            Console.WriteLine("Waiting for a connection");

            TcpListener listener = new TcpListener(IPAddress.Any, 1302);
            TcpClient client;
            listener.Start();

            while (true) { //exit flag here ?
                client = listener.AcceptTcpClient();
                ThreadPool.QueueUserWorkItem(ThreadProc, client);                
            }
        }

        private static void ThreadProc(object obj) {
            var client = (TcpClient)obj;

            StreamReader sr = new StreamReader(client.GetStream());
            StreamWriter sw = new StreamWriter(client.GetStream());
            IPEndPoint ipep = (IPEndPoint)client.Client.RemoteEndPoint;
            Console.WriteLine("New connection, IP: " + ipep.Address + " | port: " + ipep.Port);

            bool conn = true;
            while (conn) {
                Console.WriteLine("Ready");
                string request = sr.ReadLine();
                Console.WriteLine("message from client: " + request);
                switch (request) {
                    case "Hello server":
                        sw.WriteLine("Hello client");
                        sw.Flush();
                        break;
                    case "time":
                        sw.WriteLine(DateTime.Now.ToString("h:mm:ss tt"));
                        sw.Flush();
                        break;
                    case "date":
                        sw.WriteLine(DateTime.Now.ToString("dd-MM-yyyy"));
                        sw.Flush();
                        break;
                    case "end":
                        sw.WriteLine("Closing connection");
                        sw.Flush();
                        client.Close();
                        Console.WriteLine("Closed a connection");
                        conn = false;
                        break;
                    case "add":
                        sw.WriteLine("Type 2 numbers with a space between");
                        sw.Flush();
                        while(true) {
                            Console.WriteLine("waiting for numbers");
                            string input = sr.ReadLine();
                            if (input != "exit") {
                                string[] numbers = input.Split(' ');
                                int sum = Convert.ToInt32(numbers[0]) + Convert.ToInt32(numbers[1]);
                                sw.WriteLine("sum is: " + sum.ToString());
                                sw.Flush();
                            } else {
                                sw.WriteLine("Finished adding.");
                                sw.Flush();
                                break;
                            }
                        }
                        break;
                    case "sub":
                        sw.WriteLine("Type 2 numbers with a space between");
                        sw.Flush();
                        while (true) {
                            Console.WriteLine("waiting for numbers");
                            string input2 = sr.ReadLine();
                            if (input2 != "exit") {
                                string[] numbers2 = input2.Split(' ');
                                int sub = Convert.ToInt32(numbers2[0]) + Convert.ToInt32(numbers2[1]);
                                sw.WriteLine("difference is: " + sub.ToString());
                                sw.Flush();
                            } else {
                                sw.WriteLine("Finished subtracting.");
                                sw.Flush();
                                break;
                            }
                        }
                        break;
                    case "guess":
                        Random rnd = new Random();
                        int correctAnswer = rnd.Next(1, 10);
                        int guesses = 0;
                        sw.WriteLine("Guess a number between 1 and 10");
                        sw.Flush();

                        Console.WriteLine("waiting for number");
                        bool playing = true;
                        while (playing) {
                            string input3 = sr.ReadLine();
                            guesses++;
                            //exit
                            if (input3 == "exit") {
                                sw.WriteLine("Finished playing guess game.");
                                sw.Flush();
                                playing = false;
                            //used all tries
                            } else if (guesses == 10) {
                                sw.WriteLine("You didn't manage to guess the right number. Do you want to try again (y/n)?");
                                sw.Flush();
                                //Yes
                                if (sr.ReadLine() == "y") {
                                    sw.WriteLine("Guess a number between 1 and 10");
                                    sw.Flush();
                                    guesses = 0;
                                    correctAnswer = rnd.Next(1, 10);
                                //No
                                } else if (sr.ReadLine() == "n") {
                                    sw.WriteLine("Finished playing guess game.");
                                    sw.Flush();
                                    playing = false;
                                    break;
                                //anti error (burde lave en bedre fix
                                } else {
                                    sw.WriteLine("Finished playing guess game.");
                                    sw.Flush();
                                    playing = false;
                                    break;
                                }
                            //correct
                            } else if (input3 == correctAnswer.ToString()) {
                                sw.WriteLine("Great! just " + guesses + " guesses. Do you want to try again (y/n)?");
                                sw.Flush();
                                //Yes
                                if (sr.ReadLine() == "y") {
                                    sw.WriteLine("Guess a number between 1 and 10");
                                    sw.Flush();
                                    guesses = 0;
                                    correctAnswer = rnd.Next(1, 10);
                                //No
                                } else if (sr.ReadLine() == "n") {
                                    sw.WriteLine("Finished playing guess game.");
                                    sw.Flush();
                                    playing = false;
                                    break;
                                //anti error (burde lave en bedre fix
                                } else {
                                    sw.WriteLine("Finished playing guess game.");
                                    sw.Flush();
                                    playing = false;
                                    break;
                                }
                                //wrong
                            } else if (input3 != correctAnswer.ToString()) {
                                sw.WriteLine("Wrong answer, you got " + (10 - guesses).ToString() + " more tries");
                                sw.Flush();
                            }
                        }
                        break;
                    default:
                        sw.WriteLine("unknown command");
                        sw.Flush();
                        break;
                }
            }
        }
    }
}
