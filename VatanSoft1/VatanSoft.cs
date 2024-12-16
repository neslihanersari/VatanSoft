using System;
using System.Net.Http;
using System.Threading.Tasks;

class VatanSoft
{
    static async Task Main(string[] args)
    {
        string apiUrl = "https://random.dog/woof.json";
        int largeFiles = 0;
        int smallFiles = 0;
        int totalRequests = 100;

        using (HttpClient client = new HttpClient())
        {
            for (int i = 0; i < totalRequests; i++)
            {
                try
                {
                    // API'den JSON yanıtını almak 
                    HttpResponseMessage response = await client.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    // JSON'dan URL'yi ayıkla
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    string imageUrl = ExtractUrlFromJson(jsonResponse);

                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        // Resim dosyasını indir
                        HttpResponseMessage imageResponse = await client.GetAsync(imageUrl);
                        imageResponse.EnsureSuccessStatusCode();

                        long fileSize = imageResponse.Content.Headers.ContentLength ?? 0;

                        // Boyut kontrolü
                        if (fileSize > 1050000)
                        {
                            largeFiles++;
                        }
                        else
                        {
                            smallFiles++;
                        }

                        Console.WriteLine($"Request {i + 1}: File Size = {fileSize} bytes");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Request {i + 1} failed: {ex.Message}");
                }
            }
        }

        // Sonuçları yazdır
        Console.WriteLine("\n=== Results ===");
        Console.WriteLine($"Files larger than 1.05 MB: {largeFiles}");
        Console.WriteLine($"Files smaller than 1.05 MB: {smallFiles}");

        // Konsolun kapanmaması için bekletme işlemi
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    static string ExtractUrlFromJson(string json)
    {
        // JSON URL ayıklama işlemi
        int startIndex = json.IndexOf("\"url\":\"") + 7;
        int endIndex = json.IndexOf("\"", startIndex);

        if (startIndex >= 7 && endIndex > startIndex)
        {
            return json.Substring(startIndex, endIndex - startIndex);
        }

        return null;
    }
}
