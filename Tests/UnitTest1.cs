using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionContainer;

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





    }
}
