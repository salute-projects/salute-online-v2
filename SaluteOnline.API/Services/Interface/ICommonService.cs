using System.Collections.Generic;
using SaluteOnline.Domain.DTO.Common;

namespace SaluteOnline.API.Services.Interface
{
    public interface ICommonService
    {
        List<CountryDto> GetContriesList();
    }
}