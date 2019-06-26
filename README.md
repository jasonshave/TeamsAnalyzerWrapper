# Teams Analyzer (API wrapper)

Teams Analyzer is a network assessment tool for measuring network performance and detecting issues with connectivity to Microsoft Teams. Specifically, the system is designed to simiulate a real Teams audio call, measure the network performance, and report the results to your cloud tenant at [teamsanalyzer.com](https://teamsanalyzer.com).

## Purpose

Some customers are using Power BI to visualize their data from the teamsanalyzer.com API. Power BI supports injecting a header value containing the subscription key (Ocp-Apim-Subscription-Key) value. This key is required to authenticate to the API 
