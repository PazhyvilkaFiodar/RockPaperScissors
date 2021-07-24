using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace rock_paper_scissors
{
    class Program
    {
        static bool CheckEqual(string[] args)
        {
            for(int i = 0; i < args.Length - 1; i++)
                for(int j= i + 1; j < args.Length; j++)
                    if (args[i].Equals(args[j]))
                        return false;
            return true;
        }
        static string ToHexString(byte[] args)
        {
            var hex = BitConverter.ToString(args);
            hex = hex.Replace("-","");
            return hex;
        }
        static void FillMask(out int[] mask, int length, int computerMove)
        { 
            mask = new int[length];
            mask[computerMove] = 0;
            int count = 0;
            for (int i = computerMove + 1; i < length && count != length / 2; i++)
            {
                mask[i] = 1;
                ++count;
            }
            if (count < length / 2)
            {
                int index = 0;
                while (count != length / 2)
                {
                    mask[index] = 1;
                    ++count;
                    ++index;
                }
                for (int i = index; i < computerMove; i++)
                    mask[i] = -1;
            }
            else
            {
                for (int i = computerMove + length / 2 + 1; i < length; i++)
                    mask[i] = -1;
                for (int i = 0; i < computerMove; i++)
                    mask[i] = -1;
            }
        }
        static void EnterUserMove(out int userMove, string[] args)
        {
            while (true)
            {
                for (int i = 0; i < args.Length; i++)
                    Console.WriteLine("{0} - {1}", i + 1, args[i]);
                Console.WriteLine("0 - exit");
                Console.Write("Enter your move: ");
                userMove = Convert.ToInt32(Console.ReadLine());
                if(userMove >= 0 && userMove <= args.Length)
                    break;
                Console.WriteLine("Wrong move. Try again");
            }   
        }
        static int Main(string[] args)
        {
            if (args.Length % 2 == 0 || args.Length < 3 || !CheckEqual(args))
            {
                Console.WriteLine("Wrong arguments");
                return 0;
            }
            var key = new byte[16];
            using (var generator = RandomNumberGenerator.Create())
            {
                generator.GetBytes(key);
            }
            var gen = new Random();
            int computerMove = gen.Next(0, args.Length - 1);
            int[] mask;
            FillMask(out mask, args.Length, computerMove);
            byte[] hash;
            using (HMACSHA256 hmac = new HMACSHA256(key))
            {
                hash = hmac.ComputeHash(Encoding.Default.GetBytes(computerMove.ToString()));
            }
            Console.WriteLine("HMAC: {0}", ToHexString(hash));
            int userMove;
            EnterUserMove(out userMove, args);
            Console.WriteLine("Your move: {0}", userMove == 0 ? 0.ToString() : args[userMove - 1]);
            Console.WriteLine("Computer move: {0}", args[computerMove]);
            if (userMove == 0)
            {
                Console.WriteLine("You exit");
                return 0;
            }
            if(mask[userMove - 1] == 1)
                Console.WriteLine("You win!");
            if(mask[userMove - 1] == 0)
                Console.WriteLine("It's a draw.");
            if(mask[userMove - 1] == -1)
                Console.WriteLine("You lose");
            Console.WriteLine("HMAC key: {0}", ToHexString(key));
            return 0;
        }
    }
}
