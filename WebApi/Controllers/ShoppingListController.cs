using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace WebApi.Controllers
{
    public class ShoppingListController : ApiController
    {
        public static List<Item> items = new List<Item>();

        public List<Item> Get()
        {
            return items;
        }

        public HttpResponseMessage Get(HttpRequestMessage request, int id)
        {
            Item item = items.FirstOrDefault(i => i.Id == id);
            if (item == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return request.CreateResponse<Item>(HttpStatusCode.OK, item);
        }

        public HttpResponseMessage Post(HttpRequestMessage request, [FromBody]Item item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (items.Any(i => i.Id == item.Id))
            {
                return request.CreateResponse(HttpStatusCode.Conflict);
            }

            items.Add(item);
            return request.CreateResponse(HttpStatusCode.Created);
        }

        public HttpResponseMessage Put(HttpRequestMessage request, [FromBody]Item item)
        {
            if (string.IsNullOrEmpty(item.Name))
            {
                return request.CreateResponse<ItemResult>(HttpStatusCode.BadRequest, new ItemResult { Message = "Name cannot be null." });
            }

            if (!items.Any(i => i.Id == item.Id))
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            var oldItem = items.FirstOrDefault(i => i.Id == item.Id);
            oldItem.Name = item.Name;

            return request.CreateResponse(HttpStatusCode.OK);
        }

        public HttpResponseMessage Delete(HttpRequestMessage request, int id)
        {
            if (!items.Any(i => i.Id == id))
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            items.RemoveAll(i => i.Id == id);

            return request.CreateResponse(HttpStatusCode.OK);
        }
    }

    public class Item
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class ItemResult
    {
        public string Message { get; set; }
    }
}
