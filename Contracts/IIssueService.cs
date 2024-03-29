﻿using linc.Data;
using linc.Models.ViewModels.Issue;

namespace linc.Contracts
{
    public interface IIssueService
    {
        Task<ApplicationDocument> GetFileAsync(int id);

        Task<ApplicationIssue> GetIssueAsync(int id, int? languageId = null);

        Task<List<ApplicationIssue>> GetIssuesAsync();

        Task<int> CreateIssueAsync(IssueCreateViewModel input);
    }
}
