using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeDom
{
    class Program
    {
        static void Main(string[] args)
        {
            CodeDom dom = new CodeDom();
            dom.GenerateCSharpCode("SampleCode.cs");
            if (dom.CompileAutoGenCode())
            {
                //
            }
            Console.ReadLine();
        }
    }
}
