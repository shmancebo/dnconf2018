using Newtonsoft.Json;
using System.ComponentModel;

namespace NetConference.Utils.Models
{
    public class CoverageBuilding
    {
        public string id
        {
            get { return Gescal; }
            set { Gescal = value; }
        }

        [Description("GESCAL17")]
        public string Gescal17
        {
            get { return Gescal; }
            set { Gescal = value; }
        }

         [Description("NUM")]
        public string Num { get; set; }

        [JsonIgnore]
        public string Address2
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(WayType)
                    || !string.IsNullOrWhiteSpace(WayName)
                    || !string.IsNullOrWhiteSpace(Num)
                    || !string.IsNullOrWhiteSpace(Town))
                {
                    return $"{WayType} {WayName}, {Num}, {Town}";
                }
                return null;
            }
        }

        public string Verticals { get; set; }

        [Description("WAYTYPE")]
        public string WayType { get; set; }

        [Description("WAYNAME")]
        public string WayName { get; set; }

        [Description("PROVINCEID")]
        public string ProvinceId { get; set; }

        [Description("PROVINCE")]
        public string Province { get; set; }

        [Description("TOWN")]
        public string Town { get; set; }

        [Description("POSTALCODE")]
        public string PostalCode { get; set; }

        public string Gescal { get; set; }
        
        [Description("ADDRESS")]
        public string Address { get; set; }
    }
}
