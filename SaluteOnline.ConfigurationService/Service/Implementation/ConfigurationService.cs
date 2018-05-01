using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Mapster;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SaluteOnline.ConfigurationService.DAL;
using SaluteOnline.ConfigurationService.Domain;
using SaluteOnline.ConfigurationService.Service.Declaration;
using SaluteOnline.Shared.Common;
using SaluteOnline.Shared.Exceptions;
using SaluteOnline.Shared.Helpers;

namespace SaluteOnline.ConfigurationService.Service.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IGenericRepository<DashboardConfiguration> _dashboardRepository;
        private readonly IGenericRepository<ClubDashboardConfiguration> _clubDashboardRepository;
        private readonly IGenericRepository<DefaultDashboardConfiguration> _defaultDashboardConfigurationRepository;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ConfigurationService> _logger;

        public ConfigurationService(IGenericRepository<DashboardConfiguration> dashboardRepository, IGenericRepository<ClubDashboardConfiguration> clubDashboardRepository,
            IMemoryCache memoryCache, ILogger<ConfigurationService> logger, IGenericRepository<DefaultDashboardConfiguration> defaultDashboardConfigurationRepository)
        {
            _memoryCache = memoryCache;
            _dashboardRepository = dashboardRepository;
            _defaultDashboardConfigurationRepository = defaultDashboardConfigurationRepository;
            _clubDashboardRepository = clubDashboardRepository;
            _logger = logger;
        }

        public void SaveDashboardConfiguration(List<DashboardConfigurationItem> panels, string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                if (panels == null || !panels.Any())
                    return;

                _memoryCache.Remove("Dashboard:" + subjectId);

                var existing = _dashboardRepository.GetById(userGuid);
                if (existing == null)
                {
                    var config = new DashboardConfiguration
                    {
                        Guid = userGuid,
                        LastChanged = DateTimeOffset.UtcNow,
                        Panels = panels
                    };
                    _dashboardRepository.Insert(config);
                }
                else
                {
                    existing.LastChanged = DateTimeOffset.UtcNow;
                    existing.Panels = panels;
                    _dashboardRepository.Update(existing);
                }

                _memoryCache.Set("Dashboard:" + subjectId, existing);
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while saving dashboard configuration. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public DashboardConfiguration GetDashboardConfiguration(string subjectId, string role)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid) || string.IsNullOrEmpty(role))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                if (_memoryCache.TryGetValue("Dashboard:" + subjectId, out DashboardConfiguration inMemory) && inMemory != null)
                    return inMemory;

                var configuration = _dashboardRepository.GetById(userGuid);
                if (configuration != null)
                {
                    _memoryCache.Set("Dashboard:" + subjectId, inMemory);
                    return configuration;
                }

                switch (RolesHelper.ParseRole(role))
                {
                    case Roles.User:
                        return
                            _defaultDashboardConfigurationRepository.Get(t => t.ForRole == Roles.User)
                                .SingleOrDefault()
                                .Adapt<DashboardConfiguration>();
                    case Roles.SilentDon:
                        return
                            _defaultDashboardConfigurationRepository.Get(t => t.ForRole == Roles.SilentDon)
                                .SingleOrDefault()
                                .Adapt<DashboardConfiguration>();
                    case Roles.None:
                        return new DashboardConfiguration();
                    // todo get personalized configurations for each role
                    default:
                        return new DashboardConfiguration();
                }
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while loading dashboard configuration. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public void SaveClubDashboardConfiguration(ClubDashboardConfiguration configuration, string subjectId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                if (configuration?.Panels == null || !configuration.Panels.Any())
                    return;

                _memoryCache.Remove($"ClubDashboard|{configuration.ClubId}|{subjectId}");

                var existing = _clubDashboardRepository.GetAsQueryable(t => t.ClubId == configuration.ClubId && t.UserGuid == userGuid).SingleOrDefault();
                if (existing == null)
                {
                    var config = new ClubDashboardConfiguration
                    {
                        Guid = Guid.NewGuid(),
                        ClubId = configuration.ClubId,
                        UserGuid = userGuid,
                        LastChanged = DateTimeOffset.UtcNow,
                        Panels = configuration.Panels
                    };
                    _clubDashboardRepository.Insert(config);
                }
                else
                {
                    existing.LastChanged = DateTimeOffset.UtcNow;
                    existing.Panels = configuration.Panels;
                    _clubDashboardRepository.Update(existing);
                }

                _memoryCache.Set($"ClubDashboard|{configuration.ClubId}|{subjectId}", existing);
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while saving club dashboard settings. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }

        public ClubDashboardConfiguration GetClubDashboardConfiguration(string subjectId, string role, int clubId)
        {
            try
            {
                if (!Guid.TryParse(subjectId, out var userGuid) || string.IsNullOrEmpty(role))
                    throw new SoException("Corrupted token", HttpStatusCode.Unauthorized);

                if (clubId == default(int))
                    throw new SoException("Wrong club id", HttpStatusCode.BadRequest);

                if (_memoryCache.TryGetValue($"ClubDashboard|{clubId}|{subjectId}", out ClubDashboardConfiguration inMemory) && inMemory != null)
                    return inMemory;

                var configuration = _clubDashboardRepository.GetAsQueryable(t => t.ClubId == clubId && t.UserGuid == userGuid).SingleOrDefault();
                if (configuration == null)
                    // todo return default club dashboard configuration
                    return new ClubDashboardConfiguration();

                _memoryCache.Set($"ClubDashboard|{configuration.ClubId}|{subjectId}", configuration);
                return configuration;
            }
            catch (Exception ex)
            {
                if (ex is SoException)
                    throw;
                _logger.LogError(ex.Message);
                throw new SoException("Error while loading dashboard configuration. Please try a bit later", HttpStatusCode.InternalServerError);
            }
        }
    }
}
