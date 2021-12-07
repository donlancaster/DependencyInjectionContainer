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
    }
}
