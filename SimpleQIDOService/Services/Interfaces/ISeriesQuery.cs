using System.Collections.Generic;
using SimpleQIDOService.Models;

namespace SimpleQIDOService.Services.Interfaces
{
    interface ISeriesQuery
    {
        List<Dictionary<string, object>> Execute(string studyUid, Query query);
    }
}
