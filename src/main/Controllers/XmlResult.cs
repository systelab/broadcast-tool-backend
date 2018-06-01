using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Mvc;

namespace main.Controllers
{
    public class XmlResult : ActionResult
    {
        public XDocument Xml { get; private set; }
        public string ContentType { get; set; }

        public XmlResult(XDocument xml)
        {
            this.Xml = xml;
            this.ContentType = "text/xml";
        }

        public override void ExecuteResult(ActionContext context)
        {
            context.HttpContext.Response.ContentType = this.ContentType;
            if (Xml != null)
            {
                Xml.Save(context.HttpContext.Response.Body, SaveOptions.DisableFormatting);

            }
        }

        public override Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.ContentType = this.ContentType;

            if (Xml != null)
            {
                Xml.Save(context.HttpContext.Response.Body, SaveOptions.DisableFormatting);
                return Task.FromResult(0);

            }
            else
            {
                return base.ExecuteResultAsync(context);
            }
        }
    }
}