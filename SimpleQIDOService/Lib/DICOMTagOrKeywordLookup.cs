using System;
using System.Collections.Generic;
using Dicom;

namespace SimpleQIDOService.Lib
{
    /// <summary>
    /// This class handles lookup for QIDO style dicomTags which can be keywords or tags.  fo-dicom supports lookups by
    /// tags only so we create a local lookup based on keyword for performance reasons
    /// </summary>
    public class DICOMTagOrKeywordLookup
    {
        private static DICOMTagOrKeywordLookup _instance;

        private readonly Dictionary<string, DicomDictionaryEntry> _keywords = new Dictionary<string, DicomDictionaryEntry>();  

        // TODO: manage singleton lifecycle through DI container and remove this method
        public static DICOMTagOrKeywordLookup Instance()
        {
            if (_instance == null)
            {
                _instance = new DICOMTagOrKeywordLookup();
            }
            return _instance;
        }

        public DICOMTagOrKeywordLookup()
        {
            // create a lookup table from keyword to DicomDictionaryEntry
            var dictionary = DicomDictionary.Default;
            foreach (var entry in dictionary)
            {
                // NOTE: we use the uppercase version of the keyword so the lookup is case insensitive.  Need to check if the standard requires case sensitivity
                _keywords[entry.Keyword.ToUpper()] = entry;
            }
        }

        public DicomDictionaryEntry Lookup(string tagOrKeyword)
        {
            // First lookup by keyword
            DicomDictionaryEntry entry;
            // NOTE: the uppercase version of the keyword is stored as the key so we can lookup case insensitive. 
            if (_keywords.TryGetValue(tagOrKeyword.ToUpper(), out entry))
            {
                return entry;
            }

            if (tagOrKeyword.Length != 8)
            {
                return null;
            }

            // keyword not found, lookup by group/element tag
            var groupString = tagOrKeyword.Substring(0, 4);
            var group = UInt16.Parse(groupString, System.Globalization.NumberStyles.HexNumber);
            var elementString = tagOrKeyword.Substring(4, 4);
            var element = UInt16.Parse(elementString, System.Globalization.NumberStyles.HexNumber);

            var tag = new DicomTag(group, element);
            return DicomDictionary.Default[tag];
        }
    }
}