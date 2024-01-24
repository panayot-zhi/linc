using System.ComponentModel.DataAnnotations;

namespace linc.Models.Enumerations;

public enum IndexCategory
{
    [Display(Name = "Category_All", ResourceType = typeof(Resources.SharedResource))]
    All,

    [Display(Name = "Category_Articles", ResourceType = typeof(Resources.SharedResource))]
    Articles,

    [Display(Name = "Category_Analyses", ResourceType = typeof(Resources.SharedResource))]
    Analyses,

    [Display(Name = "Category_Surveys", ResourceType = typeof(Resources.SharedResource))]
    Surveys,

    [Display(Name = "Category_Discussions", ResourceType = typeof(Resources.SharedResource))]
    Discussions,

    [Display(Name = "Category_Reviews", ResourceType = typeof(Resources.SharedResource))]
    Reviews,

    [Display(Name = "Category_Chronicles", ResourceType = typeof(Resources.SharedResource))]
    Chronicles,

    [Display(Name = "Category_Foreign", ResourceType = typeof(Resources.SharedResource))]
    Foreign
}
