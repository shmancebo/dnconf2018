using System.Collections.Generic;

namespace NetConference.Utils.Models
{
    public class VerticalsCsv
    {
        public string Gescal37 { get; set; }

        public string Gescal
        {
            get
            {
                return Gescal37.Substring(0, 17);
            }
        }

        public string Technology { get; set; }

        public string Subtechnology { get; set; }

        public string Modality { get; set; }

        public string TerritoryOwner { get; set; }

        public string SpeedProfile { get; set; }

        public string SpeedEstimated { get; set; }

        public string Source { get; set; }

        public string AddressId { get; set; }

        public string Priority { get; set; }

        public string Typology { get; set; }

        public string CentralCode { get; set; }

        public string NodeCode { get; set; }
    }

    public class Verticals
    {
        public string Gescal37 { get; set; }

        public string Bisduplicate { get; set; }

        public string BisduplicateId { get; set; }

        public string Block { get; set; }

        public string BlockId { get; set; }

        public string Door { get; set; }

        public string DoorId { get; set; }

        public string Letter { get; set; }

        public string Stair { get; set; }

        public string StairId { get; set; }

        public string Floor { get; set; }

        public string FloorId { get; set; }

        public string Hand1 { get; set; }

        public string Hand2 { get; set; }

        public List<VerticalsTechnology> Technologies { get; set; }
    }

    public class VerticalsTechnology
    {
        public string Technology { get; set; }

        public string Subtechnology { get; set; }

        public string Modality { get; set; }

        public string TerritoryOwner { get; set; }

        public string SpeedProfile { get; set; }

        public string SpeedEstimated { get; set; }

        public string Source { get; set; }

        public string AddressId { get; set; }

        public string Priority { get; set; }

        public string Typology { get; set; }

        public string CentralCode { get; set; }

        public string NodeCode { get; set; }
    }

    public class RespuestaVertical
    {
        public RespuestaVertical()
        {
            gescal37 = string.Empty;
        }

        public string gescal37 { get; set; }
        public string addressNumber { get; set; }
        public string bis { get; set; }
        public string bisId { get; set; }
        public string bloque { get; set; }
        public string bloqueId { get; set; }
        public string portalPuerta { get; set; }
        public string portalPuertaId { get; set; }
        public string letraFinca { get; set; }
        public string planta { get; set; }
        public string plantaId { get; set; }
        public string escalera { get; set; }
        public string escaleraId { get; set; }
        public string mano1 { get; set; }
        public string mano2 { get; set; }
        public string Mano
        {
            get
            {
                var texto1 = string.IsNullOrWhiteSpace(mano1) ? string.Empty : mano1.Trim();
                var texto2 = string.IsNullOrWhiteSpace(mano2) ? string.Empty : mano2.Trim();
                return $"{texto1} {texto2}".Trim();
            }
        }
        public string tecnologia { get; set; }
        public string addressÏd { get; set; }
    }
}
