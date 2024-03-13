﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Visits.Models;
using Newtonsoft.Json;

namespace Visits.Controllers
{
    public class LanguageController : Controller
    {
        // GET: Language
        public ActionResult Index(string lang = "")
        {
            return View();
        }

		[HttpGet]
		public ActionResult Labels(string id = "")
		{
			List<label> list = null;
			Dictionary<string, string> labels = new Dictionary<string, string>();

			using (var db = new visitsEntities())
			{
				list = (from d in db.labels
						where d.locale_id == id
						select d).ToList();
			}
			for (int i = 0; i < list.Count; i++)
			{
				labels.Add(list[i].label1, list[i].translation);
			}

			return Content(JsonConvert.SerializeObject(labels), "application/json; charset=utf-8");
		}
	}
}