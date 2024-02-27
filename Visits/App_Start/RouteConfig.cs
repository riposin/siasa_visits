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

			// CAPTCHA Inc: BotDetect requests must not be routed
			routes.IgnoreRoute("{*botdetect}",	new { botdetect = @"(.*)BotDetectCaptcha\.ashx" });

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Preregistration", action = "Index", id = UrlParameter.Optional }
			);
		}
	}
}
