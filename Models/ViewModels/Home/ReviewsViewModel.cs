using System.Numerics;

namespace linc.Models.ViewModels.Home;

public class ReviewsViewModel
{
    public int SubmittedArticlesCount { get; set; } = 10;

    public int ReviewedArticlesCount { get; set; } = 8;

    public int ReviewedArticlesPercent => (int)Math.Round((double)(100 * ReviewedArticlesCount) / SubmittedArticlesCount);

    public int SubmittedAnalysesCount { get; set; } = 2;

    public int ReviewedAnalysesCount { get; set; } = 2;

    public int ReviewedAnalysesPercent => (int)Math.Round((double)(100 * ReviewedAnalysesCount) / SubmittedAnalysesCount);

    public int SubmittedSurveysCount { get; set; } = 11;

    public int ReviewedSurveysCount { get; set; } = 2;

    public int ReviewedSurveysPercent => (int)Math.Round((double)(100 * ReviewedSurveysCount) / SubmittedSurveysCount);

    public int SubmittedDiscussionsCount { get; set; } = 7;

    public int ReviewedDiscussionsCount { get; set; } = 5;

    public int ReviewedDiscussionsPercent => (int)Math.Round((double)(100 * ReviewedDiscussionsCount) / SubmittedDiscussionsCount);

    public int SubmittedReviewsCount { get; set; } = 14;

    public int ReviewedReviewsCount { get; set; } = 7;

    public int ReviewedReviewsPercent => (int)Math.Round((double)(100 * ReviewedReviewsCount) / SubmittedReviewsCount);

    public int SubmittedChroniclesCount { get; set; } = 3;

    public int ReviewedChroniclesCount { get; set; } = 0;

    public int ReviewedChroniclesPercent => (int)Math.Round((double)(100 * ReviewedChroniclesCount) / SubmittedChroniclesCount);

    public int SubmittedForeignCount { get; set; } = 3;

    public int ReviewedForeignCount { get; set; } = 3;

    public int ReviewedForeignPercent => (int)Math.Round((double)(100 * ReviewedForeignCount) / SubmittedForeignCount);
}