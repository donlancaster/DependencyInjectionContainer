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



}
