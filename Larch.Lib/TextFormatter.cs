﻿using Larch.Lib.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Larch.Lib
{
    public class TextFormatter : IFormatter
    {
        private static string defautDatetimeFormat = "o"; // ISO 8601
        public bool DisableSorting { get; set; }
        public bool QuoteEmptyStrings { get; set; }
        public bool DisableTimestamp { get; set; }
        public string TimestampFormat { get; set; }

        public string Format(Entry entry)
        {
            var sBuilder = new StringBuilder();
            IEnumerable<string> keys;
            if (DisableSorting)
            {
                keys = entry.Data.Keys.ToList();
            }
            else
            {
                keys = new SortedSet<string>(entry.Data.Keys.ToList());
            }

            if (!DisableTimestamp)
            {
                AppendKeyValue(sBuilder, "time", entry.Timestamp.ToString(string.IsNullOrEmpty(TimestampFormat) ? defautDatetimeFormat : TimestampFormat));
            }
            AppendKeyValue(sBuilder, "level", entry.Level);
            if (!string.IsNullOrEmpty(entry.Message))
            {
                AppendKeyValue(sBuilder, "msg", entry.Message);
            }
            foreach (var key in keys)
            {
                AppendKeyValue(sBuilder, key, entry.Data[key]);
            }
            sBuilder.AppendLine();
            return sBuilder.ToString();
        }

        private void AppendKeyValue(StringBuilder sBuilder, string key, object value)
        {
            sBuilder.Append($"{key}=");
            AppendValue(sBuilder, value);
        }

        private void AppendValue(StringBuilder sBuilder, object value)
        {
            var str = value.ToString();
            if (NeedsQouting(str))
                sBuilder.Append($"\"{str}\"");
            else
                sBuilder.Append(str);
        }

        private bool NeedsQouting(string text)
        {
            if (QuoteEmptyStrings && string.IsNullOrEmpty(text))
            {
                return true;
            }

            foreach (var c in text)
            {
                if (!((c >= 'a' && c <= 'z') ||
            (c >= 'A' && c <= 'Z') ||
            (c >= '0' && c <= '9') ||
            c == '-' || c == '.' || c == '_' || c == '/' || c == '@' || c == '^' || c == '+'))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
