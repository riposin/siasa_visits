using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Visits.Models;
using Visits.Models.TableViewModels;
using Visits.Models.ViewModels;
using System.Text;
using BotDetect.Web.Mvc;
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
			return View("Add");
		}

		[HttpGet]
		public ActionResult List()
		{
			List<PreregistrationsTableViewModel> list = null;
			using (visitsEntities db = new visitsEntities())
			{
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
			}

			// RPOOL: Disabling feature for Release version
			if(ViewBag.Debug == false)
			{
				list = new List<PreregistrationsTableViewModel>();
			}

			return View(list);
		}

		[HttpGet]
		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[CaptchaValidationActionFilter("CaptchaCode", "Captcha", "¡El Captcha no es correcto!")]
		public ActionResult Add(PreregistrationsViewModel model)
		{
			MvcCaptcha.ResetCaptcha("Captcha");
			Dictionary<string, string> errors = new Dictionary<string, string>();
			string[] keys = ModelState.Keys.ToArray();
			ModelState[] values = ModelState.Values.ToArray();
			bool isEmailOk = false;
			Guid g = Guid.NewGuid();
			List<locale> currentLocale = new List<locale>();
			List<locale> locales = ViewBag.locales; // Convert.ChangeType(ViewBag.locales, typeof(List<locale>));

			if (!String.IsNullOrEmpty(model.Language))
			{
				currentLocale = (from l in locales
								 where l.id == model.Language
								 select l).ToList();
			}

			if(currentLocale.Count == 0)
			{
				currentLocale = (from l in locales
								 where l.id == ViewBag.Settings.lang_locale_default
								 select l).ToList();
			}

			for (int i = 0; i < ModelState.Keys.Count; i++)
			{
				if (values[i].Errors.Count > 0)
				{
					errors.Add(keys[i], "");
					errors[keys[i]] = values[i].Errors[0].ErrorMessage;
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
			sbBody.AppendFormat(ViewBag.Settings.email_body_format, model.CompanyKey.ToUpper(), model.FullName, model.VisitDate.ToString(currentLocale[0].date_time_format), model.Motive, sbLink.ToString());
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