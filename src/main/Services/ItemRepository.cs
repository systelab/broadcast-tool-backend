namespace Main.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using AutoMapper;
    using Main.Models;
    using Main.ViewModels;
    using Microsoft.AspNetCore.Http;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Repository with all the queries to the database using the entity framework
    /// </summary>
    public class ItemRepository : IItemRepository
    {
        private readonly ItemsContext context;
        /// <summary>
        /// Set the context of the app
        /// </summary>
        /// <param name="_context"></param>
        public ItemRepository(ItemsContext _context)
        {
            this.context = _context;
        }

        public List<Item> Items { get; private set; }

        /// <summary>
        /// Insert the item into the database
        /// </summary>
        /// <param name="newItem">Object with the information of the item that you want to insert</param>
        public void AddItem(Item newItem)
        {
            this.context.Add(newItem);
            this.context.SaveChanges();
        }
        /// <summary>
        /// Insert stat about the user viewed an item
        /// </summary>
        /// <param name="newItem">Object with the information of the item viewed</param>
        public void AddItem_Access_Stat(Stats_Item newItem)
        {
            this.context.Add(newItem);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove the item from the database
        /// </summary>
        /// <param name="nItem">Object with the information of the item that you want to remove</param>
        public List<Item> DeleteItem(Item nItem)
        {
            this.context.Entry(nItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
            return this.context.Items.ToList();
        }

        /// <summary>
        /// List all the items saved in the database
        /// </summary>
        /// <returns>List of items object</returns>
        public object GetAllItems()
        {
            var itemslist = (from t1 in this.context.Items

                            join t2 in this.context.Users on t1.Username equals t2.UserName
                             join t3 in this.context.Categories on t1.IdCategory equals t3.Id
                             orderby t1.Dob descending
                            select new { t1.Username, t1.Id, t1.Path, t1.Pinned, t1.Title, t1.Dob, t1.Description, t2.Name, t2.LastName,t1.Deleted, t1.IdCategory, NameCategory = t3.Name, t1.ExpirationDate, t1.Draft }).Where(x => x.Deleted == false).Where(x => x.Draft == false);
            return itemslist;
        }
        /// <summary>
        /// List all the items saved in the database for anonymlous view in the viewer
        /// </summary>
        /// <returns>List of items object</returns>
        public object GetAllItemsAnonymous()
        {
            var itemslist = (from t1 in this.context.Items

                             join t2 in this.context.Users on t1.Username equals t2.UserName
                             join t3 in this.context.Categories on t1.IdCategory equals t3.Id
                             orderby t1.Dob descending
                             select new { t1.Username, t1.Id, t1.Path, t1.Pinned, t1.Title, t1.Dob, t1.Description, t2.Name, t2.LastName, t1.Deleted, t1.IdCategory, NameCategory = t3.Name, t1.ExpirationDate, t1.Draft }).Where(x => x.Deleted == false).Where(x => x.Pinned == true).Where(x => x.Draft == false).Where(x => x.ExpirationDate > DateTime.Now);
            return itemslist;
        }
        /// <summary>
        /// List all the items saved in the database for anonymlous view
        /// </summary>
        /// <returns>List of items object</returns>
        public object GetAllItemsAnonymousWall()
        {
            var itemslist = (from t1 in this.context.Items

                             join t2 in this.context.Users on t1.Username equals t2.UserName
                             join t3 in this.context.Categories on t1.IdCategory equals t3.Id
                             orderby t1.Dob descending
                             select new { t1.Username, t1.Id, t1.Path, t1.Pinned, t1.Title, t1.Dob, t1.Description, t2.Name, t2.LastName, t1.Deleted, t1.IdCategory, NameCategory = t3.Name,t1.ExpirationDate,t1.Draft }).Where(x => x.Deleted == false).Where(x => x.Draft == false);
            return itemslist;
        }

        /// <summary>
        /// List all the items saved in the database for anonymlous view
        /// </summary>
        /// <returns>List of items object</returns>
        public async Task<ICollection<ItemViewModel>> GetMostRecentItemsAnonymousWall()
        {
            var itemslist = await (from t1 in this.context.Items

                             join t2 in this.context.Users on t1.Username equals t2.UserName
                             join t3 in this.context.Categories on t1.IdCategory equals t3.Id
                             orderby t1.Dob descending
                             select new ItemViewModel { Username = t1.Username, Id = t1.Id, Path = t1.Path, Pinned = t1.Pinned, Title = t1.Title, Dob = t1.Dob, Description = t1.Description, Name = t2.Name, LastName = t2.LastName, Deleted = t1.Deleted, IdCategory = t1.IdCategory, CategoryName = t3.Name, ExpirationDate = t1.ExpirationDate, Draft = t1.Draft }).Where(x => x.Deleted == false && x.Draft == false && DateTime.Now.Subtract(x.Dob).TotalDays < 5).ToListAsync();
            return itemslist;
        }
        /// <summary>
        /// Get a specific item
        /// </summary>
        /// <param name="nItem">Object of the item that you want to retrieve, in this case the id of the item must be filled</param>
        /// <returns></returns>
        public ItemViewModel GetItem(Item nItem)
        {
            var itemslist = (from t1 in this.context.Items
                            join t2 in this.context.Users on t1.Username equals t2.UserName
                            join t3 in this.context.Categories on t1.IdCategory equals t3.Id
                            select new { t1.Username, t1.Id, t1.Path, t1.Pinned, t1.Title, t1.Dob, t1.Description, t2.Name, t2.LastName, t1.Deleted,t1.IdCategory,NameCategory = t3.Name, t1.ExpirationDate, t1.Draft }).Where(o => o.Id == nItem.Id).Where(o => o.Deleted == false);
           
            ItemViewModel it1 = new ItemViewModel
            {
                Username = itemslist.First().Username,
                Name = itemslist.First().Name,
                Title = itemslist.First().Title,
                Pinned = itemslist.First().Pinned,
                Path = itemslist.First().Path,
                Id = itemslist.First().Id,
                Description = itemslist.First().Description,
                Dob = itemslist.First().Dob,
                LastName = itemslist.First().LastName,
                IdCategory = itemslist.First().IdCategory,
                CategoryName = itemslist.First().NameCategory,
                ExpirationDate = itemslist.First().ExpirationDate,
                Draft = itemslist.First().Draft
            };
            return it1;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await this.context.SaveChangesAsync()) > 0;
        }

        /// <summary>
        /// Update information of the item
        /// </summary>
        /// <param name="nItem">Object of the item that you want to update, they ID must be filled and the information that you want to change</param>
        public void UpdateItem(Item nItem)
        {
            this.context.Entry(nItem).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
        }
        /// <summary>
        /// Get the user information providing a refresh token, in this case we are using a database but you can use other system.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserManage GetUserManageWithRefreshToken(string token)
        {
            return this.context.Users.Where(t => t.RefreshToken == token).FirstOrDefault();
        }
        /// <summary>
        /// Update the refresh token of the user session
        /// </summary>
        /// <param name="user"></param>
        public void UpdateRefreshToken(UserManage user)
        {
            this.context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
        }

        public bool AddFile(IFormFile file, int id)
        {
            string curDir = @"c:\Data_Communication_Tool";
            if (!System.IO.Directory.Exists(curDir))
            {
                System.IO.Directory.CreateDirectory(curDir);
            }

            string fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.Trim('"');
            string fullPath = Path.Combine(curDir, fileName);
            if (!System.IO.File.Exists(fullPath))
            {
                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }

            }

            ItemViewModel it = GetItem(new Item { Id = id });
            it.Path = fullPath;
            Item it1 = new Item {
                Username = it.Username,
                Type = it.Type,
                Title = it.Title,
                Pinned=it.Pinned,
                Path = it.Path,
                Id = it.Id,
                Description = it.Description,
                Dob = it.Dob,
                Email = it.Email,
                IdCategory = it.IdCategory,
                ExpirationDate = it.ExpirationDate,
                Draft = it.Draft
            };
            UpdateItem(it1);

            return true;
        }
        public static void CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[8 * 1024];
            int len;
            while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, len);
            }
        }

        public object GetAllItemsByUser(string username)
        {

            var itemslist = (from t1 in this.context.Items
                             join t2 in this.context.Users on t1.Username equals t2.UserName
                             join t3 in this.context.Categories on t1.IdCategory equals t3.Id
                             orderby t1.Dob descending
                             select new { t1.Username, t1.Id, t1.Path, t1.Pinned, t1.Title, t1.Dob, t1.Description, t2.Name, t2.LastName, t1.Deleted, t1.IdCategory, NameCategory = t3.Name,t1.ExpirationDate,t1.Draft }).Where(t => t.Username == username).Where(x => x.Deleted == false);
            return itemslist;
        }

        public byte[] GetImage(int id)
        {
            byte[] b = null;
            ItemViewModel it = GetItem(new Item { Id = id });
            b = File.ReadAllBytes(it.Path);
            if (b != null)
            {
                byte[] data = b.ToArray();
                return data;
            }
            else
            {
                return null;
            }

}
        /// <summary>
        /// Add a Category
        /// </summary>
        /// <param name="nCategory"></param>
        public void AddCategory(Category nCategory)
        {
            this.context.Add(nCategory);
            this.context.SaveChanges();
        }
        /// <summary>
        /// Get a specific category
        /// </summary>
        /// <param name="nCategory"></param>
        /// <returns></returns>
        public Category GetCategory(Category nCategory)
        {
            return this.context.Categories.Where(x => x.Id == nCategory.Id).FirstOrDefault();
        }
        /// <summary>
        /// Delete a specific category
        /// </summary>
        /// <param name="nCategory"></param>
        public void DeleteCategory(Category nCategory)
        {
            this.context.Entry(nCategory).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
        }
        /// <summary>
        /// Update the name of a specific category
        /// </summary>
        /// <param name="nCategory"></param>
        public void UpdateCategory(Category nCategory)
        {
            this.context.Entry(nCategory).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
        }
        /// <summary>
        /// Get a list of categories
        /// </summary>
        /// <returns></returns>
        public List<Category> GetAllCategories()
        {
            return this.context.Categories.Where(x => x.Deleted == false).OrderBy(x => x.Name).ToList();
        }
    }

}