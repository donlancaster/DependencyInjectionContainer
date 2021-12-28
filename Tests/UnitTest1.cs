using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionContainer;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void SimpleDependencyTest()
        {
            //ismth
            DependenciesConfiguration configuration = new DependenciesConfiguration();

            configuration.Register<ISmth, ISmthImpl>(LifeTime.Singleton);
            DependencyProvider provider = new DependencyProvider(configuration);

            ISmthImpl cl = (ISmthImpl)provider.Resolve<ISmth>();
            Assert.IsNotNull(cl, configuration.ToString() + " " + provider.ToString() + " ");
        }


        [TestMethod]
        public void DifferentLifeTimeTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<ISmth, ISmthImpl>(LifeTime.Singleton);
            configuration.Register<IService, FirstIServiceImpl>();
            DependencyProvider provider = new DependencyProvider(configuration);

            ISmthImpl cl1 = (ISmthImpl)provider.Resolve<ISmth>();
            ISmthImpl cl2 = (ISmthImpl)provider.Resolve<ISmth>();
            Assert.AreEqual(cl1, cl2);
            IService s1 = provider.Resolve<IService>();
            IService s2 = provider.Resolve<IService>();
            Assert.AreNotEqual(s1, s2);
        }


        [TestMethod]
        public void ManyImplementationsTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<IService, FirstIServiceImpl>();
            configuration.Register<IService, SecondIServiceImpl>();
            DependencyProvider provider = new DependencyProvider(configuration);
            IEnumerable<IService> impls = provider.Resolve<IEnumerable<IService>>();
            Assert.IsNotNull(impls);
            Assert.AreEqual(2, ((List<IService>)impls).Count);
        }


        [TestMethod]
        public void InnerDependencyTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<ISmth, ISmthImpl>();
            configuration.Register<IService, FirstIServiceImpl>();
            configuration.Register<IService, SecondIServiceImpl>();
            configuration.Register<IClient, SecondIClientImpl>();
            DependencyProvider provider = new DependencyProvider(configuration);

            FirstIServiceImpl cl1 = (FirstIServiceImpl)provider.Resolve<IService>();
            Assert.IsNotNull(cl1.Smth);

            SecondIClientImpl cl2 = (SecondIClientImpl)provider.Resolve<IClient>();
            Assert.IsNotNull(cl2.Serv);
            Assert.AreEqual(2, ((List<IService>)cl2.Serv).Count);
        }

        [TestMethod]
        public void SimpleRecursionTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<IClient, FirstIClientImpl>();
            configuration.Register<IData, IDataImpl>();
            DependencyProvider provider = new DependencyProvider(configuration);

            FirstIClientImpl client = (FirstIClientImpl)provider.Resolve<IClient>();
            Assert.IsNotNull(((IDataImpl)client.Data).Cl);
        }


        [TestMethod]
        public void SimpleOpenGenericTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<IAnother<ISmth>, First<ISmth>>();
            configuration.Register(typeof(IFoo<>), typeof(Second<>));
            DependencyProvider provider = new DependencyProvider(configuration);

            IAnother<ISmth> cl1 = provider.Resolve<IAnother<ISmth>>();
            Assert.IsNotNull(cl1);

            IFoo<IService> cl2 = provider.Resolve<IFoo<IService>>();
            Assert.IsNotNull(cl2);
        }

        [TestMethod]
        public void SameClassTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<HumanImpl, HumanImpl>();
            DependencyProvider provider = new DependencyProvider(configuration);
            HumanImpl humanImpl = provider.Resolve<HumanImpl>();
            Assert.IsNotNull(humanImpl);
        }


        [TestMethod]
        public void TwoSingletonParametersDependencies()
        {
            var config = new DependenciesConfiguration();
            config.Register<IA, A>(LifeTime.Singleton);
            config.Register<IB, B>(LifeTime.Singleton);
            DependencyProvider provider = new DependencyProvider(config);
            B b = (B)provider.Resolve<IB>();
            A a = (A)provider.Resolve<IA>();
            Assert.AreEqual(b.a, a);
            Assert.AreEqual(b, a.b);
            A a1 = (A)b.a;
            Assert.AreEqual(a1.b, b);
        }



        [TestMethod]
        public void ThreeSingletonParametersDependencies()
        {
            var config = new DependenciesConfiguration();
            config.Register<FirstInterface, FirstClass>(LifeTime.Singleton);
            config.Register<SecondInterface, SecondClass>(LifeTime.Singleton);
            config.Register<ThirdInterface, ThirdClass>(LifeTime.Singleton);
            DependencyProvider provider = new DependencyProvider(config);
            FirstClass first = (FirstClass)provider.Resolve<FirstInterface>();
            SecondClass second = (SecondClass)provider.Resolve<SecondInterface>();
            ThirdClass third = (ThirdClass)provider.Resolve<ThirdInterface>();
            ThirdClass thirdTmp = (ThirdClass)second.third;
            Assert.AreEqual(first.second, second);
            Assert.AreEqual(second.third, third);
            Assert.AreEqual(third.first,first);
            Assert.AreEqual(thirdTmp.first, first);
        }
    }

}






    








  