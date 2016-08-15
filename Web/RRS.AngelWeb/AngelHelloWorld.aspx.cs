using System;
using System.Text;

namespace RRS.AngelWeb
{
    public partial class AngelHelloWorld : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                var sb = new StringBuilder();
                AngelXmlHelper.GenerateHeader(RequestHelper.NullInt(Request["NextPage"]), ref sb);
                AngelXmlHelper.GenerateVariable("ReturnCode", "100", ref sb);
                AngelXmlHelper.GenerateVariable("Message", "Hello World", ref sb);
                AngelXmlHelper.GenerateFooter(ref sb);

                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "text/xml";
                Response.Write(sb.ToString());
                Response.Flush();
                Response.Close();
            }
        }
    }
}