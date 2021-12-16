using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DependencyInjectionContainer
{
    public class ImplementationInfo
    {
        public Type ImplementClassType;
        public LifeTime LifeTime;
        public bool isNullParam;

        public ImplementationInfo(LifeTime lifeTime, Type impl)
        {
            ImplementClassType = impl;
            LifeTime = lifeTime;
            isNullParam = false;
        }

    }
}
