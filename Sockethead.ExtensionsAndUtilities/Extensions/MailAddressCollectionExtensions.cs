using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Sockethead.ExtensionsAndUtilities.Extensions
{
    public static class MailAddressCollectionExtensions
    {
        /// <summary>
        /// It takes a list of MailAddress and adds each MailAddress to the collection.
        /// </summary>
        public static void Add(this MailAddressCollection collection, IEnumerable<MailAddress> addresses)
        {
            foreach (var address in addresses)
                collection.Add(address);
        }

        /// <summary>
        /// Parse a string (may contain one or multiple email addresses separated by comma or semicolon) and add to a MailAddressCollection
        /// </summary>
        public static void ParseAndAdd(this MailAddressCollection collection, string addresses)
        {
            if (!string.IsNullOrWhiteSpace(addresses))
                collection.Add(addresses
                    .Split(new[] { ',', ';' })
                    .Select(address => new MailAddress(address.Trim())));
        }
    }
}