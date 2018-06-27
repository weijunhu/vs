using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace IocpServer
{
    class Program
    {
        static void Main(string[] args)
        {
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("0.0.0.0"), 7000);
            Server mServer = new Server(5, 1024 * 1024,2);
            mServer.Start(endPoint, 7001);

            //SetProtocolEnum();
            Console.ReadKey();
        }

        static void SetProtocolEnum()
        {
            string sinScriptPath = @"..\..\Protobuf\Protocol.cs";

            string fileContent = File.ReadAllText(@"..\..\Protobuf\base.proto");
            string pattern = @"message\s+(\w+)";
            MatchCollection regex = Regex.Matches(fileContent, pattern);

            StringBuilder sinDicContent = new StringBuilder();
            sinDicContent.Append("namespace IocpServer\r\n" +
                "{\r\n" +
                "\tpublic enum Protocol\r\n" +
                "\t{\r\n");
            for (int i = 0; i < regex.Count; i++)
            {
                string key = regex[i].Groups[1].Value;
                string str = "\t\t{0},\r\n";
                sinDicContent.Append(string.Format(str,key));
            }
            sinDicContent.Append("\t};\r\n");
            sinDicContent.Append("}");
            File.WriteAllText(sinScriptPath, sinDicContent.ToString());
        }
    }
}
