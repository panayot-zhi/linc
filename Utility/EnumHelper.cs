using System.ComponentModel.DataAnnotations;
using System.Reflection;
using linc.Models.Enumerations;
using Serilog;

namespace linc.Utility
{
    public static class EnumHelper<T> 
        where T : struct, Enum
    {
        public static IList<T> GetValues(Enum value)
        {
            var enumValues = new List<T>();

            foreach (var fi in value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public))
            {
                enumValues.Add((T) Enum.Parse(value.GetType(), fi.Name, false));
            }

            return enumValues;
        }

        public static T Parse(string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }

        public static IList<string> GetNames(Enum value)
        {
            return value.GetType().GetFields(BindingFlags.Static | BindingFlags.Public).Select(fi => fi.Name).ToList();
        }

        public static IList<string> GetDisplayNames(Enum value)
        {
            return GetNames(value).Select(x => GetDisplayName(Parse(x))).ToList();
        }

        public static string GetDisplayName(T value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null)
            {
                return value.ToString();
            }

            if (fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), 
                    inherit: false) is not DisplayAttribute[] displayAttributes)
            {
                return value.ToString();
            }

            var displayAttribute = displayAttributes.FirstOrDefault();
            if (displayAttribute == null)
            {
                return value.ToString();
            }

            if (displayAttribute.ResourceType != null)
            {
                return LookupResource(displayAttribute.ResourceType, displayAttribute.Name);
            }

            return displayAttribute.Name;
        }

        private static string LookupResource(IReflect resourceManagerProvider, string resourceKey)
        {
            try
            {
                var resourceKeyProperty = resourceManagerProvider.GetProperty(resourceKey,
                    BindingFlags.Static | BindingFlags.Public, null, typeof(string),
                    Type.EmptyTypes, null);

                if (resourceKeyProperty != null && resourceKeyProperty.GetMethod != null)
                {
                    return (string) resourceKeyProperty.GetMethod.Invoke(null, null);
                }
            }
            catch(Exception ex)
            {
                var currentCulture = Thread.CurrentThread.CurrentCulture;
                var currentUiCulture = Thread.CurrentThread.CurrentUICulture;

                var logger = Log.Logger.ForContext<IndexCategory>();
                logger.Error(ex, "Could not find key for '{ResourceKey}' for the current culture ({Culture}/UI:{UICulture})",
                    resourceKey, currentCulture, currentUiCulture);
            }

            // Fallback with the key name
            return resourceKey;
        }
    }
}
