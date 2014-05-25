using Dicom;

namespace SimpleQIDOService.Lib
{
    public class DefaultResponseTags
    {
        public static DicomTag[] StudyTags =
        {
            new DicomTag(0x0008, 0x0005), // Specific Character Set
            new DicomTag(0x0008, 0x0020), // Study Date
            new DicomTag(0x0008, 0x0030), // Study Time
            new DicomTag(0x0008, 0x0050), // Accession Number
            //new DicomTag(0x0008, 0x0056), // Instance Availability
            //new DicomTag(0x0008, 0x0061), // Modalities In Study
            new DicomTag(0x0008, 0x0090), // Referring Physicians Name
            new DicomTag(0x0008, 0x0201), // Time Zone Offset from UTC
            new DicomTag(0x0008, 0x1030), // Study Description (NOTE: This is not listed in Table 6.7.1-2 in the DICOM standard which is probably incorrect)
            //new DicomTag(0x0008, 0x1190), // Retrieve URL (WADO-RS only)
            new DicomTag(0x0010, 0x0010), // Patient's Name
            new DicomTag(0x0010, 0x0020), // Patient ID
            new DicomTag(0x0010, 0x0030), // Patient's Birth Date
            new DicomTag(0x0010, 0x0040), // Patient's Sex
            new DicomTag(0x0020, 0x000D), // Study Uid
            new DicomTag(0x0020, 0x0010), // Study Id
            //new DicomTag(0x0020, 0x1206), // Number of Study Related Series
            //new DicomTag(0x0020, 0x1208), // Number of Study Related Instances
        };

        public static DicomTag[] AllStudyTags =
        {

        };

        public static DicomTag[] SeriesTags =
        {
            new DicomTag(0x0008, 0x0005), // Specific Character Set
            new DicomTag(0x0008, 0x0060), // Modality
            new DicomTag(0x0008, 0x0201), // Time Zone Offset from UTC
            new DicomTag(0x0008, 0x103E), // Series Description
            //new DicomTag(0x0008, 0x1190), // Retrieve URL
            new DicomTag(0x0020, 0x000E), // Series Uid
            new DicomTag(0x0020, 0x0011), // Series Number
            //new DicomTag(0x0020, 0x1209), // Number of Series Related Instances
            new DicomTag(0x0040, 0x0244), // Performed Procedure Step Start Date
            new DicomTag(0x0040, 0x0245), // Performed Procedure Step Start Time
            new DicomTag(0x0040, 0x0275), // Request Attribute Sequence

        };

        public static DicomTag[] AllSeriesTags =
        {

        };

        public static DicomTag[] InstaceTags =
        {
            new DicomTag(0x0008, 0x0005), // Specific Character Set
            new DicomTag(0x0008, 0x0016), // SOP Class UID
            new DicomTag(0x0008, 0x0018), // SOP Instance UID
            //new DicomTag(0x0008, 0x0056), // Instance Availability
            new DicomTag(0x0008, 0x0201), // Time Zone Offset from UTC
            //new DicomTag(0x0008, 0x1190), // Retrieve URL
            new DicomTag(0x0020, 0x0013), // Instance Number
            new DicomTag(0x0028, 0x0010), // Rows
            new DicomTag(0x0028, 0x0011), // Columns
            new DicomTag(0x0028, 0x0100), // Bits Allocated
            new DicomTag(0x0028, 0x0008), // Number of Frames
        };

        public static DicomTag[] AllInstanceTags =
        {

        };


    }
}