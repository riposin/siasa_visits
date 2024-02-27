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


namespace Visits.Controllers
{
	public class PreregistrationController : Controller
	{
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
		public ActionResult Add()
		{
			return View();
		}

		[HttpPost]
		[CaptchaValidationActionFilter("CaptchaCode", "SIASAVisitsPrereg", "¡El Captcha no es correcto!")]
		public ActionResult Add(PreregistrationsViewModel model)
		{
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
			MvcCaptcha.ResetCaptcha("SIASAVisitsPrereg");

			return Redirect(Url.Content("~/Home"));
		}
	}
}