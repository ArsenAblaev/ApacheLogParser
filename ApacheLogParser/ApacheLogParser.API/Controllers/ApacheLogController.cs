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

        /// <summary>
        /// Get top N-clients ordered by number of requests descending by data range.
        /// If start = null : start = MIN date
        /// If end = null: end = MAX date 
        /// countOfClients = 10 by default
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="countOfClients"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Get top N-routes ordered by number of requests descending by date range.
        /// If start = null: start = MIN date
        /// IF end = null: end = MAX date
        /// countOfRoutes = 10 by default
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="countOfRoutes"></param>
        /// <returns></returns>
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


        /// <summary>
        /// Get all ApacheLog fields ordered by RequestDate with offset and limit
        /// If start = null: start = MIN date
        /// If end = null: end = MAX date
        /// offset = 10 by default
        /// limit = 10 by default
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="offset"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
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
