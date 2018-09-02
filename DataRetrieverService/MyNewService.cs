using log4net;
using log4net.Config;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Configuration;
using System.IO;
using System.ServiceProcess;
using System.Threading;

namespace DataRetrieverService
{
    public partial class MyNewService : ServiceBase
    {
        public static string strGlobalPath = ConfigurationSettings.AppSettings["GlobalPath"];
        public static string APIKey = ConfigurationSettings.AppSettings["APIKey"];
        private static readonly ILog logMain = LogManager.GetLogger("Main");
        private static readonly ILog logAEMET = LogManager.GetLogger("ThreadAemet");
        private static readonly ILog logGobTraTra = LogManager.GetLogger("ThreadDatosGobTraficoTramas");
        private static readonly ILog logGobTraInt = LogManager.GetLogger("ThreadDatosGobTraficoIntensidad");
        private static readonly ILog logGobTraInc = LogManager.GetLogger("ThreadDatosGobTraficoIncidencias");
        private static readonly ILog logGobTraIncMap = LogManager.GetLogger("ThreadDatosGobTraficoIncidenciasMapa");
        private static readonly ILog logGobCalAir = LogManager.GetLogger("ThreadDatosGobCalidadAire");
        private static readonly ILog logGobConAcu = LogManager.GetLogger("ThreadDatosGobContaminacionAcustica");

        public MyNewService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            XmlConfigurator.Configure();
            logMain.Debug("** STARTING APPLICATION **");
            if (!Directory.Exists(strGlobalPath))
                Directory.CreateDirectory(strGlobalPath);

            System.Threading.Thread ThreadAemet = new System.Threading.Thread(ThreadAemetMethod);
            System.Threading.Thread ThreadDatosGobTraficoTramas = new System.Threading.Thread(ThreadDatosGobTraficoTramasMethod);
            System.Threading.Thread ThreadDatosGobTraficoIntensidad = new System.Threading.Thread(ThreadDatosGobTraficoIntensidadMethod);
            System.Threading.Thread ThreadDatosGobTraficoIncidencias = new System.Threading.Thread(ThreadDatosGobTraficoIncidenciasMethod);
            System.Threading.Thread ThreadDatosGobTraficoIncidenciasMapa = new System.Threading.Thread(ThreadDatosGobTraficoIncidenciasMapaMethod);
            System.Threading.Thread ThreadDatosGobCalidadAire = new System.Threading.Thread(ThreadDatosGobCalidadAireMethod);
            System.Threading.Thread ThreadDatosGobContaminacionAcustica = new System.Threading.Thread(ThreadDatosGobContaminacionAcusticaMethod);

            try
            {
                logMain.Debug("STARTING THREADS");
                ThreadAemet.Start();
                logMain.Debug("ThreadAemet... OK");
                ThreadDatosGobTraficoTramas.Start();
                logMain.Debug("ThreadDatosGobTraficoTramas... OK");
                ThreadDatosGobTraficoIntensidad.Start();
                logMain.Debug("ThreadDatosGobTraficoIntensidad... OK");
                ThreadDatosGobTraficoIncidencias.Start();
                logMain.Debug("ThreadDatosGobTraficoIncidencias... OK");
                ThreadDatosGobTraficoIncidenciasMapa.Start();
                logMain.Debug("ThreadDatosGobTraficoIncidenciasMapa... OK");
                ThreadDatosGobCalidadAire.Start();
                logMain.Debug("ThreadDatosGobCalidadAire... OK");
                ThreadDatosGobContaminacionAcustica.Start();
                logMain.Debug("ThreadDatosGobContaminacionAcustica... OK");
                logMain.Debug("ALL THREADS STARTED");
            }
            catch (Exception e)
            {
                logMain.Error("EXCEPTION WHILE STARTING THREADS: " + e.Message);
                throw e;
            }
        }

        protected override void OnStop()
        {
        }

