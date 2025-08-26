using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorLearn.Common
{
    public static class SlugUtility
    {
        // تبدیل فارسی → فینگلیش ساده
        private static readonly Dictionary<char, string> FaToEn = new()
        {
            ['ا'] = "a",
            ['آ'] = "a",
            ['ب'] = "b",
            ['پ'] = "p",
            ['ت'] = "t",
            ['ث'] = "s",
            ['ج'] = "j",
            ['چ'] = "ch",
            ['ح'] = "h",
            ['خ'] = "kh",
            ['د'] = "d",
            ['ذ'] = "z",
            ['ر'] = "r",
            ['ز'] = "z",
            ['ژ'] = "zh",
            ['س'] = "s",
            ['ش'] = "sh",
            ['ص'] = "s",
            ['ض'] = "z",
            ['ط'] = "t",
            ['ظ'] = "z",
            ['ع'] = "a",
            ['غ'] = "gh",
            ['ف'] = "f",
            ['ق'] = "gh",
            ['ک'] = "k",
            ['گ'] = "g",
            ['ل'] = "l",
            ['م'] = "m",
            ['ن'] = "n",
            ['و'] = "v",
            ['ه'] = "h",
            ['ی'] = "y",
            ['ء'] = "",
            ['‌'] = "-", // نیم‌فاصله
            ['ٔ'] = "",
            // اعداد فارسی
            ['۰'] = "0",
            ['۱'] = "1",
            ['۲'] = "2",
            ['۳'] = "3",
            ['۴'] = "4",
            ['۵'] = "5",
            ['۶'] = "6",
            ['۷'] = "7",
            ['۸'] = "8",
            ['۹'] = "9",
        };

        /// <summary>
        /// ورودی فارسی/انگلیسی → slug فقط با [a-z0-9-]
        /// </summary>
        public static string Slugify(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Trim
            input = input.Trim();

            // فارسی→لاتین (فینگلیش ساده)
            var sb = new StringBuilder(input.Length * 2);
            foreach (var ch in input)
            {
                if (FaToEn.TryGetValue(ch, out var map)) { sb.Append(map); continue; }
                // حروف عربی مشابه
                var ch2 = ch switch
                {
                    'ك' => 'ک',
                    'ي' => 'ی',
                    'ئ' => 'ی',
                    'ؤ' => 'و',
                    _ => ch
                };
                if (FaToEn.TryGetValue(ch2, out map)) { sb.Append(map); continue; }
                sb.Append(ch2);
            }
            var translit = sb.ToString();

            // به حروف کوچک
            translit = translit.ToLowerInvariant();

            // حذف اعراب/دی‌اکریتیک
            translit = RemoveDiacritics(translit);

            // هر چیزی غیر از a-z0-9 را به خط تیره تبدیل کن
            translit = Regex.Replace(translit, @"[^a-z0-9]+", "-");

            // خط‌تیره‌های تکراری و ابتدا/انتها
            translit = Regex.Replace(translit, @"-+", "-").Trim('-');

            return translit;
        }

        private static string RemoveDiacritics(string text)
        {
            var norm = text.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in norm)
            {
                var uc = CharUnicodeInfo.GetUnicodeCategory(c);
                if (uc != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            return sb.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
