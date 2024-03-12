using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Visits.Models;
using Visits.Models.TableViewModels;
using Visits.Models.ViewModels;
using System.Text;
using BotDetect.Web.Mvc;
using BotDetect.Web;
using Newtonsoft.Json;
using System.Net.Mail;


namespace Visits.Controllers
{
	public class PreregistrationController : Controller
	{
		public PreregistrationController()
		{
			// Data used for _Layout view
			ViewBag.Debug = false;
			#if (DEBUG)
				ViewBag.Debug = true;
			#endif

			preregistrations_settings settings;
			using (var db = new visitsEntities())
			{
				settings = (from d in db.preregistrations_settings
							where d.id == 1
							select d).ToList()[0];
			}
			ViewBag.Settings = settings;

			List<locale> locales;
			using (var db = new visitsEntities())
			{
				locales = (from d in db.locales
							select d).ToList();
			}
			ViewBag.Locales = locales;
		}

		public ActionResult Index()
		{
			return View("AddFEBE");
		}

		[HttpGet]
		public ActionResult List()
		{
			List<PreregistrationsTableViewModel> list = null;
			using (visitsEntities db = new visitsEntities())
			{
				/*
				preregistration pre = new preregistration();
				Guid g = Guid.NewGuid();
				byte[] bytes = Encoding.ASCII.GetBytes(g.ToString());
				pre.guid = bytes;
				pre.company_key = "Foo";
				pre.full_name = "Bar";
				pre.email = "r@siasa.com";
				pre.visit_date = DateTime.Parse("2024-03-01 11:15:00");
				pre.motive = "Tour";
				pre.created_at = DateTime.Now;

				db.preregistrations.Add(pre);
				db.SaveChanges();
				*/

				list = (from d in db.preregistrations
						orderby d.created_at descending
						select new PreregistrationsTableViewModel
						{
							GUID = d.guid,
							CompanyKey = d.company_key,
							FullName = d.full_name,
							Email = d.email,
							VisitDate = d.visit_date,
							Motive = d.motive
						}).ToList();

				/*var list = from d in db.preregistrations
						   where d.confirmed_at == null
						   select d;
				ViewBag.Message = "Conteo de solicitudes de visitas no confirmadas: " + list.Count().ToString();*/
			}

			// RPOOL: Disabling feature for Release version
			if(ViewBag.Debug == false)
			{
				list = new List<PreregistrationsTableViewModel>();
			}

			return View(list);
		}

		[HttpGet]
		public ActionResult AddFE()
		{
			return View();
		}

		[HttpGet]
		public ActionResult AddBE()
		{
			return View();
		}

