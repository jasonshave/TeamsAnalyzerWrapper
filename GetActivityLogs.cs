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
    public class GetActivityLogs
    {
        private readonly HttpClient httpClient;
        private readonly string subscriptionKeyName = "TeamsAnalyzerSubscriptionKey";
        
        public GetActivityLogs(IHttpClientFactory httpClientFactory)
        {
            httpClient = httpClientFactory.CreateClient();
        }

        [FunctionName("GetActivityLogs")]
        public async Task<IActionResult> Run(
            [HttpTrigger(
                AuthorizationLevel.Function, 
                "get", 
                Route = "ActivityLog/{DomainName}/{NodeId}")] HttpRequest req, 
                ILogger log, 
                string domainName, 
                string nodeId
            )
        {
            log.LogInformation($"Performing activity log collection for domain: {domainName} on node: {nodeId}.");
            
            //Get subscription key from Key Vault or local.settings.json
            var key = Environment.GetEnvironmentVariable(subscriptionKeyName);

            //Add the subscription key to the header
            httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);

            //Make the call to the API
            var httpResp = await httpClient.GetStringAsync($"https://api.teamsanalyzer.com/beta/reporting/ActivityLog/{domainName}/{nodeId}");
            
            return httpResp != null
                ? (ActionResult)new OkObjectResult(httpResp)
                : new BadRequestObjectResult("There was a problem processing your request.");
        }
    }
}
