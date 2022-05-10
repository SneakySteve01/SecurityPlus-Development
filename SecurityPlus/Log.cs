using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SecurityPlus
{
    /*
     * This class is used to securely encrypt and decrypt
     * log file data. The log file is what's sent off to
     * the remote server in the WIP logging system.
     * Why is it encrypted? Because I wanted to code some
     * encryption stuff, that's why. Feel free to use whatever
     * you want as usual.
     */
    public class Log
    {
        // This was the key used to encrypt the log file, obviously it's no longer secure, but I'm keeping it here for reference.
        private const string cryptoKey = "hVmYq3t6w9z$C&F)";

        // Just gets the time stamp for the log file.
        private static string time()
        {
            return DateTime.Now.ToString().Split(' ')[1] + " " + DateTime.Now.ToString().Split(' ')[2];
        }
        
        // This is the main method that writes a new log
        public static void log(string text)
        {
            /*
             * When log is written, you'll notice we just use C#'s built-in logging
             * system but encrypt the text. This is because we catch the Console.WriteLine
             * in another part of the code and send it to the file.
             */
            Console.WriteLine(Encrypt(cryptoKey, "[INFO] - " + time() + " | " + text));
        }
        
        // Same as above, but for errors.
        public static void error(string text)
        {
            Console.WriteLine(Encrypt(cryptoKey, "[ERROR] - " + time() + " | " + text));
        }

        /*
         * This is the actual encryption method. Decryption
         * is handled in the "SecurityPlusManager" program
         * (which is also available on the GitHub). It's pretty
         * simple, just encrypts text with the key and the AES algorithm.
         */
        private static string Encrypt(string key, string text)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(text);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }
    }
}