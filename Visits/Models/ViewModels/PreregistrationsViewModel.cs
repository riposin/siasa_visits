using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Visits.Validations;

namespace Visits.Models.ViewModels
{
	public class PreregistrationsViewModel
	{
		[Required(ErrorMessage = "MSG_REQ_FIELD")]
		[StringLength(20, ErrorMessage = "MSG_COMPKEY_LENGTH", MinimumLength = 1)]
		[Display(Name = "LBL_COMP_KEY")]
		public string CompanyKey { get; set; }

		[Required(ErrorMessage = "MSG_REQ_FIELD")]
		[StringLength(100, ErrorMessage = "MSG_FNAME_LENGTH", MinimumLength = 3)]
		[Display(Name = "LBL_FNAME")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "MSG_REQ_FIELD")]
		[StringLength(100, ErrorMessage = "MSG_EMAIL_LENGTH", MinimumLength = 5)]
		[EmailAddress(ErrorMessage = "MSG_EMAIL_INVALID")]
		[Display(Name = "LBL_EMAIL")]
		public string Email { get; set; }

		//[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Required(ErrorMessage = "MSG_REQ_FIELD")]
		[DataType(DataType.DateTime)]
		[Display(Name = "LBL_DATETIME")]
		[DateTimeGreaterThanToday]
		public DateTime VisitDate { get; set; }

		//[Required(ErrorMessage = "El {0} es requerido")]
		//[StringLength(150, ErrorMessage = "El {0} debe tener entre {2} y {1} caracteres", MinimumLength = 4)]
		[StringLength(150, ErrorMessage = "MSG_MOTIV_LENGTH")]
		[Display(Name = "LBL_VMOTIVE")]
		public string Motive { get; set; }

		[Required(ErrorMessage = "MSG_CAPTURECAPTCHA")]
		[Display(Name = "LBL_CAPTCHA")]
		public string CaptchaCode { get; set; }

		public string CaptchaId { get; set; }

		public string Language { get; set; }
	}
}