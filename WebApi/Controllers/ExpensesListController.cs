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
    public class CostsListController : ApiController
    {
        public static List<Cost> costs = new List<Cost>();

        public List<Cost> Get()
        {
            return costs;
        }

        public HttpResponseMessage Get(HttpRequestMessage request, string name)
        {
            Cost cost = costs.FirstOrDefault(i => i.name == name);
            if (cost == null)
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }

            return request.CreateResponse<Cost>(HttpStatusCode.OK, cost);
        }

        public HttpResponseMessage Post(HttpRequestMessage request, [FromBody]Cost cost)
        {
            if (string.IsNullOrEmpty(cost.name))
            {
                return request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (costs.Any(i => i.name == cost.name))
            {
                return request.CreateResponse(HttpStatusCode.Conflict);
            }

            costs.Add(cost);
            return request.CreateResponse(HttpStatusCode.Created);
        }

        public HttpResponseMessage Put(HttpRequestMessage request, [FromBody]Cost cost)
        {
            if (string.IsNullOrEmpty(cost.name))
            {
                return request.CreateResponse<CostResult>(HttpStatusCode.BadRequest, new CostResult { Message = "Name cannot be null." });
            }

            if (!costs.Any(i => i.name == cost.name))
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            var oldCost = costs.FirstOrDefault(i => i.name == cost.name);
            oldCost.name = cost.name;

            return request.CreateResponse(HttpStatusCode.OK);
        }
        public HttpResponseMessage Delete(HttpRequestMessage request, string name)
        {
            if (!costs.Any(i => i.name == name))
            {
                return request.CreateResponse(HttpStatusCode.NotFound);
            }

            costs.RemoveAll(i => i.name == name);

            return request.CreateResponse(HttpStatusCode.OK);
        }
    }
}
