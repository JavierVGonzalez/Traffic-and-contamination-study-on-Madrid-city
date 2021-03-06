This a Windows Service, written in C#, whose goal is to download traffic and metheorology data from two different sources: AEMET (Agencia Estatal de Meteorología, from Spain) and datos.gob.es (an open data web from Spain). Once installed and configured, this service will download the data by itself. There are many other data catalogs that can be downloaded in the same way, but since the URLs can be different, some trivial changes to the code may be required.

To install the service, use (you have to change the "..." to the location of your project):

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe "c:\...\bin\Release\DataRetrieverService.exe"

And to uninstall, use:

C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe /u "c:\...\bin\Release\DataRetrieverService.exe"

In the app.config file you need to configure:
1) Logging paths:

    <param name="File" value="C:\*LOGGING MAIN PATH*\logs\main.log" />  

One for the main app and one for each of the threads. Can be different.

2) Download folder and API Key

    <appSettings>
        <add key="GlobalPath" value="C:\*MAIN DATA PATH*" />
        <add key="APIKey" value="*API KEY*" />
        <add key="ClientSettingsProvider.ServiceUri" value="" />
    </appSettings>
    
Path where the data files will be downloaded, and API KEY that will be used for the AEMET API. AEMET API KEYS are free and have a duration of 3 months.
More info at https://opendata.aemet.es/centrodedescargas/inicio
