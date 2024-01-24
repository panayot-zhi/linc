using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Home
{
    public class CountsViewModel
    {
        [Display(Name = "SiteRole_Users", ResourceType = typeof(Resources.SharedResource))]
        public int UsersCount { get; init; } = 9;

        [Display(Name = "SiteRole_HeadEditors", ResourceType = typeof(Resources.SharedResource))]
        public int HeadEditorsCount { get; init; } = 2;

        [Display(Name = "SiteRole_Editors", ResourceType = typeof(Resources.SharedResource))]
        public int EditorsCount { get; init; } = 6;

        [Display(Name = "SiteRole_EditorsBoard", ResourceType = typeof(Resources.SharedResource))]
        public int EditorsBoardCount { get; init; } = 7;

        [Display(Name = "SiteRole_Administrator", ResourceType = typeof(Resources.SharedResource))]
        public int AdministratorsCount { get; init; } = 1;
    }
}
