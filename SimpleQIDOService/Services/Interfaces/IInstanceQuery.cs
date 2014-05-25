using System.Collections.Generic;
using SimpleQIDOService.Models;

namespace SimpleQIDOService.Services.Interfaces
{
    interface IInstanceQuery
    {
        List<Dictionary<string, object>> Execute(string studyUid, Query query);
        List<Dictionary<string, object>> Execute(string studyUid, string seriesUid, Query query);

    }
}
