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
            return null;
        }

    }
}
