using CsvHelper.Configuration;
using NetConference.Utils.Models;

namespace NetConference.Utils.CsvMappers
{
    public sealed class CoverageBuildingCsvMap : ClassMap<CoverageBuilding>
    {
        public CoverageBuildingCsvMap()
        {
            AutoMap();
            Map(m => m.Gescal17).Name("GESCAL17");
            Map(m => m.WayType).Name("WAYTYPE");
            Map(m => m.WayName).Name("WAYNAME");
            Map(m => m.ProvinceId).Name("PROVINCEID");
            Map(m => m.Province).Name("PROVINCE");
            Map(m => m.Town).Name("TOWN");
            Map(m => m.PostalCode).Name("POSTALCODE");
            Map(m => m.Address).Name("ADDRESS");
            Map(m => m.Num).Name("NUM");
            Map(m => m.Gescal).Ignore();
            Map(m => m.Address2).Ignore();
            Map(m => m.Verticals).Ignore();
            Map(m => m.id).Ignore();
        }
    }
}
