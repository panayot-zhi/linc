using linc.Contracts;
using linc.Data;
using linc.Utility;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;

namespace linc.Services
{
    public class LocalizationService : ILocalizationService
    {
        // TODO: Monitor Cache size!
        // TODO: Write unit tests for this service!

        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<LocalizationService> _logger;
        private readonly ApplicationDbContext _context;
        private readonly IMemoryCache _cache;

        private static string GetCacheKey(string key, int languageId) =>
            $"{languageId}_{key}";

        public LocalizationService(ApplicationDbContext context, IMemoryCache cache, 
            ILogger<LocalizationService> logger, IStringLocalizer<SharedResource> localizer)
        {
            _logger = logger;
            _localizer = localizer;
            _context = context;
            _cache = cache;
        }

        private string GetStringResource(string resourceKey, int languageId)
        {
            var key = GetCacheKey(resourceKey, languageId);
            var cachedItem = _cache.Get<string>(key);
            if (!string.IsNullOrEmpty(cachedItem))
            {
                return cachedItem;
            }

            var dbItem = _context.StringResources.FirstOrDefault(x =>
                x.Key.Trim().ToLower() == resourceKey.Trim().ToLower() && 
                x.LanguageId == languageId);

            if (dbItem == null)
            {
                // no luck
                return null;
            }

            CacheStringResource(dbItem);

            return dbItem.Value;
        }

        public async Task SetStringResource(string resourceKey, string value, string userId)
        {
            var languageId = GetCurrentLanguageId();
            var stringResource = GetStringResource(resourceKey, languageId);
            if (stringResource == null)
            {
                // NOTE: Resource could not be
                // found in the cache or database
                await CreateStringResource(resourceKey, value, languageId, userId);

                CacheStringResource(resourceKey, value, languageId);

                return;
            }

            // NOTE: we save in the cache registry only the value of the string resource
            // therefore we need to make another hop to the database to retrieve the entry
            var dbItem = _context.StringResources.FirstOrDefault(x =>
                x.Key.Trim().ToLower() == resourceKey.Trim().ToLower() &&
                x.LanguageId == languageId);

            // NOTE: Resource could be found in the cache,
            // but not in the database
            if (dbItem is null)
            {
                dbItem = await CreateStringResource(resourceKey, value, languageId, userId);
            }
            else
            {
                // Attach entity always, otherwise we read only
                _context.StringResources.Attach(dbItem);

                dbItem.Value = value;
                dbItem.EditedById = userId;
                dbItem.LastEdited = DateTime.UtcNow;

                await _context.SaveChangesAsync();
            }

            CacheStringResource(dbItem);
        }

        private async Task<ApplicationStringResource> CreateStringResource(string resourceKey, string resourceValue, int languageId, string userId)
        {
            var newEntry = new ApplicationStringResource
            {
                Key = resourceKey,
                Value = resourceValue,
                LanguageId = languageId,
                EditedById = userId,
                LastEdited = DateTime.UtcNow
            };

            await _context.StringResources.AddAsync(newEntry);
            await _context.SaveChangesAsync();

            return newEntry;
        }

        private void CacheStringResource(ApplicationStringResource stringResource)
        {
            CacheStringResource(stringResource.Key, stringResource.Value, 
                stringResource.LanguageId);
        }

        private void CacheStringResource(string resourceKey, string value, int languageId)
        {
            var key = GetCacheKey(resourceKey, languageId);
            _cache.Set(key, value,
                new MemoryCacheEntryOptions
                {
                    Size = value.Length,
                    SlidingExpiration = TimeSpan.FromHours(12),
                    PostEvictionCallbacks = {
                        new PostEvictionCallbackRegistration()
                        {
                            EvictionCallback = OnCacheEvictionCallback
                        }
                }
            });
        }

        private void OnCacheEvictionCallback(object key, object value, EvictionReason reason, object state)
        {
            if (reason != EvictionReason.Expired)
            {
                // we don't care
                // about this reason
                return;
            }

            _logger.LogInformation("CacheItem with key {ResourceKey} has been evicted from the cache registry: {Reason}", 
                key, reason);
        }

        private LocalizedHtmlString Localize(string resourceKey, params object[] args)
        {
            var item = _localizer[resourceKey, args];
            if (!item.ResourceNotFound)
            {
                return new LocalizedHtmlString(item.Name, item.Value, false, args);
            }

            var languageId = GetCurrentLanguageId();
            var stringResource = GetStringResource(resourceKey, languageId);
            if (string.IsNullOrEmpty(stringResource))
            {
                return new LocalizedHtmlString(resourceKey, resourceKey, true, args);
            }

            return new LocalizedHtmlString(resourceKey, stringResource, false, args);

        }

        public LocalizedHtmlString this[string name, params object[] arguments] => Localize(name, arguments);

        public LocalizedHtmlString this[string name] => Localize(name);

        public string GetCurrentLanguage()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            var language = SiteConstant.SupportedCultures.First(x =>
                x.Value == currentCulture).Value;

            return language;
        }

        public int GetCurrentLanguageId()
        {
            var currentCulture = Thread.CurrentThread.CurrentUICulture.Name;
            var languageId = SiteConstant.SupportedCultures.First(x =>
                x.Value == currentCulture).Key;

            return languageId;
        }
    }
}
