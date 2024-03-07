using System.ComponentModel.DataAnnotations;

namespace linc.Models.ViewModels.Home
{
    public class CountsViewModel
    {
        [Display(Name = "SiteRole_Users", ResourceType = typeof(Resources.SharedResource))]
        public int UsersCount { get; set; }

        [Display(Name = "SiteRole_HeadEditors", ResourceType = typeof(Resources.SharedResource))]
        public int HeadEditorsCount { get; set; }

        [Display(Name = "SiteRole_Editors", ResourceType = typeof(Resources.SharedResource))]
        public int EditorsCount { get; set; }

        [Display(Name = "SiteRole_EditorsBoard", ResourceType = typeof(Resources.SharedResource))]
        public int EditorsBoardCount { get; set; }

        [Display(Name = "SiteRole_Administrator", ResourceType = typeof(Resources.SharedResource))]
        public int AdministratorsCount { get; set; }
    }
}
