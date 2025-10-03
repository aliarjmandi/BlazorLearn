using System.ComponentModel.DataAnnotations;
using System.Globalization;

public sealed class DateRangeInvariantAttribute : ValidationAttribute
{
    private readonly DateTime _min, _max;

    public DateRangeInvariantAttribute(string minInclusive, string maxInclusive)
    {
        _min = DateTime.ParseExact(minInclusive, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        _max = DateTime.ParseExact(maxInclusive, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        ErrorMessage = ErrorMessage ?? "تاریخ خارج از محدوده است.";
    }

    public override bool IsValid(object? value)
    {
        if (value is null) return true;        // اختیاری بودن فیلد
        if (value is DateTime dt)
            return dt >= _min && dt <= _max;
        return false;
    }
}
