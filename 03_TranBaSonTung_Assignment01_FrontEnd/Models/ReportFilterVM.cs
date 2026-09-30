using System.ComponentModel.DataAnnotations;

namespace FrontEnd.Models;

public class ReportFilterVM : IValidatableObject
{
    [Display(Name = "Start date")]
    [Required(ErrorMessage = "Start date is required.")]
    [DataType(DataType.Date)]
    public DateTime? StartDate { get; set; }

    [Display(Name = "End date")]
    [Required(ErrorMessage = "End date is required.")]
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }

    public NewsReportVM? Report { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartDate.HasValue && EndDate.HasValue && StartDate.Value.Date > EndDate.Value.Date)
        {
            yield return new ValidationResult("Start date must be before or equal to end date.", new[] { nameof(EndDate) });
        }
    }
}
