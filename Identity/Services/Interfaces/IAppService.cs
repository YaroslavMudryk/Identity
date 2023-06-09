﻿using Identity.Dtos;
using Identity.Helpers;
using Identity.ViewModels;

namespace Identity.Services.Interfaces
{
    public interface IAppService
    {
        Task<Result<AppSecretViewModel>> ChangeAppSecretAsync(int id);
        Task<Result<AppViewModel>> CreateAppAsync(AppDto appDto);
        Task<Result<bool>> DeleteAppAsync(int id);
        Task<Result<List<AppViewModel>>> GetAllAppsAsync(int page);
        Task<Result<AppViewModel>> GetAppByIdAsync(int id);
        Task<Result<AppSecretViewModel>> GetAppSecretAsync(int id);
        Task<Result<List<AppViewModel>>> GetMyAppsAsync();
        Task<Result<AppViewModel>> UpdateAppAsync(AppDto appDto);
        Task<Result<bool>> UpdateAppClaimsAsync(AppClaimsDto appClaims);
    }
}
