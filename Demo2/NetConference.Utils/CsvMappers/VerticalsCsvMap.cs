using CsvHelper.Configuration;
using NetConference.Utils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetConference.Utils.CsvMappers
{
    public sealed class VerticalsCsvMap : ClassMap<VerticalsCsv>
    {
        public VerticalsCsvMap()
        {
            AutoMap();
            Map(m => m.Gescal37).Name("GESCAL");
            Map(m => m.Technology).Name("TECHNOLOGY");
            Map(m => m.Subtechnology).Name("SUBTYPE_TECHNOLOGY");
            Map(m => m.Modality).Name("MODALITY");
            Map(m => m.TerritoryOwner).Name("TERRITORY_OWNER");
            Map(m => m.SpeedProfile).Name("SPEED_PROFILE");
            Map(m => m.SpeedEstimated).Name("SPEED_ESTIMATED");
            Map(m => m.Source).Name("ORIGIN");
            Map(m => m.AddressId).Name("ADDRESS_ID");
            Map(m => m.Priority).Name("PRIORITY");
            Map(m => m.Typology).Name("TYPOLOGY");
            Map(m => m.CentralCode).Name("CENTRAL_CODE");
            Map(m => m.NodeCode).Name("NODE_CODE");
            Map(m => m.Gescal).Ignore();
        }
    }
}
