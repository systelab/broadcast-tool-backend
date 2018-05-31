namespace Main
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using cloudscribe.Syndication.Models.Rss;
    using Main.Services;
    using Microsoft.AspNetCore.Http;

    internal class BroadcastRssChannelProvider : IChannelProvider
    {
        private readonly IHttpContextAccessor contextAccessor;

        public string Name => "main.BroadcastRssChannelProvider";

        private IItemRepository itemRepository { get; }

        public BroadcastRssChannelProvider(IItemRepository itemRepository, IHttpContextAccessor contextAccessor)
        {
            this.itemRepository = itemRepository;
            this.contextAccessor = contextAccessor;
        }
        public async Task<RssChannel> GetChannel(CancellationToken cancellationToken = default(CancellationToken))
        {
            RssChannel channel = new RssChannel();
            channel.Title = "Broadcast tool";
            channel.Description = "This is an RSS feed for the Broadcasting tool. Do not miss any of our post!";
            channel.Generator = this.Name;

            var items = await this.itemRepository.GetMostRecentItemsAnonymousWall();

            foreach(var category in this.itemRepository.GetAllCategories().Where(p => !p.Deleted))
            {
                channel.Categories.Add(new RssCategory(category.Name));
            }

            var rssItems = new List<RssItem>();
            var request = contextAccessor.HttpContext.Request;

            var absoluteUri = string.Concat(
                        request.Scheme,
                        "://",
                        request.Host.ToUriComponent(),
                        request.PathBase.ToUriComponent(),
                        request.Path.ToUriComponent(),
                        request.QueryString.ToUriComponent());
            Uri uri = new Uri(absoluteUri);

            foreach (var item in items)
            {
                RssItem rssItem = new RssItem
                {
                    Author = item.Username,
                    Title = item.Title,
                    Description = item.Description,
                    PublicationDate = item.Dob,
                    Link = new Uri($"{uri.Scheme}://{uri.Host}/#/item/{item.Id}"),
                    Guid = new RssGuid(item.Id.ToString())
                };

                rssItems.Add(rssItem);
            }

            channel.Items = rssItems;
            channel.Link = uri;

            return channel;
        }
    }
}