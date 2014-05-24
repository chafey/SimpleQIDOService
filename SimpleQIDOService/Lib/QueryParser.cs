using System;
using System.Web;
using SimpleQIDOService.Models;
using Dicom;

namespace SimpleQIDOService.Lib
{
    /// <summary>
    /// This class handles parsing the query parameters for a QIDO request
    /// </summary>
    public class QueryParser
    {
        public static Query Parse(Uri requestUri)
        {
            var queryParams = HttpUtility.ParseQueryString(requestUri.Query);

            var query = new Query();

            foreach (var queryParam in queryParams.AllKeys)
            {
                var value = queryParams[queryParam];
                var queryParamLower = queryParam.ToLower();

                if (queryParamLower == "includefield")
                {
                    var values = value.Split(',');
                    foreach (var splitValue in values)
                    {
                        var dicomTag = GetTag(splitValue);
                        if (dicomTag == null)
                        {
                            query.Errors.Add(String.Format("include field specified unknown DICOM Keyword or Tag '{0}', skipping", queryParam));
                            continue;

                        }
                        query.IncludeFields.Add(dicomTag);
                    }
                }
                else if (queryParamLower == "fuzzymatching")
                {
                    query.FuzzyMatching = value.ToLower() == "false";
                }
                else if (queryParamLower == "limit")
                {
                    query.Limit = int.Parse(value);
                }
                else if (queryParamLower == "offset")
                {
                    query.Offset = int.Parse(value);
                }
                else
                {
                    // must be an attribute, 
                    var dicomTag = GetTag(queryParam);
                    if (dicomTag == null)
                    {
                        query.Errors.Add(String.Format("unknown DICOM Keyword or Tag '{0}' specified, skipping", queryParam));
                        continue;
                    }
                    var queryAttribute = new QueryAttribute();
                    queryAttribute.RawKey = queryParam;
                    queryAttribute.RawValue = value;
                    queryAttribute.Tag = dicomTag;
                    query.QueryAttributes.Add(dicomTag, queryAttribute);
                }
            }
            return query;
        }

        private static DicomTag GetTag(string dicomTagOrKeyword)
        {
            var entry = DICOMTagOrKeywordLookup.Instance().Lookup(dicomTagOrKeyword);
            if (entry == null)
            {
                return null;
            }
            return entry.Tag;
        }
    }
}