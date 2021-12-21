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
            Assert.IsNull(((IDataImpl)client.Data).Cl);
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
        public void AtoBtoATest()
        {
            var config = new DependenciesConfiguration();
            config.Register<IA, A>(LifeTime.Singleton);
            config.Register<IB, B>(LifeTime.Singleton);
            DependencyProvider provider = new DependencyProvider(config);
            B b = (B)provider.Resolve<IB>();
            Assert.IsNotNull(b);
            PropertyInfo[] propertyInfos = b.a.GetType().GetProperties();
            Assert.IsTrue(propertyInfos[0].GetValue(b.a).ToString().Contains("Mock"), b.a.ToString());
            b.a = (A)provider.Resolve<IA>();
            Assert.IsNotNull(b.a, b.GetType() + " " + b.a.GetType());
            propertyInfos = b.a.GetType().GetProperties();
            Assert.IsFalse(propertyInfos[0].GetValue(b.a).ToString().Contains("Mock"));

        }
    }

}






    








  