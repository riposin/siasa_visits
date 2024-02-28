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
		[Required(ErrorMessage = "La {0} es requerida")]
		[StringLength(20, ErrorMessage = "El {0} debe tener al menos {1} caracteres", MinimumLength = 1)]
		[Display(Name = "Clave de empresa")]
		public string CompanyKey { get; set; }

		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(100, ErrorMessage = "El {0} debe tener al menos {1} caracteres", MinimumLength = 3)]
		[Display(Name = "Nombre completo")]
		public string FullName { get; set; }

		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(100, ErrorMessage = "El {0} debe tener al menos {1} caracteres", MinimumLength = 5)]
		[EmailAddress]
		[Display(Name ="Correo electrónico")]
		public string Email { get; set; }

		//[DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
		[Required(ErrorMessage = "La {0} es requerida")]
		[DataType(DataType.DateTime)]
		[Display(Name = "Fecha y Hora")]
		[DateTimeGreaterThanToday]
		public DateTime VisitDate { get; set; }

		[Required(ErrorMessage = "El {0} es requerido")]
		[StringLength(150, ErrorMessage = "El {0} debe tener al menos {1} caracteres", MinimumLength = 4)]
		[Display(Name = "Motivo de visita")]
		public string Motive { get; set; }

		[Required(ErrorMessage = "Captura el código de 4 dígitos ({0}) en la caja de texto")]
		[Display(Name = "Captcha")]
		public string CaptchaCode { get; set; }
	}
}