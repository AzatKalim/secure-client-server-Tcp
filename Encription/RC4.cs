using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Numerics;
using System.Threading;

namespace Encription
{
    public class RC4
    {
        public const int SECURE_LENGTH=100;
        public const int CHAR_SIZE = 16;
        Encoding encoding = Encoding.Default;

        public BigInteger key;
        public BigInteger z;
        public BigInteger field;
        public BigInteger q;

        byte[] S = new byte[256];
        int x = 0;
        int y = 0;
        int numberOfKey = 0;
        byte[] keys = new byte[256];

        public RC4()
        {
            GenerateParms();
        }

        public RC4(BigInteger q,BigInteger field)
        {
            this.q = q;
            this.field = field;
        }

        public RC4(string q, string field)
        {
            this.q = BigInteger.Parse(q);
            this.field = BigInteger.Parse(field);
        }

        public BigInteger GenerateParam()
        {
            z = RandomNum(SECURE_LENGTH-1);
            return BigInteger.ModPow(q, x, field);
        }

        public void SetKey(BigInteger remoteParam)
        {
            key = BigInteger.ModPow(remoteParam, z, field);
            InitSBlock(key.ToByteArray());
        }

        public void SetKey(String remoteParam)
        {
            SetKey(BigInteger.Parse(remoteParam));
        }

        public RC4(byte[] key)
        {
            InitSBlock(key);
        }

        public void InitSBlock(byte[] key)
        {
            byte temp;
            for (int i = 0; i < 256; i++)
            {
                S[i] = (byte)i;
            }
            int j = 0;
            for (int i = 0; i < 256; i++)
            {
                j = (j + S[i] + key[i % key.Length]) % 256;
                temp = S[i];
                S[i] = S[j];
                S[j] = temp;

            }
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = KeyGeneration();
            }
        }

        private byte KeyGeneration()
        {
            byte temp;
            x = (x + 1) % 256;
            y = (y + S[x]) % 256;
            temp = S[y];
            S[y] = S[x];
            S[x] = temp;
            return S[(S[x] + S[y]) % 256];
        }

        private void GenerateParms()
        {
            field = GeneratePrimeNumber(SECURE_LENGTH);
            Thread.Sleep(10);
            q=GeneratePrimeNumber(SECURE_LENGTH-1);
        }

        public string Encode(string message)
        {
            byte[] result = encoding.GetBytes(message);
            return encoding.GetString(Encode(result));
        }

        public String Decode(string message)
        {
            return Encode(message);
        }

        byte[] Encode(byte[] data)
        {
            byte[] cipher = new byte[data.Length];
            for (int m = 0; m < data.Length; m++)
            {
                cipher[m] = (byte)(data[m] ^ keys[numberOfKey]);
                numberOfKey = (numberOfKey + 1) % keys.Length;
            }
            return cipher;
        }

        byte[] Decode(byte[] cipher)
        {
            return Encode(cipher);
        }

        #region Secondary functions 

        public static BigInteger RandomNum(int length)
        {
            var N = BigInteger.Pow(2, length);
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            var random = new Random();
            random.NextBytes(bytes);
            bytes[bytes.Length - 1] &= (byte)0x7F;
            R = new BigInteger(bytes);
            while (R > N - 1)
            {
                R -= N;
            }
            if (R == 1)
            {
                R++;
            }
            return R;
        }
      
        private BigInteger GeneratePrimeNumber(int length = SECURE_LENGTH)
        {
            BigInteger k = 2;
            BigInteger result = RandomBigInteger(BigInteger.Pow(k, length));
            if (result % 2 == 0)
            {
                result++;
            }
            while (!IsPrime(result))
            {
                result += 2;
            }
            return result;
        }

        public bool IsPrime(BigInteger r, uint k = 15)
        {
            if (r < 2)
                return false;
            if (r == 2)
                return true;
            if (r % 2 == 0)
                return false;
            int s = 0;
            BigInteger t = r - 1;
            while (t % 2 == 0 && t != 0)
            {
                t = t / 2;
                s++;
            }
            for (int j = 0; j < k; j++)
            {
                BigInteger a = RandomBigInteger(r - 1);
                if (a <= 1)
                {
                    a = 2;
                }
                if (a == 0 || r % a == 0)
                {
                    return false;
                }
                BigInteger b = BigInteger.ModPow(a, t, r);
                if (b == 1 || b == r - 1)
                {
                    continue;
                }
                int i = 0;
                for (; i < s; i++)
                {
                    b = BigInteger.ModPow(b, 2, r);
                    if (b == r - 1)
                    {
                        break;
                    }
                    if (b == 1)
                    {
                        return false;
                    }
                }
                if (i > s - 1)
                {
                    return false;
                }
            }
            return true;
        }

        private BigInteger RandomBigInteger(BigInteger N)
        {
            byte[] bytes = N.ToByteArray();
            BigInteger R;
            Random random = new Random();
            random.NextBytes(bytes);
            bytes[bytes.Length - 1] &= (byte)0x7F;
            R = new BigInteger(bytes);
            while (R > N - 1)
            {
                R -= N;
            }
            if (R == 1)
            {
                R++;
            }
            return R;
        }

        #endregion
    }
}
