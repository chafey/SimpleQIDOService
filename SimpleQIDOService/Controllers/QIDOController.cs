using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Results;
using SimpleQIDOService.Lib;
using SimpleQIDOService.Services.FileImplementation;
using SimpleQIDOService.Services.Interfaces;

namespace SimpleQIDOService.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "GET")]
    public class QIDOController : ApiController
    {
        // TODO: inject via DI container
        private readonly IStudyQuery _studyQuery = new StudyQuery();
        private readonly ISeriesQuery _seriesQuery = new SeriesQuery();
        private readonly IInstanceQuery _instanceQuery = new InstanceQuery();

        [Route("api/studies")]
        public IEnumerable<Dictionary<string, object>> GetStudies(HttpRequestMessage requestMessage)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            var response = _studyQuery.Execute(query);
            return response;
        }

        [Route("api/studies/{studyUid}/series")]
        public IEnumerable<Dictionary<string, object>> GetStudySeries(HttpRequestMessage requestMessage, string studyUid)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            var response = _seriesQuery.Execute(studyUid, query);
            return response;
        }

        /*[Route("api/series")]
        public IHttpActionResult SearchForSeries(HttpRequestMessage requestMessage)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            return Ok();
            //var response = _studyQuery.Execute(query);
            //return response;
        }
         * */

        [Route("api/studies/{studyUid}/series/{seriesUid}/instances")]
        public IEnumerable<Dictionary<string, object>> GetStudySeriesInstances(HttpRequestMessage requestMessage, string studyUid, string seriesUid)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            var response = _instanceQuery.Execute(studyUid, seriesUid, query);
            return response;
        }

        [Route("api/studies/{studyUid}/instances")]
        public IEnumerable<Dictionary<string, object>> GetStudyInstances(HttpRequestMessage requestMessage, string studyUid)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            var response = _instanceQuery.Execute(studyUid, query);
            return response;
        }
        
        /*
        [Route("api/instances")]
        public IHttpActionResult SearchForInstances(HttpRequestMessage requestMessage)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            return Ok();
            //var response = _studyQuery.Execute(query);
            //return response;
        }
        */

    }
}
