using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using linc.Data;
using linc.Utility;

namespace linc.Models.ViewModels.Source
{
    public class SourceIndexViewModel : PagedViewModel
    {
        public SourceIndexViewModel(int totalRecords, int pageIndex, int pageSize) : base(totalRecords, pageIndex, pageSize)
        {

        }


        public IEnumerable<ApplicationSource> Records { get; set; }


        public List<SourceCountByYears> YearFilter { get; set; }

        public int? CurrentYearFilter { get; set; }


        public List<SourceCountByIssues> IssuesFilter { get; set; }

        public int? CurrentIssueId { get; set; }


        public string AuthorsFilter { get; set; }

        public string CurrentAuthorsFilter { get; set; }

    }

}
