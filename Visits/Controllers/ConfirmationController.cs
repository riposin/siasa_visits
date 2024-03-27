using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Visits.Models;

namespace Visits.Controllers
{
	public class ConfirmationController : Controller
	{
		public ConfirmationController()
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

		[HttpGet]
		public ActionResult Index(string id = "")
		{
			return Redirect(Url.Content("~/Confirmation/Show/" + id));
		}

		public ActionResult Show(string id = "")
		{
			List<preregistration> pre = validate(id);
			if (pre.Count == 0)
			{
				return View();
			} else {
				return View(pre[0]);
			}
			

			// Not confirmed and creation is lees than the deadline(24hours)
			// 0
			//return View(pre[0]);
		}

		[HttpPost]
		public ActionResult Confirm(string id)
		{
			byte[] bytesGuid;
			
			validate(id);

			if (ViewBag.Status == 0)
			{
				bytesGuid = System.Text.Encoding.ASCII.GetBytes(id);
				using (var db = new visitsEntities())
				{
					var oPre = db.preregistrations.Find(bytesGuid);
					if (oPre.confirmed_at == null)
					{
						oPre.confirmed_at = DateTime.Now;
						db.Entry(oPre).State = System.Data.Entity.EntityState.Modified;
						db.SaveChanges();
					}
				}
				return Content("{\"success\":true, \"error\":\"0\"}", "application/json; charset=utf-8");
			} else {
				return Content("{\"success\":false, \"error\":\"" + ViewBag.Status + "\"}", "application/json; charset=utf-8");
			}
		}

		public List<preregistration> validate(string id = "")
		{
			ViewBag.Status = 0;
			ViewBag.QRData = "";
			ViewBag.Guid = id;
			List<preregistration> pre = new List<preregistration>();
			Guid guid;
			byte[] bytesGuid;

			bytesGuid = System.Text.Encoding.ASCII.GetBytes(id);

			if (!Guid.TryParse(id, out guid))
			{
				// Not valid
				ViewBag.Status = -1;
				return pre;
			}

			using (var db = new visitsEntities())
			{
				pre = (from d in db.preregistrations
					   where d.guid == bytesGuid
					   select d).ToList();
			}

			if (pre.Count != 1)
			{
				// Not found
				ViewBag.Status = -2;
				return pre;
			}

			if (ViewBag.Settings.link_invalidate_after_confirm == 1 && (pre[0].confirmed_at != null || (DateTime.Now > pre[0].visit_date)))
			{
				// Confirmed/VisitDate Expired & Only one view
				ViewBag.Status = -6;
				return pre;
			}

			if (DateTime.Now > pre[0].visit_date)
			{
				// Visit date expired
				ViewBag.Status = -3;
				return pre;
			}

			if (pre[0].confirmed_at != null)
			{
				// Confirmed
				ViewBag.Status = -4;
				return pre;
			}

			if (DateTime.Now > pre[0].created_at.AddHours(Convert.ToDouble(ViewBag.Settings.link_expiration_hours)))
			{
				// Not confirmed but creation is more than the deadline(24hours)
				ViewBag.Status = -5;
				return pre;
			}

			// Not confirmed and creation is lees than the deadline(24hours)
			// 0
			return pre;
		}
	}
}