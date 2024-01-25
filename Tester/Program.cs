using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tester;


Console.WriteLine(GetPerformance());

static List<CryptoCoin> GetPerformance()
{
    var limit = 101;
    var convert1 = "USD,SEK";
    var endpoint1 = new Uri($"https://pro-api.coinmarketcap.com/v1/cryptocurrency/listings/latest?convert={convert1}&limit={limit}"); // valde listings mest för test


    // endpoint.Query = queryString.ToString();

    string API_KEY1 = "63f9b42b-7067-41cf-803e-00025ed9b664"; // min egna api key har 10000 kostnadsfria calls per månad
    var client1 = new HttpClient();
    client1.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", API_KEY1);
    client1.DefaultRequestHeaders.Add("Accepts", "application/json");
    var result1 = client1.GetAsync(endpoint1).Result;
    var json1 = result1.Content.ReadAsStringAsync().Result;
    var listings = JsonSerializer.Deserialize<Listings>(json1);
    List<string> cryptoSymbolsList = new List<string>();

    foreach (var crypto in listings.data)
    {
        cryptoSymbolsList.Add(crypto.symbol.ToString());
    }


    string cryptoSymbolsString = string.Join(",", cryptoSymbolsList);

    var timePeriod = "all_time,yesterday,24h,7d,30d,90d,365d";
    var id = cryptoSymbolsString.ToUpper();
    var endpoint = $"https://pro-api.coinmarketcap.com/v2/cryptocurrency/price-performance-stats/latest?symbol={id}&time_period={timePeriod}";
    string apiKey = "63f9b42b-7067-41cf-803e-00025ed9b664";
    var client = new HttpClient();
    client.DefaultRequestHeaders.Add("X-CMC_PRO_API_KEY", apiKey);
    client.DefaultRequestHeaders.Add("Accepts", "application/json");
    var result = client.GetAsync(endpoint).Result;
    var json = result.Content.ReadAsStringAsync().Result;
    var performances = JsonSerializer.Deserialize<Performances>(json);
    // market_cap: 554278688.6651919, market_cap: 563851743.1169437
    List<CryptoCoin> cryptoCoin = new();

    foreach (var coin in listings.data)
    {
        float roundedPercentChange = (float)Math.Round(coin.quote.USD.percent_change_24h, 2);
        float BmarketCap = coin.quote.USD.market_cap / 1000000000;
        float roundedMarketCap = (float)Math.Round(BmarketCap, 3);
        var symbolToSearch = coin.symbol; // replace with the symbol you're looking for
                                          //var performanceData = performances.data.FirstOrDefault(pair => pair.Key == symbolToSearch).Value;


        var performanceData = performances.data.Where(x => x.Key == coin.symbol).FirstOrDefault().Value.FirstOrDefault();

        float? openPrice = performanceData.periods._30d.quote.USD.open;
        
        cryptoCoin.Add(new CryptoCoin
        {
            Price = coin.quote.USD.price,
            Summary = coin.symbol.ToUpper(),
            CryptoName = coin.name.ToUpper(),
            PriceSEK = coin.quote.SEK.price,
            PercentChange = roundedPercentChange,
            MarketCap = roundedMarketCap,// Converted to Billions
            ID = coin.cmc_rank,
            OpenPrice = openPrice
        });
        //   var bitcoinValueSek = Convert.ToDecimal(bitcoinValueUsd) * 10; // får fram real bitcoin data i USD valuta.
    }
    return cryptoCoin;
}




