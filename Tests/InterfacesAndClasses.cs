using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{

    interface ISmth
    {
    }

    class ISmthImpl : ISmth
    {
    }


    interface IService
    {
    }

    class FirstIServiceImpl : IService
    {
        public readonly ISmth Smth;

        public FirstIServiceImpl()
        {
        }

        public FirstIServiceImpl(ISmth smth)
        {
            Smth = smth;
        }
    }

    class SecondIServiceImpl : IService
    {
    }



    interface IData
    {
    }

    class IDataImpl : IData
    {
        public readonly IClient Cl;

        public IDataImpl(IClient cl)
        {
            Cl = cl;
        }
    }



    interface IClient
    {
    }

    class FirstIClientImpl : IClient
    {
        public readonly IData Data;

        public FirstIClientImpl(IData data)
        {
            Data = data;
        }
    }


    class SecondIClientImpl : IClient
    {
        public readonly IEnumerable<IService> Serv;

        public SecondIClientImpl(IEnumerable<IService> serv)
        {
            Serv = serv;
        }
    }



}
