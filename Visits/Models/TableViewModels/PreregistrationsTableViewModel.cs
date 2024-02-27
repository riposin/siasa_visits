using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Visits.Models.TableViewModels
{
	public class PreregistrationsTableViewModel
	{
		public byte[] GUID { get; set; }
		public string CompanyKey { get; set; }
		public string FullName{ get; set; }
		public string Email { get; set; }
		public DateTime VisitDate { get; set; }
		public string Motive { get; set; }
	}
}