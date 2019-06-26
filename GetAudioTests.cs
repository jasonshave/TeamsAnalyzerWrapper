using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace TeamsAnalyzer.Reporting
{
    public class GetAudioTests
    {
        private readonly HttpClient httpClient;

        //Get the subscription key from local.settings.json (if local) or App Settings in Azure or Key Vault
        private static string key = Environment.GetEnvironmentVariable("TeamsAnalyzerSubscriptionKey");
        
        public GetAudioTests(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("GetAudioTests")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function, 
                "get", 
                Route = "AudioTest/{DomainName}/{NodeId}")] HttpRequest req, 
                ILogger log, 
                string domainName, 
                string nodeId
            )
        {
            log.LogInformation($"Performing audio test collection for domain: {domainName} on node: {nodeId}.");
            
            //Add the subscription key to the header
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            //Make the call to the API
            var httpResp = await httpClient.GetStringAsync($"https://api.teamsanalyzer.com/beta/reporting/AudioTest/{domainName}/{nodeId}");
            
            return httpResp != null
                ? (ActionResult)new OkObjectResult(httpResp)
                : new BadRequestObjectResult("There was a problem processing your request.");
        }
    }
}
