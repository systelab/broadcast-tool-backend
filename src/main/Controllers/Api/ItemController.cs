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
    [Route("api/item")]
    public class ItemController : Controller
    {
        private readonly IItemRepository repository;
        private readonly UserManager<UserManage> userManager;
        private readonly ILogger<ItemController> logger;

        private readonly IMapper mapper;

        public ItemController(UserManager<UserManage> _userM, IItemRepository repository, ILogger<ItemController> logger, IMapper mapper)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = _userM ?? throw new ArgumentNullException(nameof(_userM));
        }

        /// <summary>
        /// Create a new item in the database
        /// </summary>
        /// <param name="item">item model</param>
        /// <returns>Task returning the status of the action</returns>  
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        public async Task<IActionResult> CreateItem(ItemViewModel item)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }

            try
            {
                // Save to the database
                var user = await this.userManager.FindByNameAsync(this.User.Identity.Name);
                var newItem = this.mapper.Map<Item>(item);
                newItem.Username = user.UserName;
                newItem.Email = user.Email;
                newItem.Dob = DateTime.Now;
                newItem.Path = "";
                newItem.Type = 1;

                newItem.ExpirationDate = DateTime.ParseExact(item.expDate, "dd/MM/yyyy", null);
                this.repository.AddItem(newItem);
                return this.Ok(this.mapper.Map<ItemViewModel>(newItem));
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to create the item: {ex}");
                return this.BadRequest("Error Occurred");
            }


        }
        /// <summary>
        /// Upload Image for a item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        [Route("upload")]
        [HttpPost, DisableRequestSizeLimit]
        public ActionResult UploadFile(int id)
        {
            try
            {
                var file = Request.Form.Files[0];
                if (file.Length > 0)
                {
                    if(this.repository.AddFile(file, id))
                    {
                        return Json("OK");
                    }
                    else
                    {
                        return Json("Upload Failed");
                    }
                   
                }
                return Json("Upload Failed: " );
            }
            catch (System.Exception ex)
            {
                return Json("Upload Failed: " + ex.Message);
            }
        }
        /// <summary>
        /// Get list of all the items stored in the database
        /// </summary>
        /// <returns>result of the action</returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        public IActionResult GetAllItems(int id,int localization)
        {
            try
            {
                if (id == 0)
                {
                    var results = this.repository.GetAllItems(localization);
                    return this.Ok(results);
                }
                else if (id == -1)
                {
                    var user =  this.userManager.FindByNameAsync(this.User.Identity.Name);
                    if( user.Result.Admin == true)
                    {
                        var results = this.repository.GetAllItems(0);
                        return this.Ok(results);
                    }
                    else
                    {
                        var results = this.repository.GetAllItemsByUser(this.User.Identity.Name);
                        return this.Ok(results);
                    }
                }
                else
                {
                    ItemViewModel itmv = this.repository.GetItem(new Item { Id = id });
                    itmv.access = 0;
                    var user = this.userManager.FindByNameAsync(this.User.Identity.Name);
                    if (itmv.Username == this.User.Identity.Name || user.Result.Admin == true)
                    {
                        itmv.access = 1;
                    }
                    else
                    {
                        this.repository.AddItem_Access_Stat(new Stats_Item { IdItem = itmv.Id, Dob = DateTime.Now });
                    }
                    return this.Ok(itmv);
                }
                
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the item: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }
        [Route("anonymous")]
        [HttpGet]
        public IActionResult GetAllItemsAnonymous(int id, int localization)
        {
            try
            {
                if (id == 0)
                {
                    var results = this.repository.GetAllItemsAnonymous(localization);
                    return this.Ok(results);
                }
                else if (id == -1)
                {
                    var results = this.repository.GetAllItemsAnonymousWall();
                    return this.Ok(results);
                }
                else
                {
                    ItemViewModel itmv = this.repository.GetItem(new Item { Id = id });
                    itmv.access = 0;
                    if (itmv.Username == this.User.Identity.Name)
                    {
                        itmv.access = 1;
                    }
                    else
                    {
                        this.repository.AddItem_Access_Stat(new Stats_Item { IdItem = itmv.Id, Dob = DateTime.Now });
                    }
                    return this.Ok(itmv);
                }

            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the item: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }
        [Route("image")]
        [HttpGet]
        public IActionResult GetImage(int id)
        {
            try
            {
                    byte[] results = this.repository.GetImage(id);
                    return this.Ok(results);
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the item: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }


        /// <summary>
        /// Remove a specific item
        /// </summary>
        /// <param name="uid">The id of the item that you want to remove</param>
        /// <returns>Task with the result of the action</returns>

        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpDelete]
        public async Task<IActionResult> Remove(int uid)
        {
            try
            {
                if (uid == 0 )
                {
                    return this.BadRequest("Bad data");
                }
                else
                {
                    var lookupItem = new Item { Id = uid };
                    ItemViewModel itemDetect = this.repository.GetItem(lookupItem);
                    Item itemToDelete = this.mapper.Map<Item>(itemDetect);
                    if (itemToDelete == null)
                    {
                        return this.GetAllItems(-1,0);
                    }
                    itemToDelete.Deleted = true;
                    var results = this.repository.DeleteItem(itemToDelete);
                    return this.Ok(this.mapper.Map<IEnumerable<ItemViewModel>>(results));

                }
            }
            catch (Exception ex)
            {
                this.logger.LogError($"Failed to get the item: {ex}");
                return this.BadRequest("Error Occurred");
            }
        }

        /// <summary>
        /// Update the information of an existing item
        /// </summary>
        /// <param name="item">item model</param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        [HttpPut]

        public async Task<IActionResult> UpdateItem(int id, ItemViewModel item)
        {
            if (!this.ModelState.IsValid)
            {
                return this.BadRequest("Bad data");
            }
            
            var results = this.repository.GetItem(new Item { Id = id });
            if (results == null || results.Id == 0)
            {
                return this.BadRequest("User does not exist");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(item.Description))
                {
                    results.Description = item.Description;
                }
                results.Dob = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(item.Title))
                {
                    results.Title = item.Title;
                }
                if(item.IdCategory != 0)
                {
                    results.IdCategory = item.IdCategory;
                }
                if (item.IdLocalization != 0)
                {
                    results.IdLocalization = item.IdLocalization;
                }
                results.Pinned = item.Pinned;
                results.Draft = item.Draft;
                results.ExpirationDate = DateTime.ParseExact(item.expDate, "dd/MM/yyyy", null);
                this.repository.UpdateItem(this.mapper.Map<Item>(results));
                return this.Ok(results);
            }

        }
    }
}