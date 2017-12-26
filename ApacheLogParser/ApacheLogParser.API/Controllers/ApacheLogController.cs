using System;
using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Results;
using ApacheLogParser.DAL.DataBaseModels.ApacheLogModels;
using ApacheLogParser.DAL.Repositories;

namespace ApacheLogParser.API.Controllers
{
    public class ApacheLogController : ApiController
    {
        private readonly IApacheLogRepository _repository;

        public ApacheLogController(IApacheLogRepository repository)
        {
            _repository = repository;
        }

        public JsonResult<List<ClientRequestModel>> GetTopClientRequestsByDateRange(DateTime? start = null, DateTime? end = null, int countOfClients = 10)
        {
            var result = _repository.GetTopClientRequestsByDateRange(start, end, countOfClients);

            return Json(result);
        }

        public JsonResult<List<RouteRequestModel>> GetTopRouteRequestsByDateRange(DateTime? start = null, DateTime? end = null, int countOfRoutes = 10)
        {
            var result = _repository.GetTopRouteRequestsByDateRange(start, end, countOfRoutes);

            return Json(result);
        }
    }
}
