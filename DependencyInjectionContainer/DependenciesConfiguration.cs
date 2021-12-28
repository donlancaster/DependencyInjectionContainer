using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{


    public enum LifeTime
    {
        Singleton, InstancePerDependency
    }


   public class DependenciesConfiguration
    {

        public readonly Dictionary<Type, List<ImplementationInfo>> RegisteredDependencies;

        public DependenciesConfiguration()
        {
            RegisteredDependencies = new Dictionary<Type, List<ImplementationInfo>>();
        }

        

        public void Register(Type interfaceType, Type classType, LifeTime lifeTime = LifeTime.InstancePerDependency)
        {
            if ((!interfaceType.IsInterface && interfaceType != classType) || classType.IsAbstract || !interfaceType.IsAssignableFrom(classType) && !interfaceType.IsGenericTypeDefinition)
                throw new Exception("oshibka");


            if (!RegisteredDependencies.ContainsKey(interfaceType))
            {
                List<ImplementationInfo> impl = new List<ImplementationInfo> { new ImplementationInfo(lifeTime, classType) };
                RegisteredDependencies.Add(interfaceType, impl);
            }
            else
            {
                RegisteredDependencies[interfaceType].Add(new ImplementationInfo(lifeTime, classType));
            }


        }



        public void Register<TDependency, TImplementation>(LifeTime lifeTime = LifeTime.InstancePerDependency)
        {
            Register(typeof(TDependency), typeof(TImplementation), lifeTime);
        }


    }
}
