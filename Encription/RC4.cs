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
        public const int SECURE_LENGTH=15;

        public BigInteger key;
        public BigInteger x;
        public BigInteger field;
        public BigInteger q;

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
            x = RandomNum(SECURE_LENGTH-1);
            return BigInteger.ModPow(q, x, field);
        }

        public void SetKey(BigInteger remoteParam)
        {
            key = BigInteger.ModPow(remoteParam, x, field);
        }
        public void SetKey(String remoteParam)
        {
            SetKey(BigInteger.Parse(remoteParam));
        }

        private void GenerateParms()
        {
            field = RandomNum(SECURE_LENGTH);
            Thread.Sleep(10);
            q= RandomNum(SECURE_LENGTH);
        }

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
    }
}
