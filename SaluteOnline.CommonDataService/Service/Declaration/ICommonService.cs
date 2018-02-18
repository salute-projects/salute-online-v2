using System.Collections.Generic;
using SaluteOnline.CommonDataService.DTO;

namespace SaluteOnline.CommonDataService.Service.Declaration
{
    public interface ICommonService
    {
        List<CountryDto> GetContriesList();
    }
}