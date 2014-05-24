using System.Collections.Generic;
using SimpleQIDOService.Models;

namespace SimpleQIDOService.Services.Interfaces
{
    interface IStudyQuery
    {
        List<Dictionary<string, object>> Execute(Query query);
    }
}
