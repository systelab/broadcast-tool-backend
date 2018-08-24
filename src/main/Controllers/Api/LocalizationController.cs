namespace Main.Controllers.Api
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;

    using Main.Models;
    using Main.Services;
    using Main.ViewModels;

    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    [EnableCors("MyPolicy")]
    [Route("api/[controller]")]
    public class LocalizationController : Controller
    {
        private readonly IItemRepository repository;
        private readonly UserManager<UserManage> userManager;
        private readonly ILogger<LocalizationController> logger;

        private readonly IMapper mapper;

        public LocalizationController(UserManager<UserManage> _userM, IItemRepository repository, ILogger<LocalizationController> logger, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = _userM ?? throw new ArgumentNullException(nameof(_userM));
        }

        /// <summary>
        /// Create a new localization
        /// </summary>
        /// <param name="localization">localization model</param>
        /// <returns></returns>  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] LocalizationViewModel localization)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            try
            {
                this.repository.AddLocalization(this.mapper.Map<Localization>(localization));
                return this.Ok("Done");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the localization: {ex}");
                return this.BadRequest("Error Occurred");
            }


        }

        /// <summary>
        /// Get list of localizations
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Get(int id)
        {
            try
            {
                if (id == 0)
                {
                    var results = this.repository.GetAllLocalizations();
                    return this.Ok(results);
                }
                else
                {
                    Localization itmv = this.repository.GetLocalization(new Localization { Id = id });
                    return this.Ok(this.mapper.Map<LocalizationViewModel>(itmv));
                }
                
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the localization: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Remove a specific localization
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>Task with the result of the action</returns>

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<IActionResult> Remove(int id)
        {
            try
            {
                if (id == 0 )
                {
                    return this.BadRequest("Bad data");
                }
                else
                {
                    Localization itmv = this.repository.GetLocalization(new Localization { Id = id });
                    if (itmv == null)
                    {
                        return this.Get(-1);
                    }
                    itmv.Deleted = true;
                    this.repository.DeleteLocalization(itmv);
                    return this.Ok("Done");

                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed delete localization: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Update the information of an existing localization
        /// </summary>
        /// <param name="localization">item model</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]

        public async Task<IActionResult> Update([FromBody] LocalizationViewModel localization)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            Localization results = this.repository.GetLocalization(new Localization { Id = localization.Id });
            if (results == null || results.Id == 0)
            {
                return this.BadRequest("User does not exist");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(localization.Name))
                {
                    results.Name = localization.Name;
                }
                this.repository.UpdateLocalization(results);
                return this.Ok(results);
            }

        }
    }
}