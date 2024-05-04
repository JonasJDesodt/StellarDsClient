using System.Collections.Specialized;

namespace StellarDsClient.Ui.Mvc.Extensions
{
    internal static class NameValueCollectionExtensions
    {
        internal static IDictionary<string, string> ToDictionary(this NameValueCollection nameValueCollection)
        {
            var dictionary = new Dictionary<string, string>();

            foreach (var key in nameValueCollection.AllKeys)
            {
                dictionary.Add($"{key}", $"{nameValueCollection[key]}");
            }

            return dictionary;
        }
    }
}
