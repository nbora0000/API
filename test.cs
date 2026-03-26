using System;
using System.Net.Http;
using System.Threading.Tasks;
class Program {
    static async Task Main() {
        var client = new HttpClient();
        try {
            var response = await client.GetAsync("http://localhost:5029/api/orders");
            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        } catch (Exception e) {
            Console.WriteLine(e);
        }
    }
}
