using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Visits.Models;
using Visits.Models.TableViewModels;
using Visits.Models.ViewModels;
using System.Text;

namespace Visits.Controllers
{
	public class HomeController : Controller
	{
		public ActionResult Index()
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
					   select new PreregistrationsTableViewModel {
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

		public ActionResult About()
		{
			ViewBag.Message = "Your application description page.";

			return View();
		}

		public ActionResult Contact()
		{
			ViewBag.Message = "Your contact page.";

			return View();
		}
	}
}