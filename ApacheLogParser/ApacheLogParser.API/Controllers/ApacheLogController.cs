using System;
using System.Web.Http;
using ApacheLogParser.Common.Services.Interfaces;
using ApacheLogParser.DAL.Repositories;

namespace ApacheLogParser.API.Controllers
{
    public class ApacheLogController : ApiController
    {
        private readonly IApacheLogRepository _repository;
        private readonly ILogger _logger;

        public ApacheLogController(IApacheLogRepository repository, ILogger logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public IHttpActionResult GetTopClientRequestsByDateRange(DateTime? start = null, DateTime? end = null, int countOfClients = 10)
        {
            try
            {
                var result = _repository.GetTopClientRequestsByDateRange(start, end, countOfClients);

                return Ok(new { Data = result });
            }
            catch (Exception exception)
            {
                _logger.Error($"Error has occured while getting data from DataBase | Action=GetTopClientRequestsByDateRange | RequestUri={Request.RequestUri}");
                return InternalServerError(exception);
            }
        }

        public IHttpActionResult GetTopRouteRequestsByDateRange(DateTime? start = null, DateTime? end = null, int countOfRoutes = 10)
        {
            try
            {
                var result = _repository.GetTopRouteRequestsByDateRange(start, end, countOfRoutes);

                return Ok(new { Data = result });
            }
            catch (Exception exception)
            {
                _logger.Error($"Error has occured while getting data from DataBase | Action=GetTopRouteRequestsByDateRange | RequestUri={Request.RequestUri}");
                return InternalServerError(exception);
            }
        }

        public IHttpActionResult GetTopRequestsByDateRange(DateTime? start = null, DateTime? end = null, int offset = 10, int limit = 10)
        {
            try
            {
                var result = _repository.GetTopRequestsByDateRange(start, end, offset, limit);

                return Ok(new { Data = result });
            }
            catch (Exception exception)
            {
                _logger.Error($"Error has occured while getting data from DataBase | Action=GetTopRequestsByDateRange | RequestUri={Request.RequestUri}");
                return InternalServerError(exception);
            }
        }
    }
}
