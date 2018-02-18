using System;
using System.Collections.Generic;
using System.Net;
using Mapster;
using Microsoft.Extensions.Logging;
using SaluteOnline.CommonDataService.DAL;
using SaluteOnline.CommonDataService.Domain;
using SaluteOnline.CommonDataService.DTO;
using SaluteOnline.CommonDataService.Service.Declaration;
using SaluteOnline.Shared.Exceptions;

namespace SaluteOnline.CommonDataService.Service.Implementation
{
    public class CommonService : ICommonService
    {
        private readonly ILogger<CommonService> _logger;
        private readonly IGenericRepository<Country> _repository;

        public CommonService(IGenericRepository<Country> repository, ILogger<CommonService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public List<CountryDto> GetContriesList()
        {
            try
            {
                return _repository.GetAsQueryable().Adapt<List<CountryDto>>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw new SoException("Internal error", HttpStatusCode.InternalServerError);
            }
        }
    }
}
