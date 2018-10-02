using NetConference.Utils.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NetConference.Utils.Helpers
{
    public static class CastHelper
    {
        public static List<T1> Mapper<T, T1>(this List<T> entities)
        {
            var result = Activator.CreateInstance<List<T1>>();

            foreach (var entity in entities)
            {
                var resultObject = Activator.CreateInstance<T1>();
                MapObjects(entity, resultObject);
                result.Add(resultObject);
            }

            return result;
        }

        public static RespuestaVertical GetRespuestaVertical(string gescal)
        {
            return GetRespuesta(gescal);
        }

        static RespuestaVertical GetRespuesta(string gescal)
        {
            return MappingRespuestaVertical(gescal);
        }

        static RespuestaVertical MappingRespuestaVertical(string gescal)
        {
            if (gescal.Length < 37)
            {
                gescal = gescal.PadRight(37);
            }

            var respuesta = new RespuestaVertical();
            var infoAdicional = gescal.Substring(17, gescal.Length - 17);

            respuesta.gescal37 = gescal;
            respuesta.addressNumber = GetStandardValue(gescal.Substring(12, 5));
            respuesta.letraFinca = GetStandardValue(infoAdicional.Substring(6, 1));
            respuesta.mano1 = GetStandardValue(infoAdicional.Substring(12, 4));
            respuesta.mano2 = GetStandardValue(infoAdicional.Substring(16, 4));


            SetDomicilio(respuesta, infoAdicional);
            SetBisDuplicado(respuesta, infoAdicional);
            SetBloque(respuesta, infoAdicional);
            SetPortalPuerta(respuesta, infoAdicional);
            SetEscalera(respuesta, infoAdicional);

            return respuesta;
        }

        static void SetDomicilio(RespuestaVertical respuesta, string infoAdicional)
        {
            var domicilio = infoAdicional.Substring(9, 3);
            var listaTrad = GetPlanta();

            if (!string.IsNullOrWhiteSpace(domicilio.Trim()))
            {
                respuesta.plantaId = domicilio;
                var domSinEspacios = domicilio.Trim();
                if (domSinEspacios.Length == 2)
                {
                    if (listaTrad.Any(b => b.Code == domSinEspacios))
                    {
                        respuesta.planta = listaTrad.First(b => b.Code == domSinEspacios).Name;
                    }
                }
                else
                {
                    respuesta.planta = domicilio;
                }
            }
        }

        static void SetBisDuplicado(RespuestaVertical respuesta, string infoAdicional)
        {
            var bis = infoAdicional.Substring(0, 1);
            var listaTrad = GetBisDuplicate();

            if (!string.IsNullOrWhiteSpace(bis.Trim()))
            {
                if (listaTrad.Any(b => b.Code == bis))
                {
                    respuesta.bis = listaTrad.First(b => b.Code == bis).Name;
                }
                else
                {
                    respuesta.bis = bis;
                }
                respuesta.bisId = bis;
            }
        }

        static void SetBloque(RespuestaVertical respuesta, string infoAdicional)
        {
            var bloque = infoAdicional.Substring(1, 3);
            if (!string.IsNullOrWhiteSpace(bloque.Trim()))
            {
                var tipo = bloque.Substring(0, 1);
                var listaTrad = GetBloque();

                if (listaTrad.Any(b => b.Code == tipo))
                {
                    respuesta.bloque = $"{listaTrad.First(b => b.Code == tipo).Name} {bloque.Substring(1, 2)}";
                    respuesta.bloqueId = bloque;
                }
            }
        }

        static void SetPortalPuerta(RespuestaVertical respuesta, string infoAdicional)
        {
            var portalPuerta = infoAdicional.Substring(4, 2);

            if (!string.IsNullOrWhiteSpace(portalPuerta.Trim()))
            {
                if (portalPuerta.Contains("O") || portalPuerta.Contains("U"))
                {
                    var tipo = portalPuerta.Substring(0, 1);
                    var valor = portalPuerta.Substring(1, 1);

                    switch (tipo)
                    {
                        case "O":
                            respuesta.portalPuerta = $"Portal {valor}";
                            break;
                        case "U":
                            respuesta.portalPuerta = $"Puerta {valor}";
                            break;
                    }
                }
                else
                {
                    respuesta.portalPuerta = portalPuerta;
                }
                respuesta.portalPuertaId = portalPuerta;
            }
        }

        static void SetEscalera(RespuestaVertical respuesta, string infoAdicional)
        {
            var escalera = infoAdicional.Substring(7, 2);

            if (!string.IsNullOrWhiteSpace(escalera.Trim()))
            {
                var escaleras = GetEscalera();

                var valor1 = EvaluateEscalera(escaleras, escalera.Substring(0, 1));
                var valor2 = EvaluateEscalera(escaleras, escalera.Substring(1, 1));

                respuesta.escalera = $"{valor1} {valor2}";
                respuesta.escaleraId = escalera;
            }
        }

        static string GetStandardValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value?.Trim()))
            {
                return null;
            }
            return value;
        }

        static void MapObjects<T, T1>(T source, T1 target)
        {
            Type sourceType = source.GetType();
            Type targetType = target.GetType();

            var sourceProperties = sourceType.GetProperties();
            var destionationProperties = targetType.GetProperties();

            foreach (var prop in destionationProperties)
            {
                PropertyInfo sourceProp = null;
                if (Attribute.GetCustomAttribute(prop, typeof(DescriptionAttribute)) is DescriptionAttribute attribute)
                {
                    sourceProp = sourceProperties.FirstOrDefault(p => p.Name == attribute.Description);
                }

                if (sourceProp == null)
                {
                    sourceProp = sourceProperties.FirstOrDefault(p => p.Name == prop.Name);
                }

                if (sourceProp != null)
                {
                    prop.SetValue(target, sourceProp.GetValue(source, null), null);
                }
            }
        }

        static List<Planta> GetPlanta()
        {
            return new List<Planta>
            {
                new Planta { Code = " ", Name = "-- PLANTA --" },
                new Planta { Code = "AL", Name = "Altillo" },
                new Planta { Code = "AM", Name = "Almacén" },
                new Planta { Code = "AS", Name = "Ascensor" },
                new Planta { Code = "AT", Name = "Atico" },
                new Planta { Code = "BA", Name = "Bajo" },
                new Planta { Code = "BU", Name = "Bungalow" },
                new Planta { Code = "CH", Name = "Chalet" },
                new Planta { Code = "DU", Name = "Dúplex" },
                new Planta { Code = "EN", Name = "Entresuelo" },
                new Planta { Code = "ET", Name = "Entreplanta" },
                new Planta { Code = "GA", Name = "Garaje" },
                new Planta { Code = "KI", Name = "Kiosco" },
                new Planta { Code = "MO", Name = "Módulo" },
                new Planta { Code = "LO", Name = "Local" },
                new Planta { Code = "NA", Name = "Nave" },
                new Planta { Code = "OF", Name = "Oficina" },
                new Planta { Code = "PR", Name = "Principal" },
                new Planta { Code = "PT", Name = "Puesto" },
                new Planta { Code = "SA", Name = "Sobreático" },
                new Planta { Code = "SE", Name = "Semisótano" },
                new Planta { Code = "SO", Name = "Sótano" },
                new Planta { Code = "SS", Name = "Semisótano" },
                new Planta { Code = "S1", Name = "Sótano 1" },
                new Planta { Code = "S2", Name = "Sótano 2" },
                new Planta { Code = "S3", Name = "Sótano 3" },
                new Planta { Code = "S4", Name = "Sótano 4" },
                new Planta { Code = "S5", Name = "Sótano 5" },
                new Planta { Code = "S6", Name = "Sótano 6" },
                new Planta { Code = "S7", Name = "Sótano 7" },
                new Planta { Code = "S8", Name = "Sótano 8" },
                new Planta { Code = "S9", Name = "Sótano 9" },
                new Planta { Code = "X1", Name = "Semisótano 1" },
                new Planta { Code = "X2", Name = "Semisótano 2" },
                new Planta { Code = "X3", Name = "Semisótano 3" },
                new Planta { Code = "X4", Name = "Semisótano 4" },
                new Planta { Code = "X5", Name = "Semisótano 5" },
                new Planta { Code = "X6", Name = "Semisótano 6" },
                new Planta { Code = "X7", Name = "Semisótano 7" },
                new Planta { Code = "X8", Name = "Semisótano 8" },
                new Planta { Code = "X9", Name = "Semisótano 9" }
            };
        }

        static List<Bis> GetBisDuplicate()
        {
            return new List<Bis>
            {
                new Bis { Code = " ", Name = "-- BIS/DUPLICADO --" },
                new Bis { Code = "B", Name = "Bis" },
                new Bis { Code = "C", Name = "Cuadruplicado" },
                new Bis { Code = "D", Name = "Duplicado" },
                new Bis { Code = "K", Name = "Kilómetro" },
                new Bis { Code = "Q", Name = "Quintuplicado" },
                new Bis { Code = "T", Name = "Triplicado" },
                new Bis { Code = "X", Name = "Kilómetro margen derecho" },
                new Bis { Code = "Y", Name = "Kilómetro margen izquierdo" }
            };
        }

        static List<Bloque> GetBloque()
        {
            return new List<Bloque>
            {
                new Bloque { Code = " ", Name = "-- BLOQUE --" },
                new Bloque { Code = "A", Name = "Almacén" },
                new Bloque { Code = "B", Name = "Bloque" },
                new Bloque { Code = "C", Name = "Casa" },
                new Bloque { Code = "E", Name = "Edificio" },
                new Bloque { Code = "F", Name = "Estación" },
                new Bloque { Code = "G", Name = "Garaje" },
                new Bloque { Code = "H", Name = "Chalet" },
                new Bloque { Code = "L", Name = "Pabellón" },
                new Bloque { Code = "N", Name = "Nave" },
                new Bloque { Code = "P", Name = "Parcela" },
                new Bloque { Code = "R", Name = "Grupo" },
                new Bloque { Code = "S", Name = "Sector" },
                new Bloque { Code = "T", Name = "Torre" },
                new Bloque { Code = "Z", Name = "Zona" }
            };
        }

        static List<Escalera> GetEscalera()
        {
            return new List<Escalera>
            {
                new Escalera { Code = " ", Name = "-- ESCALERA --" },
                new Escalera { Code = "V", Name = "Izquierda" },
                new Escalera { Code = "W", Name = "Derecha" },
                new Escalera { Code = "X", Name = "Centro" },
                new Escalera { Code = "Y", Name = "Interior" },
                new Escalera { Code = "Z", Name = "Exterior" }
            };
        }

        static string EvaluateEscalera(List<Escalera> tradEscaleras, string valor)
        {
            if (!string.IsNullOrWhiteSpace(valor.Trim()) && tradEscaleras.Any(e => e.Code == valor))
            {
                return tradEscaleras.First(e => e.Code == valor).Name;
            }
            return valor;
        }
    }
}