        private static void ThreadAemetMethod()
        {

            while (true)
            {

                string hourMinute = null;

                string strFolderMA = strGlobalPath + "\\AEMET\\Madrid Aeropuerto\\";
                string strFolderCU = strGlobalPath + "\\AEMET\\Ciudad Universitaria\\";
                string strFolderRe = strGlobalPath + "\\AEMET\\Retiro\\";
                string strFolderCV = strGlobalPath + "\\AEMET\\Cuatro Vientos\\";

                if (!Directory.Exists(strFolderMA))
                    Directory.CreateDirectory(strFolderMA);
                if (!Directory.Exists(strFolderCU))
                    Directory.CreateDirectory(strFolderCU);
                if (!Directory.Exists(strFolderRe))
                    Directory.CreateDirectory(strFolderRe);
                if (!Directory.Exists(strFolderCV))
                    Directory.CreateDirectory(strFolderCV);

                hourMinute = DateTime.Now.ToString("yyyy_MM_dd_HHmm");
                if (hourMinute.Substring(hourMinute.Length - 4) == "0030")
                {
                    try
                    {
                        logAEMET.Debug("TRYING TO GET DATA (Madrid Aeropuerto)");
                        var client = new RestClient("https://opendata.aemet.es/opendata/api/observacion/convencional/datos/estacion/3129/?api_key=" + APIKey);
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);
                        JObject jObject = JObject.Parse(response.Content);
                        JToken value1 = jObject.SelectToken("datos");
                        JToken value2 = jObject.SelectToken("metadatos");

                        client = new RestClient(value1.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);

                        File.WriteAllText(strFolderMA + "datos_AEMET_" + hourMinute + ".txt", response.Content);

                        client = new RestClient(value2.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);
                        File.WriteAllText(strFolderMA + "metadatos_AEMET_" + hourMinute + ".txt", response.Content);

                        logAEMET.Debug("DATA RETRIEVED (Madrid Aeropuerto)");
                    }
                    catch (Exception e)
                    {
                        logAEMET.Error("ERROR: " + e.Message);
                    }

                    try
                    {
                        logAEMET.Debug("TRYING TO GET DATA (Ciudad Universitaria)");
                        var client = new RestClient("https://opendata.aemet.es/opendata/api/observacion/convencional/datos/estacion/3194U/?api_key=" + APIKey);
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);
                        JObject jObject = JObject.Parse(response.Content);
                        JToken value1 = jObject.SelectToken("datos");
                        JToken value2 = jObject.SelectToken("metadatos");

                        client = new RestClient(value1.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);

                        File.WriteAllText(strFolderCU + "datos_AEMET_" + hourMinute + ".txt", response.Content);

                        client = new RestClient(value2.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);
                        File.WriteAllText(strFolderCU + "metadatos_AEMET_" + hourMinute + ".txt", response.Content);

                        logAEMET.Debug("DATA RETRIEVED (Ciudad Universitaria)");
                    }
                    catch (Exception e)
                    {
                        logAEMET.Error("ERROR: " + e.Message);
                    }
                    try
                    {
                        logAEMET.Debug("TRYING TO GET DATA (Retiro)");
                        var client = new RestClient("https://opendata.aemet.es/opendata/api/observacion/convencional/datos/estacion/3195/?api_key=" + APIKey);
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);
                        JObject jObject = JObject.Parse(response.Content);
                        JToken value1 = jObject.SelectToken("datos");
                        JToken value2 = jObject.SelectToken("metadatos");

                        client = new RestClient(value1.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);

                        File.WriteAllText(strFolderRe + "datos_AEMET_" + hourMinute + ".txt", response.Content);

                        client = new RestClient(value2.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);
                        File.WriteAllText(strFolderRe + "metadatos_AEMET_" + hourMinute + ".txt", response.Content);

                        logAEMET.Debug("DATA RETRIEVED (Retiro)");
                    }
                    catch (Exception e)
                    {
                        logAEMET.Error("ERROR: " + e.Message);
                    }
                    try
                    {
                        logAEMET.Debug("TRYING TO GET DATA (Cuatro Vientos)");
                        var client = new RestClient("https://opendata.aemet.es/opendata/api/observacion/convencional/datos/estacion/3196/?api_key=" + APIKey);
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);
                        JObject jObject = JObject.Parse(response.Content);
                        JToken value1 = jObject.SelectToken("datos");
                        JToken value2 = jObject.SelectToken("metadatos");

                        client = new RestClient(value1.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);

                        File.WriteAllText(strFolderCV + "datos_AEMET_" + hourMinute + ".txt", response.Content);

                        client = new RestClient(value2.ToString());
                        request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        response = client.Execute(request);
                        File.WriteAllText(strFolderCV + "metadatos_AEMET_" + hourMinute + ".txt", response.Content);

