using System;
using System.Globalization;
using System.ComponentModel.DataAnnotations;

namespace Visits.Validations
{
	[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
	sealed public class DateTimeGreaterThanToday : ValidationAttribute
	{
		public DateTimeGreaterThanToday(): base("MSG_DTGREATER_TODAY") {}

		protected override ValidationResult IsValid(object value, ValidationContext validationContext)
		{
			DateTime dateMustBeGreater = Convert.ToDateTime(value);
			DateTime nowNoSeconds = DateTime.Now;
			nowNoSeconds = nowNoSeconds.AddMinutes(-1).AddSeconds(nowNoSeconds.Second * -1);
			if (nowNoSeconds > dateMustBeGreater)
			{
				var errorMessage = FormatErrorMessage(validationContext.DisplayName);
				return new ValidationResult(errorMessage);
			}
			
			return ValidationResult.Success;
		}
	}
}