using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    class DependencyProvider
    {
        private readonly DependenciesConfiguration _configuration;

        private readonly ConcurrentDictionary<Type, object> _singletonImplementations =
          new ConcurrentDictionary<Type, object>();




        private readonly Stack<Type> _recursionStackResolver = new Stack<Type>();

        public DependencyProvider(DependenciesConfiguration configuration)
        {
            _configuration = configuration;
        }


        private object Resolve(Type t)
        {
            Type dependencyType = t;
            List<ImplementationInfo> infos = GetImplementationsInfos(dependencyType);


            if (infos == null && t.GetGenericTypeDefinition() != typeof(IEnumerable<>))
                throw new Exception("Unregistered dependency");


            if (_recursionStackResolver.Contains(t))
                return null;

            _recursionStackResolver.Push(t);

            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                dependencyType = t.GetGenericArguments()[0];
                infos = GetImplementationsInfos(dependencyType);
                if (infos == null) throw new Exception("Unregistered dependency");
                List<object> implementations = new List<object>();
                foreach (ImplementationInfo info in infos)
                {
                    implementations.Add(GetImplementation(info, t));
                }

                return ConvertToIEnumerable(implementations, dependencyType);



            }


            return null;
        }

        private List<ImplementationInfo> GetImplementationsInfos(Type dependencyType)
        {
            if (_configuration.RegisteredDependencies.ContainsKey(dependencyType))
                return _configuration.RegisteredDependencies[dependencyType];


            if (!dependencyType.IsGenericType) return null;



            Type definition = dependencyType.GetGenericTypeDefinition();
            return _configuration.RegisteredDependencies.ContainsKey(definition)
                ? _configuration.RegisteredDependencies[definition]
                : null;

        }





        private bool IsDependency(Type t)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return IsDependency(t.GetGenericArguments()[0]);

            return _configuration.RegisteredDependencies.ContainsKey(t);
        }



        private object GetImplementation(ImplementationInfo implementInfo, Type resolvingDependencyType)
        {
            Type innerTypeForOpenGeneric = null;
            if (implementInfo.ImplementClassType.IsGenericType && implementInfo.ImplementClassType.IsGenericTypeDefinition &&
                implementInfo.ImplementClassType.GetGenericArguments()[0].FullName == null)
                innerTypeForOpenGeneric = resolvingDependencyType.GetGenericArguments().FirstOrDefault();

            if (implementInfo.LifeTime == LifeTime.Singleton)
            {
                if (!_singletonImplementations.ContainsKey(implementInfo.ImplementClassType))
                {
                    object singleton = CreateInstanceForDependency(implementInfo.ImplementClassType, innerTypeForOpenGeneric);
                    _singletonImplementations.TryAdd(implementInfo.ImplementClassType, singleton);
                }

                return _singletonImplementations[implementInfo.ImplementClassType];
            }

            return CreateInstanceForDependency(implementInfo.ImplementClassType, innerTypeForOpenGeneric);
        }

        private object CreateInstanceForDependency(Type implClassType, Type innerTypeForOpenGeneric)
        {
            return null;
        }




        private object ConvertToIEnumerable(List<object> implementations, Type t)
        {
            return null;
        }


    }
}
