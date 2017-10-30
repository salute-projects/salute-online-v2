using SaluteOnline.Domain.Domain.Mongo;
using SaluteOnline.Domain.DTO.Common;

namespace SaluteOnline.Domain.Conversion
{
    public static class CommonMappings
    {
        public static CountryDto ToDto(this Country country)
        {
            return new CountryDto
            {
                Code = country.Code,
                Name = country.Name
            };
        }
    }
}
