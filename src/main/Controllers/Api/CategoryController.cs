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
    [Route("api/category")]
    public class CategoryController : Controller
    {
        private readonly IItemRepository repository;
        private readonly UserManager<UserManage> userManager;
        private readonly ILogger<ItemController> logger;

        private readonly IMapper mapper;

        public CategoryController(UserManager<UserManage> _userM, IItemRepository repository, ILogger<ItemController> logger, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = _userM ?? throw new ArgumentNullException(nameof(_userM));
        }

        /// <summary>
        /// Create a new category
        /// </summary>
        /// <param name="category">category model</param>
        /// <returns></returns>  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryViewModel category)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            try
            {
                this.repository.AddCategory(this.mapper.Map<Category>(category));
                return this.Ok("Done");
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the category: {ex}");
                return this.BadRequest("Error Occurred");
            }


        }

        /// <summary>
        /// Get list of categories
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAllCategories(int id)
        {
            try
            {
                if (id == 0)
                {
                    var results = this.repository.GetAllCategories();
                    return this.Ok(results);
                }
                else
                {
                    Category itmv = this.repository.GetCategory(new Category { Id = id });
                    return this.Ok(this.mapper.Map<CategoryViewModel>(itmv));
                }
                
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the item: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Remove a specific category
        /// </summary>
        /// <param name="uid"></param>
        /// <returns>Task with the result of the action</returns>

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<IActionResult> RemoveCategory(int id)
        {
            try
            {
                if (id == 0 )
                {
                    return this.BadRequest("Bad data");
                }
                else
                {
                    Category itmv = this.repository.GetCategory(new Category { Id = id });
                    if (itmv == null)
                    {
                        return this.GetAllCategories(-1);
                    }
                    itmv.Deleted = true;
                    this.repository.DeleteCategory(itmv);
                    return this.Ok("Done");

                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed delete category: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Update the information of an existing category
        /// </summary>
        /// <param name="category">item model</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]

        public async Task<IActionResult> UpdateCategory([FromBody] CategoryViewModel category)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            Category results = this.repository.GetCategory(new Category { Id = category.Id });
            if (results == null || results.Id == 0)
            {
                return this.BadRequest("User does not exist");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(category.Name))
                {
                    results.Name = category.Name;
                }
                this.repository.UpdateCategory(results);
                return this.Ok(results);
            }

        }
    }
}