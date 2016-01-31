using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Xml.Serialization;
using Microsoft.CSharp;
using Ploeh.AutoFixture;
using Ploeh.AutoFixture.Kernel;
using PowerArgs;

namespace AutoGenXml
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var arguments = Args.Parse<Arguments>(args);

                var param = new CompilerParameters
                {
                    GenerateExecutable = false,
                    IncludeDebugInformation = false,
                    GenerateInMemory = true
                };
                param.ReferencedAssemblies.Add("System.dll");
                param.ReferencedAssemblies.Add("System.Xml.dll");
                param.ReferencedAssemblies.Add("System.Data.dll");
                param.ReferencedAssemblies.Add("System.Core.dll");
                param.ReferencedAssemblies.Add("System.Xml.Linq.dll");

                //new Dictionary<String, String> {{"CompilerVersion", "v3.5"}}
                var codeProvider = new CSharpCodeProvider();
                var results = codeProvider.CompileAssemblyFromFile(param, arguments.ClassFile);

                if (results.Errors.HasErrors)
                {
                    foreach (var error in results.Errors)
                    {
                        Console.WriteLine(error);
                    }
                }
                else
                {
                    object o = results.CompiledAssembly.CreateInstance(arguments.TypeName);
                    if (o == null)
                    {
                        Console.WriteLine("Unable to instantiate type " + arguments.TypeName);
                    }
                    else
                    {
                        var fixture = new Fixture();

                        var createMethod = typeof (SpecimenFactory).
                            GetMethod("Create", new[] {typeof (ISpecimenBuilder)}).
                            MakeGenericMethod(o.GetType());

                        var populated = createMethod.Invoke(null, new object[] {fixture});

                        var serializer = new XmlSerializer(populated.GetType());
                        var writer = new StreamWriter(arguments.TypeName + ".xml");
                        serializer.Serialize(writer, populated);
                    }
                }

            }
            catch (ArgException e)
            {
                Console.WriteLine(ArgUsage.GenerateUsageFromTemplate<Arguments>());
            }
        }
    }
}
