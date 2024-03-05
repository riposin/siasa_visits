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
		public ActionResult Index()
		{
			return View("AddFE");
		}

		[HttpGet]
		public ActionResult List()
		{
			List<PreregistrationsTableViewModel> list = null;
			using (visitsEntities db = new visitsEntities())
			{
				ViewBag.Message = "Lista de pre-registros ordenados por fecha de creacion descendientemente";

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
			string body = "";

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
			mail.To.Add("rpool@siasa.com");
			mail.From = new MailAddress("ripostf@gmail.com");
			mail.Subject = "Solicitud de confirmación de Visita";
			body = "<p>Clave de empresa: <span style=\"font-weight: bold;\">{0}</span><br/>Nombre completo: <span style=\"font-weight: bold;\">{1}</span><br/>Fecha y hora: <span style=\"font-weight: bold;\">{2}</span><br/>Motivo: <span style=\"font-weight: bold;\">{3}</span><br/>Enlace para confirmar el pre-registro: <span style=\"font-weight: bold;\"><a href=\"{4}\">{4}</a></span><br/></p>";
			string link = "https://localhost:44321/Confirmation/Confirm/{0}";
			StringBuilder sbLink = new StringBuilder();
			StringBuilder sbBody = new StringBuilder();
			sbLink.AppendFormat(link, g.ToString());
			sbBody.AppendFormat(body, model.CompanyKey, model.FullName, model.VisitDate.ToString("dd/MM/yyyy hh:mm tt"), model.Motive, sbLink.ToString());
			/*body += "Clave de Empresa: " + model.CompanyKey + "\n";
			body += "Nombre: " + model.FullName + "\n";
			body += "Fecha y hora: " + model.VisitDate.ToString() + "\n";
			body += "Motivo: " + model.Motive + "\n";
			body += "Enlace para confirmar el pre-registro: " + g.ToString();*/
			mail.Body = sbBody.ToString();
			mail.IsBodyHtml = true;
			SmtpClient smtp = new SmtpClient();
			smtp.Host = "smtp.gmail.com";
			smtp.Port = 587;
			smtp.UseDefaultCredentials = false;
			smtp.Credentials = new System.Net.NetworkCredential("", ""); // Enter seders User name and password  
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
					pre.company_key = model.CompanyKey;
					pre.full_name = model.FullName;
					pre.email = model.Email;
					pre.visit_date = model.VisitDate;
					pre.motive = model.Motive;
					pre.created_at = DateTime.Now;

					db.preregistrations.Add(pre);
					db.SaveChanges();
				}
				/*using (var db = new visitsEntities())
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
				}*/
				return Content("{\"success\":1}", "application/json; charset=utf-8");
			}
			else
			{
				return Content("{\"success\":3, \"error\":" + JsonConvert.SerializeObject(errors) + "}", "application/json; charset=utf-8");
			}

			


		}
	}
}