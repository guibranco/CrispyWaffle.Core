﻿namespace CrispyWaffle.Extensions
{
    using GoodPractices;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml;
    using Utilities;
    using Validations;

    /// <summary>
    /// Helper class for generic conversions
    /// </summary>
    public static class ConversionExtensions
    {
        /// <summary>
        /// An IEnumerable&lt;String&gt; extension method that converts a binary representation string to the bytes.
        /// </summary>
        /// <param name="input">The string to act on.</param>
        /// <returns>string in binary representation as a Byte[].</returns>
        public static byte[] ToBytes(this IEnumerable<string> input)
        {
            return input.Select(s => Convert.ToByte(s, 2)).ToArray();
        }

        /// <summary>
        /// A String extension method that converts this object to a boolean. This method assumes
        /// that <b>any</b> value other than stated in the optional parameter toTrue will be valid as
        /// false. It is useful for converting binary flags that can only take on two distinct
        /// values​​, or that only one value represents success and any other is invalid.
        /// </summary>
        /// <param name="str">The str to act on.</param>
        /// <param name="validValueForTrue">(Optional) the valid value for true.</param>
        /// <returns>The given data converted to a Boolean.</returns>
        public static bool ToBoolean(this string str, string validValueForTrue = "S")
        {
            return str != null && str.Equals(validValueForTrue, StringExtensions.Comparison);
        }

        /// <summary>
        /// A Boolean extension method that convert this object into a String representation.
        /// </summary>
        /// <param name="boolean">The boolean to act on.</param>
        /// <param name="trueValue">The value if true.</param>
        /// <param name="falseValue">The value if false.</param>
        /// <returns>The given data converted to a String.</returns>
        public static string ToString(this bool boolean, string trueValue, string falseValue)
        {
            return boolean ? trueValue : falseValue;
        }

        /// <summary>
        /// To the date time.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">input</exception>
        /// <exception cref="ArgumentOutOfRangeException">input</exception>
        public static DateTime ToDateTime(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new ArgumentNullException(nameof(input), "Input value cannot be null");
            input = input.ToLower().Trim();
            switch (input)
            {
                case "agora":
                case "now":
                    return DateTime.Now;
                case "hoje":
                case "today":
                    return DateTime.Today;
                case "ontem":
                case "yesterday":
                    return DateTime.Today.AddDays(-1);
                case "amanhã":
                case "amanha":
                case "tomorrow":
                    return DateTime.Today.AddDays(1);
                default:
                    if (DateTime.TryParse(input, out var result))
                        return result;
                    if (input.Length == 10 &&
                        DateTime.TryParseExact(input,
                                               @"dd/MM/yyyy",
                                               CultureInfo.InvariantCulture,
                                               DateTimeStyles.None,
                                               out result))
                        return result;
                    throw new ArgumentOutOfRangeException(nameof(input), input, "Unable to parse the string to a valid datetime");
            }
        }

        /// <summary>
        /// Tries to convert string to date time.
        /// </summary>
        /// <param name="input">The input string a valid DateTime format.</param>
        /// <param name="value">The DateTime value.</param>
        /// <returns><b>True</b> if success, <b>false</b> otherwise</returns>
        public static bool TryToDateTime(this string input, out DateTime value)
        {
            value = DateTime.MinValue;
            if (string.IsNullOrWhiteSpace(input))
                return false;
            input = input.ToLower().Trim();
            switch (input)
            {
                case "agora":
                case "now":
                    value = DateTime.Now;
                    return true;
                case "hoje":
                case "today":
                    value = DateTime.Today;
                    return true;
                case "ontem":
                case "yesterday":
                    value = DateTime.Today.AddDays(-1);
                    return true;
                case "amanhã":
                case "amanha":
                case "tomorrow":
                    value = DateTime.Today.AddDays(1);
                    return true;
                default:
                    if (DateTime.TryParse(input, out value))
                        return true;
                    return input.Length == 10 &&
                           DateTime.TryParseExact(input,
                               @"dd/MM/yyyy",
                               CultureInfo.InvariantCulture,
                               DateTimeStyles.None,
                               out value);
            }
        }

        /// <summary>
        /// Convert a string representation of Int32 to Int32 type
        /// </summary>
        /// <param name="input">The string representation of a Int32</param>
        /// <returns>The string as Int32</returns>
        public static int ToInt32(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;
            var success = int.TryParse(input, NumberStyles.Number, StringExtensions.Culture, out var result);
            return success ? result : 0;
        }

        /// <summary>
        /// To the int64.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>Int64.</returns>
        public static long ToInt64(this string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;
            var success = long.TryParse(input, NumberStyles.Number, StringExtensions.Culture, out var result);
            return success ? result : 0;
        }

        /// <summary>
        /// A String extension method that converts a string to a decimal.
        /// </summary>
        /// <param name="input">The str to act on.</param>
        /// <returns>str as a Decimal.</returns>
        /// <remarks>Guilherme B. Stracini, 27/03/2013.</remarks>
        public static decimal ToDecimal(this string input)
        {
            return string.IsNullOrWhiteSpace(input)
                ? 0M
                : decimal.TryParse(input, NumberStyles.Number, StringExtensions.Culture, out var result)
                       ? result
                       : 0M;
        }

        /// <summary>
        /// An IEnumerable&lt;Byte&gt; extension method
        /// that converts the bytes to a binary string representation
        /// string.
        /// </summary>
        /// <param name="bytes">The bytes to act on.</param>
        /// <returns>bytes as a Binary String[] representation.</returns>
        public static string[] ToBinaryString(this IEnumerable<byte> bytes)
        {
            return bytes.Select(b => Convert.ToString(b, 2).PadLeft(8, '0')).ToArray();
        }

        /// <summary>
        ///  Converts a decimal input to monetary readable format (PT-BR - BRL)
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToMonetary(this decimal input)
        {
            return input == 0 ? "No value" : $@"R${input:### ##0.00}";
        }

        /// <summary>
        ///     Converts a DateTime instance to Unix Timestamp (number of seconds that have elapsed since 
        ///     00:00:00 Coordinated Universal Time (UTC), Thursday, 1 January 1970)
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static int ToUnixTimeStamp(this DateTime dateTime)
        {
            return (int)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        /// <summary>
        ///     Converts a Unix Timestamp (number of seconds that have elapsed since 00:00:00 Coordinated 
        ///     Universal Time (UTC), Thursday, 1 January 1970) to a DateTime instance
        /// </summary>
        /// <param name="epochTime">The Unix Timestamp</param>
        /// <returns>A DateTime instance of the epochTime</returns>
        public static DateTime FromUnixTimeStamp(this int epochTime)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
                    .AddSeconds(Math.Round((double)epochTime / 1000))
                    .ToLocalTime();
        }

        /// <summary>
        /// Parses the phone number.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static PhoneNumber ParsePhoneNumber(this string number)
        {
            var result = new PhoneNumber(0, 0, 0);
            if (number.TryParsePhoneNumber(ref result))
                return result;
            throw new InvalidTelephoneNumberException(number.RemoveNonNumeric());
        }

        /// <summary>
        /// Tries the parse phone number.
        /// </summary>
        /// <param name="number">The phone number as string.</param>
        /// <param name="result">The phone number as PhoneNumber struct.</param>
        /// <returns><b>True</b> if succeed, <b>false</b> otherwise</returns>
        public static bool TryParsePhoneNumber(this string number, ref PhoneNumber result)
        {
            var dirty = number.RemoveNonNumeric();
            var dirtyLength = dirty.Length;
            if (dirty.StartsWith(@"55") && dirtyLength > 11 && dirtyLength < 15)
            {
                dirty = dirty.Remove(0, 2);
                dirtyLength -= 2;
            }

            if (dirtyLength < 10 ||
                dirtyLength > 12)
                return false;
            var prefix = dirty.Substring(0, 1).Equals(@"0", StringExtensions.Comparison) &&
                         (dirtyLength == 11 || dirtyLength == 12)
                             ? dirty.Substring(1, 2)
                             : dirty.Substring(0, 2);
            var hasNineDigits = dirtyLength >= 9 &&
                                dirty.Substring(dirtyLength - 9, 1)
                                     .Equals(@"9", StringExtensions.Comparison);
            var allowedDigits = hasNineDigits ? 9 : 8;
            var telephoneNumber = dirty.Substring(dirtyLength - allowedDigits, allowedDigits);
            result = new PhoneNumber(55, prefix.ToInt32(), telephoneNumber.ToInt64());
            return true;
        }

        /// <summary>
        /// To HTML list string.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <param name="includeListTag">Includes the UL tags in the return string.</param>
        /// <returns>System.String.</returns>
        public static string ToListString(this Exception exception, bool includeListTag = true)
        {
            var builder = new StringBuilder();
            if (includeListTag)
                builder.Append(@"<ul>");

            var inner = exception;
            while (inner != null)
            {
                var stackTrace = inner.StackTrace ?? string.Empty;
                if (ExceptionValidations.StackTracePattern.IsMatch(stackTrace))
                    stackTrace =
                        ExceptionValidations
                            .StackTracePattern
                            .Matches(stackTrace)
                            .Cast<Match>()
                            .Aggregate(string.Empty,
                                       (current, match) =>
                                           current + $@"<li> {match.Groups["method"].Value}</li>");
                builder.AppendFormat(@"<li><b>{0}</b> (<u>{1}</u>)</li>{2}",
                                     inner.Message,
                                     inner.GetType().FullName,
                                     stackTrace);
                inner = inner.InnerException;
            }

            if (includeListTag)
                builder.AppendLine(@"</ul>");
            return builder.ToString();
        }

        /// <summary>
        /// Cleans the list items.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="maxItems">The maximum items.</param>
        /// <returns></returns>
        public static string CleanListItems(this string input, int maxItems = 15)
        {
            input = input?.Replace(@"<ul>", string.Empty).Replace(@"</ul>", string.Empty) ??
                             string.Empty;

            var pattern = new Regex(@"<li class='operation'>(?<operation>.+?)</li>(?:\s+<li>(?:.+?)</li>)*",
                                    RegexOptions.IgnoreCase |
                                    RegexOptions.CultureInvariant |
                                    RegexOptions.Multiline);
            if (!pattern.IsMatch(input) || maxItems <= 0)
                return input;
            var matches = pattern.Matches(input);
            var max = matches.Count > maxItems
                          ? maxItems
                          : matches.Count;
            input = string.Empty;
            for (var i = 1; i <= max; i++)
                input += matches[matches.Count - i];
            return input;
        }

        /// <summary>
        /// To the pretty string.
        /// </summary>
        /// <param name="json">The json.</param>
        /// <returns></returns>
        public static string ToPrettyString(this string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return string.Empty;
            if (!json.IsValidJson())
                return json;
            var parsedJson = JToken.Parse(json);
            return parsedJson.ToString(Newtonsoft.Json.Formatting.Indented);
        }

        /// <summary>
        /// To the ident string.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns>System.String.</returns>
        public static string ToIdentString(this XmlDocument document)
        {
            if (document == null)
                return null;
            var builder = new StringBuilder();
            var settings = new XmlWriterSettings
            {
                Indent = true,
                IndentChars = "\t",
                NewLineChars = "\r\n",
                NewLineHandling = NewLineHandling.Replace
            };
            using (var writer = XmlWriter.Create(builder, settings))
                document.Save(writer);
            return builder.ToString();
        }

        /// <summary>
        /// Calculates the module-10 of a string.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>System.Int32.</returns>
        public static int ToModule10(this string input)
        {
            var number = Regex.Replace(input, "[^0-9]", "");
            var sum = 0;
            var weight = 2;
            var counter = number.Length - 1;
            while (counter >= 0)
            {
                var multiplication = number.Substring(counter, 1).ToInt32() * weight;
                if (multiplication >= 10)
                    multiplication = 1 + (multiplication - 10);
                sum += multiplication;
                weight = weight == 2 ? 1 : 2;
                counter--;
            }
            var digit = 10 - sum % 10;
            if (digit == 10)
                digit = 0;
            return digit;
        }

        /// <summary>
        /// To the ordinal.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string ToOrdinal(this long number)
        {
            if (number < 0)
                return number.ToString();
            var rem = number % 100;
            if (rem >= 11 && rem <= 13)
                return $"{number}th";
            switch (number % 10)
            {
                case 1:
                    return $"{number}st";
                case 2:
                    return $"{number}nd";
                case 3:
                    return $"{number}rd";
                default:
                    return $"{number}th";
            }
        }

        /// <summary>
        /// To the ordinal.
        /// </summary>
        /// <param name="number">The number.</param>
        /// <returns></returns>
        public static string ToOrdinal(this int number)
        {
            return ((long)number).ToOrdinal();
        }

        /// <summary>
        /// Formats the document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public static string FormatDocument(this string document)
        {
            if (string.IsNullOrWhiteSpace(document))
                return "Invalid document";
            var documentPattern = document.Length == 14 ? @"{0:00\.000\.000/0000-00}" : @"{0:000\.000\.000-00}";
            return string.Format(documentPattern, document.RemoveNonNumeric().ToInt64());
        }

        /// <summary>
        /// Formats the zip code.
        /// </summary>
        /// <param name="zipCode">The zip code.</param>
        /// <returns></returns>
        public static string FormatBrazilianZipCode(this string zipCode)
        {
            return Regex.Replace(zipCode.RemoveNonNumeric(), @"(\d{5})(\d{3})", "$1-$2");
        }

        /// <summary>
        /// Deeps the clone.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance">The instance.</param>
        /// <param name="useNonPublic">if set to <c>true</c> [use non public].</param>
        /// <returns></returns>
        public static T DeepClone<T>(this T instance, bool useNonPublic = true)
        {
            var type = typeof(T);
            var ctors = type.GetConstructors().OrderByDescending(c => c.GetParameters().Length);
            foreach (var ctor in ctors)
            {
                var arguments = new List<object>();
                var parameters = ctor.GetParameters();
                foreach (var parameter in parameters)
                {
                    var property = type.GetProperty(parameter.Name, BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance);
                    if (property == null && !useNonPublic)
                        continue;
                    if (property == null)
                    {
                        property = type.GetProperty(parameter.Name, BindingFlags.NonPublic | BindingFlags.IgnoreCase | BindingFlags.Instance);
                        if (property == null)
                            continue;
                    }
                    if (property.PropertyType != parameter.ParameterType)
                        continue;
                    arguments.Add(property.GetValue(instance));
                }
                return (T)ctor.Invoke(arguments.ToArray());
            }
            return default;
        }
    }
}