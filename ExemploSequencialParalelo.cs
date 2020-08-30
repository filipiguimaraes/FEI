using System;
using System.Collections;
using System.Threading;
using System.Diagnostics;

namespace ExemploSequencialParalelo
{
    class Program
    {
        private static readonly int _QuantidadeNumerosPrimos = 222;
        private static readonly int _QuantidadeThreads =2;
        private static readonly int _BBB = 170;
        private static readonly int _CCC = 193;
        private static long _Soma = 0;
        private static ArrayList _NumerosPrimos;

        static void Main(string[] args)
        {
            _NumerosPrimos = new ArrayList(_QuantidadeNumerosPrimos);
            Calculo c = new Calculo(_NumerosPrimos, _QuantidadeNumerosPrimos, _QuantidadeThreads, _BBB, _CCC);
            var watch = Stopwatch.StartNew();
            //c.CalcularSequencial();
            c.CalcularParaleloAsync();
            watch.Stop();

            _NumerosPrimos.Sort();
            for (int i = 0; i < _QuantidadeNumerosPrimos; i++)
            {
                Console.WriteLine(string.Format("{1}", i, _NumerosPrimos[i]));
                _Soma += (int)_NumerosPrimos[i];
            }

            Console.WriteLine(string.Format("A soma total dos números primo é: {0}", _Soma));
            Console.WriteLine(string.Format("O tempo de execução no algoritmo paralelo é de {0}ms", watch.ElapsedMilliseconds));
        }
    }

    class Calculo
    {
        private readonly int _QuantidadeNumerosPrimos;
        private readonly int _QuantidadeThreads;
        private static int _ValorReferencia = 0;
        private readonly ArrayList _NumerosPrimos;
        private readonly bool _Menor;
        public Calculo(ArrayList numerosPrimos, int quantidadeNumerosPrimos, int quantidadeThreads, int bbb, int ccc)
        {
            _QuantidadeNumerosPrimos = quantidadeNumerosPrimos;
            _QuantidadeThreads = quantidadeThreads;
            _ValorReferencia = bbb * ccc;
            _NumerosPrimos = numerosPrimos;
            _Menor = _ValorReferencia < 5000;
        }


        public ArrayList CalcularSequencial()
        {
            int x = _ValorReferencia;
            while (_NumerosPrimos.Count < _QuantidadeNumerosPrimos)
            {
                if (_Menor) x++;
                else x--;
                if (TestaPrimo3(x)) _NumerosPrimos.Add(x);
            }

            return _NumerosPrimos;
        }

        public void CalcularParaleloAsync()
        {
            int[] vValorRef = new int[_QuantidadeThreads + 1];
            Thread[] threads = new Thread[_QuantidadeThreads];

            vValorRef[0] = _ValorReferencia;
            while (_NumerosPrimos.Count < _QuantidadeNumerosPrimos)
            {
                for (int i = 1; i < _QuantidadeThreads; i++)
                {
                    if (_Menor) vValorRef[i] = vValorRef[i - 1] + taxaLimite;
                    else vValorRef[i] = vValorRef[i - 1] - taxaLimite;

                }

                for (int i = 0; i < _QuantidadeThreads; i++)
                {
                    threads[i] = new Thread(() => TestaPrimoParalelo(vValorRef[i]));
                    threads[i].Start();
                    threads[i].Join();
                }

                if (_Menor) vValorRef[0] = vValorRef[_QuantidadeThreads - 1] + taxaLimite;
                else vValorRef[0] = vValorRef[_QuantidadeThreads - 1] - taxaLimite;

            }
        }
        int taxaLimite = 15;
        public void TestaPrimoParalelo(int n)
        {
            int limite;
            if (_Menor) limite = n + taxaLimite;
            else limite = n - taxaLimite;

            while (n != limite && _NumerosPrimos.Count < _QuantidadeNumerosPrimos)
            {
                if (TestaPrimo3(n)) lock (_NumerosPrimos) _NumerosPrimos.Add(n);

                if (_Menor) n++;
                else n--;
            }
        }

        bool TestaPrimo3(int n)
        {
            bool EhPrimo;
            int d = 3;
            if (n <= 1 || (n != 2 && n % 2 == 0))
                EhPrimo = false;
            else
                EhPrimo = true;

            while (EhPrimo && d <= n / 2)
            {
                if (n % d == 0)
                    EhPrimo = false;
                d = d + 2;
            }
            return EhPrimo;
        }
    }
}