		[HttpGet]
		public ActionResult AddFEBE()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult AddFE(PreregistrationsViewModel model)
		{
			SimpleCaptcha captcha = new SimpleCaptcha();
			bool isHuman = captcha.Validate(model.CaptchaCode, model.CaptchaId);
			MvcCaptcha.ResetCaptcha("CaptchaCode");

			if (!ModelState.IsValid || !isHuman)
			{
				return Content("{\"success\":false}", "application/json; charset=utf-8");
			}

			using (var db = new visitsEntities())
			{
				preregistration pre = new preregistration();
				Guid g = Guid.NewGuid();
				byte[] bytes = Encoding.ASCII.GetBytes(g.ToString());
				pre.guid = bytes;
				pre.company_key = model.CompanyKey;
				pre.full_name = model.FullName;
				pre.email = model.Email;
				pre.visit_date = model.VisitDate;
				pre.motive = model.Motive;
				pre.created_at = DateTime.Now;

				db.preregistrations.Add(pre);
				db.SaveChanges();
			}

			return Content("{\"success\":true}", "application/json; charset=utf-8");
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[CaptchaValidationActionFilter("CaptchaCode", "Captcha", "¡El Captcha no es correcto!")]
		public ActionResult AddBE(PreregistrationsViewModel model)
		{
			MvcCaptcha.ResetCaptcha("CaptchaCode");

			if (!ModelState.IsValid)
			{
				return View(model);
			}

			using (var db = new visitsEntities())
			{
				preregistration pre = new preregistration();
				Guid g = Guid.NewGuid();
				byte[] bytes = Encoding.ASCII.GetBytes(g.ToString());
				pre.guid = bytes;
				pre.company_key = model.CompanyKey;
				pre.full_name = model.FullName;
				pre.email = model.Email;
				pre.visit_date = model.VisitDate;
				pre.motive = model.Motive;
				pre.created_at = DateTime.Now;

				db.preregistrations.Add(pre);
				db.SaveChanges();
			}
			
			return Redirect(Url.Content("~/Home"));
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[CaptchaValidationActionFilter("CaptchaCode", "Captcha", "¡El Captcha no es correcto!")]
		public ActionResult AddFEBE(PreregistrationsViewModel model)
		{
			MvcCaptcha.ResetCaptcha("Captcha");
			Dictionary<string, string> errors = new Dictionary<string, string>();
			string[] keys = ModelState.Keys.ToArray();
			ModelState[] values = ModelState.Values.ToArray();
			bool isEmailOk = false;
			Guid g = Guid.NewGuid();

			for (int i = 0; i < ModelState.Keys.Count; i++)
			{
				errors.Add(keys[i], "");
				if (values[i].Errors.Count > 0)
				{
					errors[ModelState.Keys.ToArray()[i]] = values[i].Errors[0].ErrorMessage;
				}
			}

			if (!ModelState.IsValid)
			{
				return Content("{\"success\":2, \"error\":" + JsonConvert.SerializeObject(errors) + "}", "application/json; charset=utf-8");
			}

			MailMessage mail = new MailMessage();
			mail.To.Add(model.Email);
			mail.From = new MailAddress(ViewBag.Settings.smtp_user);
			mail.Subject = System.Net.WebUtility.HtmlDecode(ViewBag.Settings.email_subject);
			StringBuilder sbLink = new StringBuilder();
			StringBuilder sbBody = new StringBuilder();
			sbLink.AppendFormat(string.Format("{0}://{1}{2}", Request.Url.Scheme, Request.Url.Authority, Url.Content("~")) + ViewBag.Settings.link_url_format, g.ToString());
			sbBody.AppendFormat(ViewBag.Settings.email_body_format, model.CompanyKey.ToUpper(), model.FullName, model.VisitDate.ToString(ViewBag.Settings.email_date_time_format), model.Motive, sbLink.ToString());
			mail.Body = sbBody.ToString();
			mail.IsBodyHtml = true;
			SmtpClient smtp = new SmtpClient();
			smtp.Host = ViewBag.Settings.smtp_host;
			smtp.Port = ViewBag.Settings.smtp_port;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new System.Net.NetworkCredential(ViewBag.Settings.smtp_user, ViewBag.Settings.smtp_password);
			smtp.EnableSsl = true;

			try
			{
				smtp.Send(mail);
				isEmailOk = true;
			}
			catch (Exception e)
			{
				isEmailOk = false;
			}
			
			if (isEmailOk)
			{
				using (var db = new visitsEntities())
				{
					preregistration pre = new preregistration();
					byte[] bytes = Encoding.ASCII.GetBytes(g.ToString());
					pre.guid = bytes;
					pre.company_key = model.CompanyKey.ToUpper();
					pre.full_name = model.FullName;
					pre.email = model.Email;
					pre.visit_date = model.VisitDate;
					pre.motive = model.Motive;
					pre.created_at = DateTime.Now;

					db.preregistrations.Add(pre);
					db.SaveChanges();
				}
				return Content("{\"success\":1, \"error\":" + JsonConvert.SerializeObject(errors) + "}", "application/json; charset=utf-8");
			}
			else
			{
				return Content("{\"success\":3, \"error\":" + JsonConvert.SerializeObject(errors) + "}", "application/json; charset=utf-8");
			}
		}
	}
}