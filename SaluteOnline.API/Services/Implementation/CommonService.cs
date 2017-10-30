using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SaluteOnline.API.DAL;
using SaluteOnline.API.Services.Interface;
using SaluteOnline.Domain.Conversion;
using SaluteOnline.Domain.DTO.Common;

namespace SaluteOnline.API.Services.Implementation
{
    public class CommonService : ICommonService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger _logger;

        public CommonService(IUnitOfWork unitOfWork, ILogger<AccountService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public List<CountryDto> GetContriesList()
        {
            try
            {
                return _unitOfWork.Countries.Get().Select(t => t.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
