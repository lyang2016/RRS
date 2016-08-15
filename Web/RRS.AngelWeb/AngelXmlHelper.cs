using System.Text;

namespace RRS.AngelWeb
{
    public class AngelXmlHelper
    {
        public static void GenerateHeader(int nextPage, ref StringBuilder sb)
        {
            sb.Append(@"<?xml version=""1.0"" encoding=""UTF-8""?>");
            sb.Append(@"<ANGELXML><MESSAGE><PLAY>");
            sb.Append(@"<PROMPT type=""text"">.</PROMPT>");
            sb.Append(@"</PLAY>");
            sb.Append(@"<GOTO destination=""/");
            sb.Append(nextPage);
            sb.Append(@"""/>");
            sb.Append(@"</MESSAGE>");
            sb.Append(@"<VARIABLES>");
        }

        public static void GenerateFooter(ref StringBuilder sb)
        {
            sb.Append(@"</VARIABLES>");
            sb.Append(@"</ANGELXML>");
        }

        public static void GenerateVariable(string varName, string varValue, ref StringBuilder sb)
        {
            sb.Append(@"<VAR name=""");
            sb.Append(varName);
            sb.Append(@""" ");
            sb.Append(@"value=""");
            sb.Append(varValue);
            sb.Append(@"""/>");
        }
    }
}