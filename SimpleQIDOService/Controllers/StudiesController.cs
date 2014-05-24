using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using SimpleQIDOService.Lib;
using SimpleQIDOService.Services.FileImplementation;
using SimpleQIDOService.Services.Interfaces;

namespace SimpleQIDOService.Controllers

{
    [EnableCors(origins: "*", headers: "*", methods: "get,post")]
    public class StudiesController : ApiController
    {
        // TODO: inject via DI container
        private readonly IStudyQuery _studyQuery = new StudyQuery();

        public IEnumerable<Dictionary<string, object>> GetStudies(HttpRequestMessage requestMessage)
        {
            var query = QueryParser.Parse(requestMessage.RequestUri);
            var response = _studyQuery.Execute(query);
            return response;
        }
    }
}
