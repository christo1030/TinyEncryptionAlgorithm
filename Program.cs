using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TinyEncryptionAlgo
{
    class Program
    {
        static void Main(string[] args)
        {
            UInt32[] key = { 0x00000000, 0x80000000, 0x00000000, 0x00000000 };
            run_test("TextFile.txt", "output1.txt", key,true);
            key =null;
            key = new UInt32[]{ 0x80000000, 0x00000000, 0x00000000, 0x00000000 };
            run_test("TextFile.txt", "output2.txt", key,true);
            key = null;
            key = new UInt32[] { 0x198bc882, 0x8ada3412, 0x04594590, 0xfc3a1675 };
            run_test("TextFile.txt", "output3.txt", key,true);
            
            
            run_test("TextFile1.txt", "output3.txt", key,false);
            
            run_test("TextFile2.txt", "output3.txt", key,false);
            
            run_test("TextFile3.txt", "output3.txt", key,false);
            
            run_test("TextFile4.txt", "output3.txt", key,false);

            Console.In.ReadLine();
        }
        static void run_test(String in_filename, String out_filename, UInt32[] key,Boolean show_out) {
            //String text  = Console.In.ReadLine();
            Stopwatch timer1 = new Stopwatch();

            string text = System.IO.File.ReadAllText(in_filename);
            Console.WriteLine("Plain Text Size: {0}", text.Length);
            String[] tmp3 = new String[key.Length];
            for (int i = 0; i < key.Length; i++)
            {
                tmp3[i] = (key[i].ToString("X")).PadLeft(8, '0'); ;
            }
            String key_string = ConvertStringArrayToString(tmp3);
            if (show_out)
            {
                Console.WriteLine("Key: {0}", key_string);
                Console.WriteLine("Plain Text:");
                Console.Write(text + "\n");
            }
            string text2 = text.PadRight(text.Length + (8 - (text.Length % 8)));
            Char[] array = text2.ToCharArray();
            String[] stringarray = new String[(text.Length / 8)];
            for (int i = 0; i < stringarray.Length; i++)
                stringarray[i] = text.Substring(i * 8, 8);
            
            UInt32[] array2 = new UInt32[stringarray.Length];

            for (int i = 0; i < stringarray.Length; i++)
            {
                array2[i] = Convert.ToUInt32(stringarray[i], 16);
            }
            //encrypt unsigned int32 array in blocks of 2
            timer1.Start();
            for (int i = 0; i < (stringarray.Length); i += 2)
            {
                UInt32[] tmp = new UInt32[2];
                tmp[1] = array2[i + 1];
                tmp[0] = array2[i]; ;
                encrypt(tmp, key);
                array2[i + 1] = tmp[1];
                array2[i] = tmp[0];
            }
            timer1.Stop();
            String[] tmp2 = new String[array2.Length];
            for (int i = 0; i < array2.Length; i++)
            {
                tmp2[i] = (array2[i].ToString("X"));
            }
            String output = ConvertStringArrayToString(tmp2);
            System.IO.File.WriteAllText(out_filename, output);
            if (show_out)
            {
                Console.WriteLine("Encrypted output: {0}", output);
            }
            Console.WriteLine("Encryption took :" + timer1.Elapsed.ToString());


            timer1.Reset();
            timer1.Start();
            for (int i = 0; i < (stringarray.Length); i += 2)
            {
                UInt32[] tmp = new UInt32[2];
                tmp[1] = array2[i + 1];
                tmp[0] = array2[i]; ;
                decrypt(tmp, key);
                array2[i + 1] = tmp[1];
                array2[i] = tmp[0];
            }
            timer1.Stop();
            Console.WriteLine("Decryption took :" + timer1.Elapsed.ToString());
            //get all elements of decrypted array2 to char

            for (int i = 0; i < array2.Length; i++)
            {
                //array[i] = Convert.ToChar(array2[i]);
                stringarray[i] = array2[i].ToString("X").ToLower();
            }
            if (show_out)
            {
                Console.WriteLine(array);
            }
        }
        static void encrypt(UInt32[] v, UInt32[] key) { 
            UInt32 v0=v[0], v1=v[1], sum=0, i;
            UInt32 delta = 0x9e3779b9;
            UInt32 k0 = key[0], k1=key[1], k2=key[2],k3=key[3];
            for (i = 0; i < 32; i++)
            {
                sum += delta;
                v0 += ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                v1 += ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
            }
            v[0] = v0; v[1] = v1;
            //Console.WriteLine("{0} {1}", v0, v1);
        }

        static void decrypt(UInt32[] v, UInt32[] key)
        {
            UInt32 v0 = v[0], v1 = v[1], sum = 0xC6EF3720, i;  /* set up */
            UInt32 delta = 0x9e3779b9;                     /* a key schedule constant */
            UInt32 k0 = key[0], k1 = key[1], k2 = key[2], k3 = key[3];   /* cache key */
            for (i = 0; i < 32; i++)
            {                         /* basic cycle start */
                v1 -= ((v0 << 4) + k2) ^ (v0 + sum) ^ ((v0 >> 5) + k3);
                v0 -= ((v1 << 4) + k0) ^ (v1 + sum) ^ ((v1 >> 5) + k1);
                sum -= delta;
            }                                              /* end cycle */
            v[0] = v0; v[1] = v1;
            //Console.WriteLine("{0} {1}", v0, v1);
        }
        static string ConvertStringArrayToString(string[] array)
        {
            StringBuilder builder = new StringBuilder();
            foreach (string value in array)
            {
                builder.Append(value);
                builder.Append('.');
            }
            return builder.ToString();
        }
    }
}
