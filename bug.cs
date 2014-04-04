using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        private static Random _rnd = new Random((int)DateTime.Now.Ticks & 0x0000FFFF);

        private static string myrandcall()
        {
            string[] prefix = new string[] {"a", "n", "k", "w"};
            string[] letter = new string[] {"a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "r", "s", "t", "u", "w", "x","y", "z"};
            StringBuilder call = new StringBuilder();
            switch((int)_rnd.Next(6))
            {
                case 0: // 1x1 call
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    call.Append(_rnd.Next(10).ToString());
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
                case 1: // 1x2 call
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    call.Append(_rnd.Next(10).ToString());
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
                case 2: // 1x3 call
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                  call.Append(_rnd.Next(10).ToString());
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
                case 3: // 2x1 call
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    if (call[0].Equals("a")) // only A[A-L] are valid 2xn calls
                    {
                        call.Append(letter[_rnd.Next(13)]);
                    }
                    else
                    {
                        call.Append(letter[_rnd.Next(letter.Length)]);
                    }
                   call.Append(_rnd.Next(10).ToString());
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
                case 4: // 2x2 call
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    if (call[0].Equals("a"))
                    {
                        call.Append(letter[_rnd.Next(13)]);
                    }
                    else
                    {
                        call.Append(letter[_rnd.Next(letter.Length)]);
                    }
                    call.Append(_rnd.Next(10).ToString());
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
                case 5: // 2x3 call
                default:
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    if (call[0].Equals("a"))
                    {
                        call.Append(letter[_rnd.Next(13)]);
                    }
                    else
                    {
                        call.Append(letter[_rnd.Next(letter.Length)]);
                    }
                    call.Append(_rnd.Next(10).ToString());
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
            }

            return call.ToString();
            
            
        }

        private static string randCall()
        {
            // Generate a random "call sign" to play
            // US call sign rules http://wireless.fcc.gov/services/index.htm?job=call_signs_1&id=amateur
            string prefix = "aknw";
            string letter = "abcdefghijklmnopqrstuvwxyz";
            string number = "0123456789";
            StringBuilder call = new StringBuilder();
            // Decide what length of call sign to generate
            switch ((int)_rnd.Next(6))
            {
                case 0: // 1x1 call
                    call.Append(prefix[_rnd.Next(prefix.Length + 1)]);
                    call.Append(number[_rnd.Next(number.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    break;
                case 1: // 1x2 call
                    call.Append(prefix[_rnd.Next(prefix.Length + 1)]);
                    call.Append(number[_rnd.Next(number.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    break;
                case 2: // 1x3 call
                    call.Append(prefix[_rnd.Next(prefix.Length + 1)]);
                    call.Append(number[_rnd.Next(number.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    break;
                case 3: // 2x1 call
                    call.Append(prefix[_rnd.Next(prefix.Length + 1)]);
                    if (call[0].Equals("a")) // only A[A-L] are valid 2xn calls
                    {
                        call.Append(letter[_rnd.Next(13)]);
                    }
                    else
                    {
                        call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    }
                    call.Append(number[_rnd.Next(number.Length + 1)]);
                    call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    break;
                case 4: // 2x2 call
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    if (call[0].Equals("a"))
                    {
                        call.Append(letter[_rnd.Next(13)]);
                    }
                    else
                    {
                        call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    }
                    call.Append(number[_rnd.Next(number.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
                case 5: // 2x3 call
                default:
                    call.Append(prefix[_rnd.Next(prefix.Length)]);
                    if (call[0].Equals("a"))
                    {
                        call.Append(letter[_rnd.Next(13)]);
                    }
                    else
                    {
                        call.Append(letter[_rnd.Next(letter.Length + 1)]);
                    }
                    call.Append(number[_rnd.Next(number.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    call.Append(letter[_rnd.Next(letter.Length)]);
                    break;
            }

            return call.ToString();
        }
        
        static void Main(string[] args)
        {
            Hashtable morse = new Hashtable();

            morse.Add('a', ".-");
            morse.Add('b', "-...");
            morse.Add('c', "-.-.");
            morse.Add('d', "-..");
            morse.Add('e', ".");
            morse.Add('f', "..-.");
            morse.Add('g', "--.");
            morse.Add('h', "....");
            morse.Add('i', "..");
            morse.Add('j', ".---");
            morse.Add('k', "-.-");
            morse.Add('l', ".-..");
            morse.Add('m', "--");
            morse.Add('n', "-.");
            morse.Add('o', "---");
            morse.Add('p', ".--.");
            morse.Add('q', "--.-");
            morse.Add('r', ".-.");
            morse.Add('s', "...");
            morse.Add('t', "-");
            morse.Add('u', "..-");
            morse.Add('v', "...-");
            morse.Add('w', ".--");
            morse.Add('x', "-..-");
            morse.Add('y', "-.--");
            morse.Add('z', "--..");
            morse.Add('0', "-----");
            morse.Add('1', ".----");
            morse.Add('2', "..---");
            morse.Add('3', "...--");
            morse.Add('4', "....-");
            morse.Add('5', ".....");
            morse.Add('6', "-....");
            morse.Add('7', "--...");
            morse.Add('8', "---..");
            morse.Add('9', "----.");
            morse.Add(' ', " ");
            morse.Add('.', "·–·–·–");
            morse.Add(',', "--..--");
            morse.Add('?', "..--..");
            morse.Add('!', "-.-.--");
            morse.Add('/', "-..-.");

            while (true)
            {
                // Get a random call sign to play
                string morseText = myrandcall();
                foreach (char c in morseText)
                {
                    // Get the Morse "song" corresponding to the current letter and send it to buzz()
                    // buzz(speaker, (string)morse[c], note);
                    Console.Write(c);
                }
                // Delay 5 seconds before repeating
                // speaker.SetDutyCycle(0);
                //Thread.Sleep(5000);
            }
        }
    }
}
