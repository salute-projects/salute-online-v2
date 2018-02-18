using System.Collections.Generic;
using SaluteOnline.Shared.Common;

namespace SaluteOnline.API.DTO.Club
{
    public class ClubInfoAggregation
    {
        public int Count { get; set; }
        public IEnumerable<KeyValuePair<ClubStatus, int>> ByStatus { get; set; } = new List<KeyValuePair<ClubStatus, int>>();
        public IEnumerable<KeyValuePair<string, IEnumerable<string>>> Geography { get; set; } = new List<KeyValuePair<string, IEnumerable<string>>>();

    }
}
