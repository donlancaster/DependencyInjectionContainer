﻿using System;
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

        //



        public Dictionary<Type, object> mockedWType = new Dictionary<Type, object>();

        public Dictionary<String, object> resolvedDictionary = new Dictionary<String, object>();

        public List<object> resolvedList = new List<object>();

        public DependencyProvider(DependenciesConfiguration configuration)
        {
            _configuration = configuration;
        }



        /*    public TDependency Resolve<TDependency>()
            {
                var resolved = (TDependency)Resolve(typeof(TDependency));
                var resolvedType = resolved.GetType();
                Console.WriteLine("resolved " + resolved);
                Console.WriteLine("resolved type " + resolved);
                Console.WriteLine("mockedDictionary count " + mockedDictionary.Count);
                Console.WriteLine("mockedDictionary " + mockedDictionary.ToString());
                if (mockedDictionary.ContainsKey(resolved))
                {
                    var mockedParameter = mockedDictionary[resolved];
                    ConstructorInfo[] constructors = mockedParameter.GetType().GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
                    object resolvedMock = null;
                    foreach (ConstructorInfo constructor in constructors)
                    {
                        ParameterInfo[] parameters = constructor.GetParameters();
                        List<object> paramsValues = new List<object>();
                        foreach (ParameterInfo parameter in parameters)
                        {
                            if (parameter.GetType() == resolvedType)
                            {
                                paramsValues.Add(resolved);
                                continue;
                            }
                            if (IsDependency(parameter.ParameterType))
                            {

                                object obj = Resolve(parameter.ParameterType);

                                if (obj == null)
                                {
                                    var mockA = new Mock<IA>();
                                    obj = mockA.Object;
                                    mockedDictionary.Add(mockedParameter,obj);

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
                        resolvedMock = Activator.CreateInstance(mockedDictionary[resolved].GetType(), paramsValues.ToArray());
                        Console.WriteLine(resolvedMock.ToString());
                        resolvedMockedDictionary.Add(resolved, resolvedMock);
                        mockedDictionary.Remove(resolved);
                    }


                    Console.WriteLine(resolvedMockedDictionary.Count);
                    Console.WriteLine(resolvedMockedDictionary.ToString());


                    if (resolvedMockedDictionary.Count != 0)
                    {
                        foreach (var item in resolvedMockedDictionary)
                        {
                            var type = item.Key;
                            var newParam = item.Value;


                        }
                    }



                }
                return resolved;
            }
    */


        /*        public TDependency Resolve<TDependency>()
                {
                    var resolved = (TDependency)Resolve(typeof(TDependency));
                    var resolvedType = resolved.GetType();
                    Console.WriteLine("resolved: " + resolved.ToString() + "| type: " + resolvedType);
                    Console.WriteLine("mocked list size: "+mocked.Count);

                    if (mocked.Count != 0)
                    {
                        for (int i = 0; i < mocked.Count; i++)
                        {
                            Console.WriteLine(mocked[i].ToString());
                            ConstructorInfo[] constructors = mocked[i].GetType().GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
                            object resolvedMock = null;
                            foreach (ConstructorInfo constructor in constructors)
                            {
                                ParameterInfo[] parameters = constructor.GetParameters();
                                List<object> paramsValues = new List<object>();
                                foreach (ParameterInfo parameter in parameters)
                                {

                                    if (parameter.GetType() == resolvedType)
                                    {
                                        paramsValues.Add(resolved);
                                        continue;
                                    }

                                    if (IsDependency(parameter.ParameterType))
                                    {
                                        object obj = Resolve(parameter.ParameterType);
                                        if (obj == null)
                                        {
                                            var mockA = new Mock<IA>();
                                            obj = mockA.Object;
                                            mocked.Add(obj);
                                            mockedWType.Add(obj, parameter.ParameterType);
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
                                resolvedMock = constructor.Invoke(paramsValues.ToArray());
                               // resolvedMock = Activator.CreateInstance(mocked[i].GetType(), paramsValues.ToArray());
                                Console.WriteLine(resolvedMock.ToString());
                            }
                        }
                    }
                    return resolved;
                }
        */
        /*   if (mockedWType.Count != 0)
              {
                  object resolvedMock = null;

                  foreach (var item in mockedWType)
                  {
                      object mockedObj = item.Key;
                      Type mockedObjType = item.Value;
                      Console.WriteLine("mock object: " + item.Key + " | type: " + item.Value);

                      ConstructorInfo[] constructors = mockedObjType.GetConstructors().OrderByDescending(x => x.GetParameters().Length).ToArray();
                      Console.WriteLine(constructors.ToString());
                      foreach (ConstructorInfo constructor in constructors)
                      {
                          ParameterInfo[] parameters = constructor.GetParameters();
                          Console.WriteLine("параметры конструктора для "+ mockedObj.ToString() +" "+parameters.ToString());
                          List<object> paramsValues = new List<object>();
                          foreach (ParameterInfo parameter in parameters)
                          {
                              Console.WriteLine("parameter: " + parameter.Name + " | for: " + constructor.GetType());
                              if (parameter.GetType() == resolvedType)
                              {
                                  paramsValues.Add(resolved);
                                  Console.WriteLine("нашел " + parameter.GetType() + " ");
                                  continue;
                              }



                              if (IsDependency(parameter.ParameterType))
                              {
                                  object obj = Resolve(parameter.ParameterType);
                                  Console.WriteLine("объект который мы взяли из диктионари короче " + obj.ToString());
                                  if (obj == null)
                                  {
                                      Console.WriteLine("mock: " + parameter.ParameterType.Name + " | ");
                                      Type p = parameter.ParameterType.GetInterfaces()[0];
                                      var mockA = new Mock<IA>();
                                      obj = mockA.Object;
                                      Console.WriteLine("замокал "+ obj.ToString());
                                      mockedWType.Add(obj, parameter.ParameterType);
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
                          resolvedMock = constructor.Invoke(paramsValues.ToArray());
                          Console.WriteLine("resolvedMock : "+resolvedMock.ToString());
                      }




                  }

              }*/



        public TDependency Resolve<TDependency>()
        {
            var resolved = (TDependency)Resolve(typeof(TDependency));
            Type resolvedType = null;
            if (resolved == null)
                return default(TDependency);
                resolvedType = resolved.GetType();


            PropertyInfo[] propertyInfos = resolved.GetType().GetProperties();
            Console.WriteLine("current object: " + resolved); ;
            for (int i = 0; i < propertyInfos.Length; i++)
            {
                Console.WriteLine("field name = " + propertyInfos[i].Name + " field type = " + propertyInfos[i].PropertyType);
                //                Console.WriteLine("value = " + propertyInfos[i].GetValue(resolved));
            }
            Console.WriteLine();

            if (mockedWType.Count != 0 && resolvedDictionary.Count != 0)
            {


                for (int i = 0; i < propertyInfos.Length; i++)
                {
                    if (propertyInfos[i].GetValue(resolved).ToString().StartsWith("Mock"))
                    {
                        String tmp = propertyInfos[i].PropertyType.ToString();

                        if (resolvedDictionary.ContainsKey(tmp.Substring(1)))
                        {
                            Console.WriteLine("Before resolving mock in class " + resolved.ToString() + ": " + propertyInfos[i].PropertyType.ToString() + " " + propertyInfos[i].Name + " = " + propertyInfos[i].GetValue(resolved).ToString());

                            propertyInfos[i].SetValue(resolved, resolvedDictionary[propertyInfos[i].PropertyType.ToString().Substring(1)]);

                            Console.WriteLine("After resolving mock in class  " + resolved.ToString() + ": " + propertyInfos[i].PropertyType.ToString() + " " + propertyInfos[i].Name + " = " + propertyInfos[i].GetValue(resolved).ToString());
                        }

                    }


                }
            }
            try
            {
                resolvedDictionary.Add(resolvedType.ToString(), resolved);
            }
            catch
            {

            }
            resolvedList.Add(resolved);
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
                        if (obj == null && ((parameter.ParameterType.Name.Equals("IA") || (parameter.ParameterType.Name.Equals("IB")))))
                        {
                            Mock mock;
                            if (parameter.ParameterType.Name.Equals("IA"))
                                mock = new Mock<IA>();
                            else mock = new Mock<IB>();
                            obj = mock.Object;
                            this.hasMock = true;
                            // mockedDictionary.Add(implInstance, obj);
                            //mocked.Add(obj);
                            mockedWType.Add(parameter.ParameterType, obj);

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
