namespace Main.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Main.Models;
    using Main.ViewModels;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Interface with all the method needed
    /// </summary>
    public interface IItemRepository
    {
        void AddItem(Item nItem);
        void AddItem_Access_Stat(Stats_Item nItem);

        List<Item> DeleteItem(Item nItem);

        object GetAllItems();
        object GetAllItemsAnonymous();
        object GetAllItemsAnonymousWall();
        object GetAllItemsByUser(string username);
        ItemViewModel GetItem(Item nItem);

        Task<bool> SaveChangesAsync();

        void UpdateItem(Item nItem);

        UserManage GetUserManageWithRefreshToken(string token);
        void UpdateRefreshToken(UserManage user);
        bool AddFile(IFormFile file, int id);
        byte[] GetImage(int id);

        void AddCategory(Category nCategory);
        Category GetCategory(Category nCategory);
        void DeleteCategory(Category nCategory);
        void UpdateCategory(Category nCategory);
        List<Category> GetAllCategories();
    }
}