                        logAEMET.Debug("DATA RETRIEVED (Cuatro Vientos)");
                    }
                    catch (Exception e)
                    {
                        logAEMET.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);
                }
                else
                {
                    logAEMET.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }

        }

        private static void ThreadDatosGobTraficoTramasMethod()
        {
            while (true)
            {
                string strFolder = strGlobalPath + "\\DatosGobierno\\TraficoTramas\\";
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                string hourMinute;

                hourMinute = DateTime.Now.ToString("yyyy - MM - dd - HHmm");
                if (hourMinute.Substring(hourMinute.Length - 1) == "5" || hourMinute.Substring(hourMinute.Length - 1) == "0")
                {
                    try
                    {
                        logGobTraTra.Debug("TRYING TO GET DATA");
                        var client = new RestClient("http://datos.madrid.es/egob/catalogo/208223-7605484-trafico-intensidad-tramas.kml");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);

                        File.WriteAllText(strFolder + "datos_tratra_" + hourMinute + ".txt", response.Content);

                        logGobTraTra.Debug("DATA RETRIEVED");
                    }
                    catch (Exception e)
                    {
                        logGobTraTra.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);
                }
                else
                {
                    logGobTraTra.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }
        }

        private static void ThreadDatosGobTraficoIntensidadMethod()
        {
            while (true)
            {
                string strFolder = strGlobalPath + "\\DatosGobierno\\TraficoIntensidad\\";
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                string hourMinute;

                hourMinute = DateTime.Now.ToString("yyyy - MM - dd - HHmm");
                if (hourMinute.Substring(hourMinute.Length - 1) == "5" || hourMinute.Substring(hourMinute.Length - 1) == "0")
                {
                    try
                    {
                        logGobTraInt.Debug("TRYING TO GET DATA");
                        var client = new RestClient("http://datos.madrid.es/egob/catalogo/202087-0-trafico-intensidad.xml");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);

                        File.WriteAllText(strFolder + "datos_trafint_" + hourMinute + ".kml", response.Content);

                        logGobTraInt.Debug("DATA RETRIEVED");
                    }
                    catch (Exception e)
                    {
                        logGobTraInt.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);

                }
                else
                {
                    logGobTraInt.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }

        }

        private static void ThreadDatosGobTraficoIncidenciasMethod()
        {
            while (true)
            {
                string strFolder = strGlobalPath + "\\DatosGobierno\\TraficoIncidencias\\";
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                string hourMinute;

                hourMinute = DateTime.Now.ToString("yyyy - MM - dd - HHmm");
                if (hourMinute.Substring(hourMinute.Length - 1) == "5" || hourMinute.Substring(hourMinute.Length - 1) == "0")
                {
                    try
                    {
                        logGobTraInc.Debug("TRYING TO GET DATA");
                        var client = new RestClient("http://datos.madrid.es/egob/catalogo/202062-0-trafico-incidencias-viapublica.xml");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);

                        File.WriteAllText(strFolder + "datos_trainc_" + hourMinute + ".xml", response.Content);

                        logGobTraInc.Debug("DATA RETRIEVED");
                    }
                    catch (Exception e)
                    {
                        logGobTraInc.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);

                }
                else
                {
                    logGobTraInc.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }

        }
        private static void ThreadDatosGobTraficoIncidenciasMapaMethod()
        {
            while (true)
            {
                string strFolder = strGlobalPath + "\\DatosGobierno\\TraficoIncidenciasMapa\\";
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                string hourMinute;

                hourMinute = DateTime.Now.ToString("yyyy - MM - dd - HHmm");
                if (hourMinute.Substring(hourMinute.Length - 1) == "5" || hourMinute.Substring(hourMinute.Length - 1) == "0")
                {
                    try
                    {
                        logGobTraIncMap.Debug("TRYING TO GET DATA");
                        var client = new RestClient("http://datos.madrid.es/egob/catalogo/208252-0-incidencias-viapublica-mapa.kml");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);

                        File.WriteAllText(strFolder + "datos_traincmap_" + hourMinute + ".kml", response.Content);

                        logGobTraIncMap.Debug("DATA RETRIEVED");
                    }
                    catch (Exception e)
                    {
                        logGobTraIncMap.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);

                }
                else
                {
                    logGobTraIncMap.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }

        }

        private static void ThreadDatosGobCalidadAireMethod()
        {
            while (true)
            {
                string strFolder = strGlobalPath + "\\DatosGobierno\\CalidadAire\\";
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                string hourMinute;

                hourMinute = DateTime.Now.ToString("yyyy - MM - dd - HHmm");
                if (hourMinute.Substring(hourMinute.Length - 1) == "5" || hourMinute.Substring(hourMinute.Length - 1) == "0")
                {
                    try
                    {
                        logGobCalAir.Debug("TRYING TO GET DATA");
                        var client = new RestClient("http://datos.madrid.es/egob/catalogo/212531-10530806-calidad-aire-tiempo-real.csv");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);

                        File.WriteAllText(strFolder + "datos_calair_" + hourMinute + ".xml", response.Content);

                        logGobCalAir.Debug("DATA RETRIEVED");
                    }
                    catch (Exception e)
                    {
                        logGobCalAir.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);

                }
                else
                {
                    logGobCalAir.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }

        }
        private static void ThreadDatosGobContaminacionAcusticaMethod()
        {
            while (true)
            {
                string strFolder = strGlobalPath + "\\DatosGobierno\\ContaminacionAcustica\\";
                if (!Directory.Exists(strFolder))
                    Directory.CreateDirectory(strFolder);

                string hourMinute;

                hourMinute = DateTime.Now.ToString("yyyy - MM - dd - HHmm");
                if (hourMinute.Substring(hourMinute.Length - 1) == "5" || hourMinute.Substring(hourMinute.Length - 1) == "0")
                {
                    try
                    {
                        logGobConAcu.Debug("TRYING TO GET DATA");
                        var client = new RestClient("http://datos.madrid.es/egob/catalogo/215885-0-contaminacion-ruido.txt");
                        var request = new RestRequest(Method.GET);
                        request.AddHeader("cache-control", "no-cache");
                        IRestResponse response = client.Execute(request);

                        File.WriteAllText(strFolder + "datos_conacu_" + hourMinute + ".txt", response.Content);

                        logGobConAcu.Debug("DATA RETRIEVED");
                    }
                    catch (Exception e)
                    {
                        logGobConAcu.Error("ERROR: " + e.Message);
                    }
                    Thread.Sleep(60000);

                }
                else
                {
                    logGobConAcu.Info("Sleeping...");
                    Thread.Sleep(60000);
                }
            }

        }
    }
}
