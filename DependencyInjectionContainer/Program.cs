using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        //    A a = (A)provider.Resolve<IA>();
            B b = (B)provider.Resolve<IB>();



            b.a = provider.ResolveMocked<IA>();

       
           
            

            Console.WriteLine(b.ToString());
            Console.WriteLine(b.a.ToString());
           
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
        Console.WriteLine("method b") ;
    }
    public IA a { get; set; }
    public B(IA ia)
    {
        this.a = ia;
        Console.WriteLine("b constructor " + ia.ToString());
    }
}

public interface IA
{
 //   public void setB(IB b);

}

public class A :IA
{
    private int valll;



    public  IB b { get; set; }
    public A(IB ib, int val)
    {
        valll = val;
        this.b = ib;
        Console.WriteLine("a constructor " + ib.ToString());
    }
}