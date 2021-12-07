using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DependencyInjectionContainer;
namespace Tests
{

   
    class ContainerTests
    {
        [TestMethod]
        public void SimpleDependencyTest()
        {
            DependenciesConfiguration configuration = new DependenciesConfiguration();
            configuration.Register<ISmth, ISmthImpl>(LifeTime.Singleton);
            DependencyProvider provider = new DependencyProvider(configuration);

            ISmthImpl cl = (ISmthImpl)provider.Resolve<ISmth>();
            Assert.IsNotNull(cl);
        }
    }
}
