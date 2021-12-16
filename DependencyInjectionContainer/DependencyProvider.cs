using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Moq;

namespace DependencyInjectionContainer
{
    public class DependencyProvider
    {
        private readonly DependenciesConfiguration _configuration;

        public readonly ConcurrentDictionary<Type, object> _singletonImplementations = new ConcurrentDictionary<Type, object>();



        private readonly Stack<Type> _recursionStackResolver = new Stack<Type>();

        //

        public List<object> mocked = new List<object>();



        public DependencyProvider(DependenciesConfiguration configuration)
        {
            _configuration = configuration;
        }


        public TDependency Resolve<TDependency>()
        {
            return (TDependency)Resolve(typeof(TDependency));
        }

        public TDependency ResolveMocked<TDependency>()
        {
            return (TDependency)ResolveMocked(typeof(TDependency));
        }

        private object ResolveMocked(Type type)
        {
            List<ImplementationInfo> infos = GetImplementationsInfos(type);

            object obj = GetImplementation(infos[0], type);
            return obj;

        }

        private object Resolve(Type t)
        {
            Type dependencyType = t;
            List<ImplementationInfo> infos = GetImplementationsInfos(dependencyType);


            if (infos == null && t.GetGenericTypeDefinition() != typeof(IEnumerable<>))
                throw new Exception("Unregistered dependency");



            if (_recursionStackResolver.Contains(t))
            {

                return null;

            }

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


            object obj = GetImplementation(infos[0], t);

            _recursionStackResolver.Pop();
            return obj;
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
            //если class<T>, это genericTypeDefinition
            //если class<int>, это genericType 
            Type innerTypeForOpenGeneric = null;
            if (implementInfo.ImplementClassType.IsGenericType && implementInfo.ImplementClassType.IsGenericTypeDefinition &&
                implementInfo.ImplementClassType.GetGenericArguments()[0].FullName == null)
                innerTypeForOpenGeneric = resolvingDependencyType.GetGenericArguments().FirstOrDefault(); //getGenericTypeAdgs[0]
            //
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
        //List<List<List<int>>>
        private object CreateInstanceForDependency(Type implementClassType, Type innerTypeForOpenGeneric)
        {
            ConstructorInfo[] constructors = implementClassType.GetConstructors()
                 .OrderByDescending(x => x.GetParameters().Length).ToArray();
            object implInstance = null;
            foreach (ConstructorInfo constructor in constructors)
            {
                ParameterInfo[] parameters = constructor.GetParameters();
                List<object> paramsValues = new List<object>();
                foreach (ParameterInfo parameter in parameters)
                {
                    if (IsDependency(parameter.ParameterType))
                    {
                        //нашел А
                        object obj = Resolve(parameter.ParameterType);
                        if (obj == null)
                        {
                            var mockA = new Mock<IA>();
                            obj = mockA.Object;
                           // mocked.Add(obj);

                        }
                        paramsValues.Add(obj);
                    }
                    else
                    {
                        object obj = null;
                        try
                        {
                            obj = Activator.CreateInstance(parameter.ParameterType, null);
                        }
                        catch
                        {
                            // ignored
                        }

                        paramsValues.Add(obj);
                    }
                }

                try
                {
                    if (innerTypeForOpenGeneric != null)
                        //<T>
                        implementClassType = implementClassType.MakeGenericType(innerTypeForOpenGeneric);
                    //Console.WriteLine(innerTypeForOpenGeneric.ToString());
                    implInstance = Activator.CreateInstance(implementClassType, paramsValues.ToArray());
                    break;
                }
                catch
                {
                    // ignored
                }
            }

            return implInstance;


        }





        private object ConvertToIEnumerable(List<object> implementations, Type t)
        {
            var enumerableType = typeof(Enumerable);
            var castMethod = enumerableType.GetMethod(nameof(Enumerable.Cast))?.MakeGenericMethod(t);
            var toListMethod = enumerableType.GetMethod(nameof(Enumerable.ToList))?.MakeGenericMethod(t);

            IEnumerable<object> itemsToCast = implementations;

            var castedItems = castMethod?.Invoke(null, new[] { itemsToCast });

            return toListMethod?.Invoke(null, new[] { castedItems });
        }


    }
}
