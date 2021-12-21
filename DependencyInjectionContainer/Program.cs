using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using DependencyInjectionContainer;
namespace DependencyInjectionContainer
{
    class Program
    {

        static void Main(string[] args)
        {
            var config = new DependenciesConfiguration();
            config.Register<IA, A>(LifeTime.Singleton);
            config.Register<IB, B>(LifeTime.Singleton);

            DependencyProvider provider = new DependencyProvider(config);

            B b = (B)provider.Resolve<IB>();

            b.a = (A)provider.Resolve<IA>();

            //B
            Console.WriteLine("=== obejct B ===");
            PropertyInfo[] propertyInfosB = b.GetType().GetProperties();
            for (int i = 0; i < propertyInfosB.Length; i++)
            {
                Console.WriteLine("fieldName: " + propertyInfosB[i].Name + " | fieldType: " + propertyInfosB[i].PropertyType.FullName);
                Console.WriteLine("value = " + propertyInfosB[i].GetValue(b));
            }

            //A
            Console.WriteLine("\n\n=== object A ===");
            PropertyInfo[] propertyInfosA = b.a.GetType().GetProperties();
            for (int i = 0; i < propertyInfosA.Length; i++)
            {
                Console.WriteLine("fieldName: " + propertyInfosA[i].Name + " | fieldType: " + propertyInfosA[i].PropertyType);
                Console.WriteLine("value = " + propertyInfosA[i].GetValue(b.a));
            }
        }
    }
}
    public interface IB
    {
        public void methodB();
    }


    public class B : IB
    {
        public void methodB()
        {
            Console.WriteLine("method b");
        }
        public IA a { get; set; }
        public B(IA ia)
        {
            this.a = ia;
            Console.WriteLine("B constructor: gets " + ia.ToString()+"\n");
        }
    }

    public interface IA
    {
 
    }

    public class A : IA
    {
        private int valll;
        public IB b { get; set; }
        public A(IB ib, int val)
        {
            valll = val;
            this.b = ib;
            Console.WriteLine("A constructor: gets " + ib.ToString());
        }
    }
