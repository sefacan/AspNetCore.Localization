using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace AspNetCore.Localization.Localization
{
    public class SqlStringLocalizerFactory : IStringLocalizerFactory
    {
        private const string ResourcesName = "system.localizations";
        private readonly ConcurrentDictionary<string, IStringLocalizer> _localizations = new ConcurrentDictionary<string, IStringLocalizer>();
        private readonly IServiceProvider _serviceProvider;

        public SqlStringLocalizerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            if (_localizations.ContainsKey(ResourcesName))
                return _localizations[ResourcesName];

            var sqlStringLocalizer = new SqlStringLocalizer(LoadAllLocalizationResources());
            return _localizations.GetOrAdd(ResourcesName, sqlStringLocalizer);
        }

        public IStringLocalizer Create(Type resourceSource)
        {
            if (_localizations.ContainsKey(ResourcesName))
                return _localizations[ResourcesName];

            var sqlStringLocalizer = new SqlStringLocalizer(LoadAllLocalizationResources());
            return _localizations.GetOrAdd(ResourcesName, sqlStringLocalizer);
        }

        protected IDictionary<string, string> LoadAllLocalizationResources()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dataContext = scope.ServiceProvider.GetService<AppDataContext>();
                var languageResources = dataContext.LanguageResources
                    .Include(lr => lr.Language)
                    .ToDictionary(kvp => $"{kvp.Key}.{kvp.Language.Culture.ToLowerInvariant()}", kvp => kvp.Value);

                return languageResources;
            }
        }
    }
}