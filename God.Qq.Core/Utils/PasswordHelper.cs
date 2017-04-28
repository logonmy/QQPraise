using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Security.Cryptography;
using Noesis.Javascript;

namespace God.Qq.Core
{
    internal class PasswordHelper
    {
        /// <summary>
        /// 根据QQ号码和验证码加密密码
        /// </summary>
        /// <param name="qqNum">QQ号码</param>
        /// <param name="password">QQ密码</param>
        /// <param name="verifycode">验证码</param>
        /// <returns>密码密文</returns>
        public static string GetPassword(string qqNum, string password, string verifycode)
        {
            //uin为QQ号码转换为16位的16进制
            int qq;
            int.TryParse(qqNum, out qq);

            qqNum = qq.ToString("x");
            qqNum = qqNum.PadLeft(16, '0');
            //Console.WriteLine(qqNum);

            String P = hexchar2bin(md5(password));
            String U = md5(P + hexchar2bin(qqNum)).ToUpper();
            String V = md5(U + verifycode.ToUpper()).ToUpper();
            return V;
        }

        public static string md5(string input)
        {
            byte[] buffer = MD5.Create().ComputeHash(Encoding.GetEncoding("ISO-8859-1").GetBytes(input));
            return binl2hex(buffer);
        }

        public static string binl2hex(byte[] buffer)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < buffer.Length; i++)
            {
                builder.Append(buffer[i].ToString("x2"));
            }
            return builder.ToString();
        }

        public static string hexchar2bin(string passWord)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < passWord.Length; i = i + 2)
            {
                builder.Append(Convert.ToChar(Convert.ToInt32(passWord.Substring(i, 2), 16)));
            }
            return builder.ToString();
        }
    }

    internal class Gtk
    {
        private static readonly Type EvaluateType;
        static Gtk()
        {
            var sb = new StringBuilder();
            sb.Append("package aa{");
            sb.Append(" public class JScript {");
            sb.Append("     public static function getGTK(str) {");
            sb.Append(" var hash = 5381;");
            sb.Append(" for(var i = 0, len = str.length; i < len; ++i) ");
            sb.Append(" { ");
            sb.Append("    hash += (hash << 5) + str.charAt(i).charCodeAt(); ");
            sb.Append(" } ");
            sb.Append("    return hash & 0x7fffffff; ");
            sb.Append(" }");
            sb.Append(" }");
            sb.Append("}");

            var parameters = new CompilerParameters { GenerateInMemory = true };

            CodeDomProvider provider = new Microsoft.JScript.JScriptCodeProvider();

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, sb.ToString());

            Assembly assembly = results.CompiledAssembly;

            EvaluateType = assembly.GetType("aa.JScript");
        }
        public static string GetGtk(params object[] para)
        {
            object obj = EvaluateType.InvokeMember("getGTK", BindingFlags.InvokeMethod, null, null, para);

            return obj.ToString();
        }
    }


    public class NewPasswordHelper
    {
        public static readonly string JsFile;
        static NewPasswordHelper()
        {
            var jsFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "\\Password.js";
            JsFile = File.ReadAllText(jsFilePath);
        }

        /// <summary>
        /// 根据QQ号码和验证码加密密码
        /// </summary>
        /// <param name="qq16">QQ号码</param>
        /// <param name="password">QQ密码</param>
        /// <param name="code"></param>
        /// <returns>密码密文</returns>
        public static string GetPassword(string qq16,string password,string code)
        {
            using (var ctx = new JavascriptContext())
            {
                ctx.Run(JsFile);
                var result = ctx.Run(string.Format("QXWEncodePwd('{0}','{1}','{2}');", qq16, password, code)) ?? "";
                return result.ToString();
            }
        }

        public static string GetQqHash(string qq, string ptwebqq)
        {
            using (var ctx = new JavascriptContext())
            {
                ctx.Run(JsFile);
                var result = ctx.Run(string.Format("GetQqHash('{0}','{1}');", qq, ptwebqq)) ?? "";
                return result.ToString();
            }
        }
    }
}
