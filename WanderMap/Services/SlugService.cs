using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using Unidecode.NET;

namespace WanderMap.Services
{
    public interface ISlugService
    {
        string GenerateSlug(string phrase);
        Task<string> GenerateUniqueSlugAsync<T>(DbContext context,
                                                string phrase,
                                                Expression<Func<T, bool>>? excludePredicate = null,
                                                CancellationToken cancellation = default)
            where T : class;
    }

    public class SlugService : ISlugService
    {
        public string GenerateSlug(string phrase)
        {
            var translit = phrase.Unidecode();
            var str = translit.Normalize(NormalizationForm.FormD);
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    sb.Append(c);
            }
            var cleaned = Regex.Replace(sb.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant(),
                                        @"[^a-z0-9\s-]", "");
            cleaned = Regex.Replace(cleaned, @"\s+", " ").Trim();
            cleaned = Regex.Replace(cleaned, @"\s", "-").Trim('-');

            return string.IsNullOrEmpty(cleaned) ? "item" : cleaned;
        }

        public async Task<string> GenerateUniqueSlugAsync<T>(DbContext context,
                                                             string phrase,
                                                             Expression<Func<T, bool>>? excludePredicate = null,
                                                             CancellationToken cancellation = default)
            where T : class
        {
            var baseSlug = GenerateSlug(phrase);

            const int maxLength = 120;
            if (baseSlug.Length > maxLength) baseSlug = baseSlug[..maxLength];

            var candidate = baseSlug;
            var i = 1;

            var set = context.Set<T>().AsQueryable();

            // Якщо є excludePredicate (наприклад при редагуванні), будемо ігнорувати поточну сутність
            if (excludePredicate != null)
                set = set.Where(Expression.Lambda<Func<T, bool>>(Expression.Not(excludePredicate.Body), excludePredicate.Parameters));

            var param = Expression.Parameter(typeof(T), "e");
            var prop = Expression.PropertyOrField(param, "Slug");
            var candidateConst = Expression.Constant(candidate);
            var equals = Expression.Equal(prop, candidateConst);
            var lambda = Expression.Lambda<Func<T, bool>>(equals, param);

            while (await set.AnyAsync(lambda, cancellation))
            {
                candidate = $"{baseSlug}-{i}";
                if (candidate.Length > maxLength)
                {
                    var suffix = $"-{i}";
                    var allowedBaseLen = maxLength - suffix.Length;
                    if (allowedBaseLen <= 0)
                        candidate = $"{baseSlug}-{i}";
                    else
                        candidate = $"{baseSlug[..allowedBaseLen]}{suffix}";
                }

                candidateConst = Expression.Constant(candidate);
                equals = Expression.Equal(prop, candidateConst);
                lambda = Expression.Lambda<Func<T, bool>>(equals, param);

                i++;
            }

            return candidate;
        }
    }
}
