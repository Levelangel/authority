using System;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using System.Reflection;

namespace test
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            CSharpCodeProvider privoder = new CSharpCodeProvider();



            // 2.ICodeComplier
            //ICodeCompiler objICodeCompiler = objCSharpCodePrivoder.CreateCompiler();

            // 3.CompilerParameters
            CompilerParameters options = new CompilerParameters();
            //options.ReferencedAssemblies.Add("System.dll");
            options.ReferencedAssemblies.Add("System.Windows.Forms.dll"); 
            //options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            // 4.CompilerResults
            CompilerResults cr = privoder.CompileAssemblyFromSource(options, getCode());

            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("编译错误：");
                foreach (CompilerError err in cr.Errors)
                {
                    Console.WriteLine(err.ErrorText);
                }
            }
            else
            {
                // 通过反射，调用HelloWorld的实例

                Type DriverType = cr.CompiledAssembly.GetType("Driver");
                DriverType.InvokeMember("OutPut", 
                    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public, 
                    null, null, new object[]{"abcde"});

                
                //Assembly objAssembly = cr.CompiledAssembly;
                //object objHelloWorld = objAssembly.CreateInstance("test.Driver");
                //MethodInfo objMI = objHelloWorld.GetType().GetMethod("OutPut");

                //objMI.Invoke(objHelloWorld, null);
            }

        }


        private string getCode()
        {
            StringBuilder sb = new StringBuilder();
            //DateTime tm = DateTime.Now;
            
            sb.Append("using System;");
            sb.Append("using System.Windows.Forms;");
            //sb.Append("namespace test {");
            sb.Append("    public static class Driver");
            sb.Append("    {");
            sb.Append("        public static void OutPut(string testStr)");
            sb.Append("        {");
            sb.Append("             DateTime tm = DateTime.Now;");
            sb.Append("             MessageBox.Show(\"This is a test.\" + testStr  + tm.ToString());");
            sb.Append("        }");
            sb.Append("    }");
            //sb.Append("}");
            
            return sb.ToString();
        }

        private string getCode2()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("using System;");
            sb.Append("using System.Windows.Forms;");
            sb.Append("namespace test {");
            sb.Append("    public class Driver");
            sb.Append("    {");
            sb.Append("        public void OutPut()");
            sb.Append("        {");
            sb.Append("             MessageBox.Show(\"This is a test.\");");
            sb.Append("        }");
            sb.Append("    }");
            sb.Append("}");

            return sb.ToString();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            CSharpCodeProvider privoder = new CSharpCodeProvider();
            CompilerParameters options = new CompilerParameters();
            options.ReferencedAssemblies.Add("System.Windows.Forms.dll");
            options.GenerateExecutable = false;
            options.GenerateInMemory = true;
            CompilerResults cr = privoder.CompileAssemblyFromSource(options, getCode2());

            if (cr.Errors.HasErrors)
            {
                Console.WriteLine("编译错误：");
                foreach (CompilerError err in cr.Errors)
                {
                    Console.WriteLine(err.ErrorText);
                }
            }
            else
            {
                // 通过反射，调用HelloWorld的实例

                //Type DriverType = cr.CompiledAssembly.GetType("Driver");
                //DriverType.InvokeMember("OutPut",
                //    BindingFlags.InvokeMethod | BindingFlags.Static | BindingFlags.Public,
                //    null, null, null);


                Assembly objAssembly = cr.CompiledAssembly;
                object objHelloWorld = objAssembly.CreateInstance("test.Driver");
                MethodInfo objMI = objHelloWorld.GetType().GetMethod("OutPut");

                objMI.Invoke(objHelloWorld, null);
            }

        }

    }
}

