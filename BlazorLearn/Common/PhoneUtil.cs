using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BlazorLearn.Common
{
    /// <summary>
    /// ابزار نرمال‌سازی شماره تلفن به قالب E.164 (با تمرکز روی ایران).
    /// مثال‌ها:
    ///  - "09121234567"  -> "+989121234567"
    ///  - "9121234567"   -> "+989121234567"
    ///  - "+989121234567" (همان می‌ماند)
    ///  - "00989121234567" -> "+989121234567"
    ///  - "۰۹۱۲-۱۲۳-۴۵۶۷" -> "+989121234567"
    ///  - ورودی نامعتبر -> null
    /// </summary>
    public static class PhoneUtil
    {
        // حداقل/حداکثر طول E.164 (بدون +)
        private const int MinE164Digits = 8;
        private const int MaxE164Digits = 15;

        /// <summary>
        /// تلاش برای تبدیل به E.164. اگر موفق بود رشتهٔ E.164 برمی‌گرداند، وگرنه null.
        /// </summary>
        public static string? ToE164(string? input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return null;

            // 1) نرمال‌سازی رقم‌ها و حذف نویزهای ظاهری
            var s = NormalizeDigits(input).Trim();
            s = StripNoise(s);

            // اگر از قبل با + شروع می‌شود، همان را اعتبارسنجی کن
            if (s.StartsWith("+"))
            {
                return IsValidE164(s) ? s : null;
            }

            // 2) حالات بین‌المللی بدون '+'
            // "0098..."  -> "+98..."
            if (s.StartsWith("00"))
            {
                var international = "+" + s.Substring(2);
                return IsValidE164(international) ? international : null;
            }

            // "98..." → "+98..."  (بعضی جاها + را نمی‌گذارند)
            if (s.StartsWith("98"))
            {
                var international = "+" + s;
                return IsValidE164(international) ? international : null;
            }

            // 3) حالات داخلی ایران
            // "09xxxxxxxxx" → "+98" + (بدون صفر)
            if (s.Length >= 10 && s.StartsWith("0"))
            {
                var candidate = "+98" + s.Substring(1);
                return IsValidE164(candidate) ? candidate : null;
            }

            // "9xxxxxxxxx" (بدون صفر ابتدایی) → "+98" + همان
            if (s.Length == 10 && s.StartsWith("9"))
            {
                var candidate = "+98" + s;
                return IsValidE164(candidate) ? candidate : null;
            }

            // سایر حالات: نامشخص
            return null;
        }

        /// <summary>
        /// نسخهٔ Try برای تبدیل به E.164.
        /// </summary>
        public static bool TryToE164(string? input, out string? e164)
        {
            e164 = ToE164(input);
            return e164 is not null;
        }

        /// <summary>
        /// ارقام فارسی/عربی را به ارقام لاتین تبدیل می‌کند.
        /// </summary>
        public static string NormalizeDigits(string value)
        {
            if (string.IsNullOrEmpty(value))
                return value;

            var sb = new StringBuilder(value.Length);

            foreach (var ch in value)
            {
                // فارسی: ۰..۹ (U+06F0..U+06F9)
                if (ch >= '۰' && ch <= '۹')
                {
                    sb.Append((char)('0' + (ch - '۰')));
                }
                // عربی: ٠..٩ (U+0660..U+0669)
                else if (ch >= '٠' && ch <= '٩')
                {
                    sb.Append((char)('0' + (ch - '٠')));
                }
                else
                {
                    sb.Append(ch);
                }
            }

            return sb.ToString();
        }

        /// <summary>
        /// نویزهای رایج در ورودی شماره (فاصله، خط تیره، پرانتز، نقطه و ...) را حذف می‌کند.
        /// فقط + (در ابتدای رشته) و ارقام را نگه می‌دارد.
        /// </summary>
        private static string StripNoise(string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            s = s.Replace(" ", "")
                 .Replace("-", "")
                 .Replace("(", "")
                 .Replace(")", "")
                 .Replace(".", "")
                 .Replace("_", "")
                 .Replace("/", "")
                 .Replace("\\", "")
                 .Replace("ـ", ""); // کشیده

            // اگر چند + وجود دارد یا + وسط/انتهای متن آمده، همه را حذف کن
            // فقط اگر از اول '+' بود همان را نگه می‌داریم.
            if (s.Length > 0 && s[0] == '+')
            {
                // + جلو را نگه می‌داریم، بقیه غیر عدد حذف شوند
                var rest = new string(s.Skip(1).Where(char.IsDigit).ToArray());
                return "+" + rest;
            }

            // در غیر این صورت فقط ارقام را نگه می‌داریم
            return new string(s.Where(char.IsDigit).ToArray());
        }

        /// <summary>
        /// اعتبارسنجی ساده E.164: باید با + شروع شود و بین 8 تا 15 رقم بعد از آن داشته باشد.
        /// </summary>
        private static bool IsValidE164(string s)
        {
            if (string.IsNullOrEmpty(s) || s[0] != '+')
                return false;

            var digits = s.AsSpan(1);
            if (digits.Length < MinE164Digits || digits.Length > MaxE164Digits)
                return false;

            for (int i = 0; i < digits.Length; i++)
                if (!char.IsDigit(digits[i])) return false;

            return true;
        }

        /// <summary>
        /// نمایش داخلی ایران از E.164: "+98912xxxxxxx" → "0912xxxxxxx"
        /// اگر E.164 معتبر نباشد، null برمی‌گرداند.
        /// </summary>
        public static string? ToIranDomestic(string? e164)
        {
            if (!IsValidE164(e164 ?? "")) return null;
            if (!e164!.StartsWith("+98")) return null; // فقط ایران

            var rest = e164.Substring(3); // بعد از 98
            // موبایل‌ها عموماً 9xxxxxxxxx هستند → 0 + 9...
            return "0" + rest;
        }
    }
}

