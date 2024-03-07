using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Visits
{
	public class RouteConfig
	{
		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
			//routes.MapMvcAttributeRoutes();

			// CAPTCHA Inc: BotDetect requests must not be routed
			routes.IgnoreRoute("{*botdetect}",	new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });
			routes.IgnoreRoute("{*botdetect}",	new { botdetect = @"(.*)simple-captcha-endpoint\.ashx" });

			routes.MapRoute(
				name: "Confirmation",
				url: "Confirmation/{action}/{id}",
				defaults: new { controller = "Confirmation", action = "Index", id = UrlParameter.Optional }
			);

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Preregistration", action = "AddFEBE", id = UrlParameter.Optional }
			);
		}
	}
}
