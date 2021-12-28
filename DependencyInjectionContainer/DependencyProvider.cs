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

        public bool hasMock = false;

        private readonly Stack<Type> _recursionStackResolver = new Stack<Type>();




        //словарь тип-объект моков
        public Dictionary<Type, object> mockedObjects = new Dictionary<Type, object>();
        //название типа, объект. Содержит те объекты, которые прошли resolve
        public Dictionary<String, object> resolvedDictionary = new Dictionary<String, object>();
        //список существующих типов (для разрешения моков)
        public List<Type> existingTypes = new List<Type>();


        public DependencyProvider(DependenciesConfiguration configuration)
        {
            _configuration = configuration;
        }

        public TDependency Resolve<TDependency>()
        {
            var resolved = (TDependency)Resolve(typeof(TDependency));
            Type resolvedType = null;
            if (resolved == null)
                return default(TDependency);
            resolvedType = resolved.GetType();
            var interfaces = resolved.GetType().GetInterfaces();
            PropertyInfo[] propertyInfos = resolved.GetType().GetProperties();
            Console.WriteLine("current object: " + resolved); ;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                Console.WriteLine("fieldName: " + propertyInfos[i].Name + " fieldType: " + propertyInfos[i].PropertyType);
                //                Console.WriteLine("value = " + propertyInfos[i].GetValue(resolved));
            }
            Console.WriteLine();

            if (mockedObjects.Count != 0 && resolvedDictionary.Count != 0)
            {

                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].GetValue(resolved).ToString().StartsWith("Mock"))
                    {
                        //Fist : SecondInterface, SecondClass


                        String tmp = propertyInfos[i].PropertyType.ToString();
                        if (resolvedDictionary.ContainsKey(tmp))
                        {
                            Console.WriteLine("Before resolving mock in class " + resolved.ToString() + ": " + propertyInfos[i].PropertyType.ToString() + " " + propertyInfos[i].Name + " = " + propertyInfos[i].GetValue(resolved).ToString());

                            propertyInfos[i].SetValue(resolved, resolvedDictionary[propertyInfos[i].PropertyType.ToString()]);

                            Console.WriteLine("After resolving mock in class  " + resolved.ToString() + ": " + propertyInfos[i].PropertyType.ToString() + " " + propertyInfos[i].Name + " = " + propertyInfos[i].GetValue(resolved).ToString());
                        }
                    }
                }
            }
            try
            {
                //resolvedDictionary.Add(resolvedType.ToString(), resolved);
                resolvedDictionary.Add(interfaces[0].ToString(), resolved);

            }
            catch
            {

            }

            Console.WriteLine("\nresolved " + resolved.ToString() + "\n");
            return resolved;
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

            if (!existingTypes.Contains(t))
                existingTypes.Add(t);

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
                        //iSecond
                        object obj = Resolve(parameter.ParameterType);


                        Type parameterType = parameter.ParameterType;

                        if (obj == null && existingTypes.Contains(parameterType))
                        {
                            Mock mock;
                            var type = typeof(Mock<>).MakeGenericType(parameterType);
                            mock = (Mock)Activator.CreateInstance(type);
                            obj = mock.Object;
                            mockedObjects.Add(parameter.ParameterType, obj);
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
                catch (Exception e)
                {
                    Console.WriteLine(e.StackTrace);
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
