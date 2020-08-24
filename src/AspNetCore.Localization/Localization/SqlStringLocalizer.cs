using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AspNetCore.Localization.Localization
{
    public class SqlStringLocalizer : IStringLocalizer
    {
        private readonly IDictionary<string, string> _localizations;

        public SqlStringLocalizer(IDictionary<string, string> localizations)
        {
            _localizations = localizations;
        }

        public LocalizedString this[string name] => new LocalizedString(name, GetLocalizationString(name));

        public LocalizedString this[string name, params object[] arguments]
        {
            get
            {
                var formattedResource = string.Format(GetLocalizationString(name), arguments);
                return new LocalizedString(name, formattedResource);
            }
        }

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures) => _localizations.Select(l => new LocalizedString(l.Key, l.Value));

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }

        protected string GetLocalizationString(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return string.Empty;

            string newName = name.Trim().ToLowerInvariant();
            var culture = CultureInfo.CurrentCulture.ToString().ToLowerInvariant();
            var computedName = $"{newName}.{culture}";

            if (_localizations.TryGetValue(computedName, out string resource))
                return resource;

            return newName;
        }
    }
}