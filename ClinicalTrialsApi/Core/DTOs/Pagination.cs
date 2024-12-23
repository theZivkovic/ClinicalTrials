using Json.Schema;
using LanguageExt;

namespace ClinicalTrialsApi.Core.DTOs
{
    public class Pagination
    {
        public const int MaxLimit = 50;
        public int Limit { get; set; } = MaxLimit;
        
        public int Offset { get; set; } = 0;

        public static Option<Pagination> ParseFromRequest(IHeaderDictionary headers)
        {
            var offset = 0;
            var limit = 0;

            var offsetHeader = headers["x-pagination-offset"];

            if (!offsetHeader.Any())
            {
                offset = 0;
            }

            bool offsetParsingOk = Int32.TryParse(offsetHeader, out offset);

            if (offset < 0)
            {
                offset = 0;
            }

            var limitHeader = headers["x-pagination-limit"];

            if (!limitHeader.Any())
            {
                limit = MaxLimit;
            }
            else
            {
                bool limitParsingOk = Int32.TryParse(limitHeader, out limit);
                if (!limitParsingOk)
                {
                    return Option<Pagination>.None;
                }
                if (limit > MaxLimit)
                {
                    limit = MaxLimit;
                }
                if (limit < 0)
                {
                    limit = 0;
                }
            }

            return Option<Pagination>.Some(new Pagination
            {
                Limit = limit,
                Offset = offset
            });
        }
    }
}
