using System;
using System.IO;
using System.Text;

namespace APGMPCSSIntegration.Constant.Helpers.Services
{
    public class RC5DataEncryption
    {
        private const int N = 256;
        private static int[] sbox;
        private string password;
        private string text;

        public RC5DataEncryption()
        {
        }

        public RC5DataEncryption(string password, string text)
        {
            this.password = password;
            this.text = text;
        }

        public RC5DataEncryption(string password)
        {
            this.password = password;
        }

        public static string EncryptData(string text)
        {
            RC5DataEncryption rc5 = new RC5DataEncryption("APG@321#AMWAL", text);
            return (StrToHexStr(rc5.EnDeCrypt()));
        }
        public static string DecryptData(string text)
        {
            RC5DataEncryption rc5 = new RC5DataEncryption("APG@321#AMWAL", HexStrToStr(text));
            return (rc5.EnDeCrypt());
        }


        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public string Password
        {
            get { return password; }
            set { password = value; }
        }

        public string EnDeCrypt()
        {
            RC5Initialize();

            int i = 0, j = 0, k = 0;
            StringBuilder cipher = new StringBuilder();
            for (int a = 0; a < text.Length; a++)
            {
                i = (i + 1) % N;
                j = (j + sbox[i]) % N;
                int tempSwap = sbox[i];
                sbox[i] = sbox[j];
                sbox[j] = tempSwap;

                k = sbox[(sbox[i] + sbox[j]) % N];
                int cipherBy = ((int)text[a]) ^ k;  //xor operation
                cipher.Append(Convert.ToChar(cipherBy));
            }
            return cipher.ToString();
        }

        public static string StrToHexStr(string str)
        {
            var bytes = Encoding.UTF8.GetBytes(str);
            var hex = ByteArrayToString(bytes); ;
            return hex;
        }

        public static string HexStrToStr(string hexStr)
        {
            var bytes = StringToByteArray(hexStr);
            var text = Encoding.UTF8.GetString(bytes);

            return text;
        }

        public static byte[] StringToByteArray(String hex)
        {
            hex = hex.ToLower().Replace("0x", "").Replace(" ", "").Replace("-", "").Replace("\r", "").Replace("\n", "");
            int NumberChars = hex.Length / 2;
            byte[] bytes = new byte[NumberChars];
            using (var sr = new StringReader(hex))
            {
                for (int i = 0; i < NumberChars; i++)
                    bytes[i] =
                      Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return bytes;
        }

        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        private void RC5Initialize()
        {
            sbox = new int[N];
            int[] key = new int[N];
            int n = password.Length;
            for (int a = 0; a < N; a++)
            {
                key[a] = (int)password[a % n];
                sbox[a] = a;
            }

            int b = 0;
            for (int a = 0; a < N; a++)
            {
                b = (b + sbox[a] + key[a]) % N;
                int tempSwap = sbox[a];
                sbox[a] = sbox[b];
                sbox[b] = tempSwap;
            }
        }
    }
}
