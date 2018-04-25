using System.ComponentModel.DataAnnotations;
using real_estate_agency.Resources;

namespace real_estate_agency.Models.ViewModels
{
    public class BlockUserViewModel
    {
        public string UserId { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputReason", ErrorMessageResourceType = typeof(Resource))]
        public string Message { get; set; }

        [Required(ErrorMessageResourceName = "ModelValidatorInputDurability", ErrorMessageResourceType = typeof(Resource))]
        public BlockDuration Duration { get; set; }
    }

    public enum BlockDuration
    {
        [Display(Name = "ModelValidatorInpitBlockHour", ResourceType = typeof(Resource))]
        Hour,
        [Display(Name = "ModelValidatorInpitBlockDay", ResourceType = typeof(Resource))]
        Day,
        [Display(Name = "ModelValidatorInpitBlockWeek", ResourceType = typeof(Resource))]
        Week,
        [Display(Name = "ModelValidatorInpitBlockMonth", ResourceType = typeof(Resource))]
        Month,
        [Display(Name = "ModelValidatorInpitBlockPermanently", ResourceType = typeof(Resource))]
        Forever
    }
}