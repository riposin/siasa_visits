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
		[HttpGet]
		public ActionResult Index(string id = "")
		{
			return Redirect(Url.Content("~/Confirmation/Show/" + id));
		}

		[HttpGet]
		public ActionResult Show(string id = "")
		{
			ViewBag.Status = 0;
			ViewBag.QRData = "";
			List <preregistration> pre;
			preregistrations_settings settings;
			Guid guid;
			byte[] bytesGuid;

			using (var db = new visitsEntities())
			{
				settings = (from d in db.preregistrations_settings
							where d.id == 1
							select d).ToList()[0];
			}

			ViewBag.Settings = settings;
			bytesGuid = System.Text.Encoding.ASCII.GetBytes(id);

			if (!Guid.TryParse(id, out guid))
			{
				// Not valid
				ViewBag.Status = -1;
				return View();
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
				return View();
			}

			if(DateTime.Now > pre[0].visit_date)
			{
				// Visit date expired
				ViewBag.Status = -3;
				return View(pre[0]);
			}

			if (pre[0].confirmed_at != null)
			{
				// Confirmed
				ViewBag.Status = -4;
				return View(pre[0]);
			}

			if (DateTime.Now > pre[0].created_at.AddHours(Convert.ToDouble(settings.link_expiration_hours)))
			{
				// Not confirmed but creation is more than the deadline(24hours)
				ViewBag.Status = -5;
				return View(pre[0]);
			}

			// Not confirmed and creation is lees than the deadline(24hours)
			// 0
			ViewBag.Guid = id;
			return View(pre[0]);
		}

		[HttpPost]
		public ActionResult Confirm(string id)
		{
			List<preregistration> pre;
			Guid guid;
			byte[] bytesGuid;

			bytesGuid = System.Text.Encoding.ASCII.GetBytes(id);

			if (!Guid.TryParse(id, out guid))
			{
				// Not valid
				return Content("{\"success\":false, \"error\":\"-1\"}", "application/json; charset=utf-8");
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
				return Content("{\"success\":false, \"error\":\"-2\"}", "application/json; charset=utf-8");
			}

			using (var db = new visitsEntities())
			{
				var oPre = db.preregistrations.Find(bytesGuid);
				if(oPre.confirmed_at == null)
				{
					oPre.confirmed_at = DateTime.Now;
					db.Entry(oPre).State = System.Data.Entity.EntityState.Modified;
					db.SaveChanges();
				}
			}

			return Content("{\"success\":true, \"error\":\"0\"}", "application/json; charset=utf-8");
		}
	}
}