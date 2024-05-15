using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApplicationFapp.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace WebApplicationFapp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient client = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        //public async Task<IActionResult> Index()
        //{
        //    using HttpResponseMessage response = await client.GetAsync("http://www.contoso.com/");
        //    response.EnsureSuccessStatusCode();
        //    string responseBody = await response.Content.ReadAsStringAsync();
        //    Console.WriteLine(responseBody);

        //    return View();
        //}
        //[Route("Hello")]
        //public IActionResult method1()
        //{
        //    //return ("Hello");
        //    return new RedirectToActionResult("Books", "Store", new { });
        //}
        [HttpPost]
        public async Task<IActionResult> Index(SalesRequest salesRequest, IFormFile file)
        {
            salesRequest.Id = Guid.NewGuid().ToString();

            using (var content = new StringContent(JsonConvert.SerializeObject(salesRequest),
                System.Text.Encoding.UTF8, "application/json"))
            {
                //call our function and pass the content

                HttpResponseMessage response = await client.PostAsync(" http://localhost:7209/api/OnSalesUploadWriteToQueue", content);
                string returnValue = response.Content.ReadAsStringAsync().Result;
            }
            //if (file != null)
            //{
            //    var fileName = salesRequest.Id + Path.GetExtension(file.FileName);
            //    BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient("functionsalesrep");
            //    var blobClient = blobContainerClient.GetBlobClient(fileName);

            //    var httpHeaders = new BlobHttpHeaders
            //    {
            //        ContentType = file.ContentType
            //    };

            //    await blobClient.UploadAsync(file.OpenReadStream(), httpHeaders);
            //    return View();
            //}

            return RedirectToAction(nameof(Index));
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> LeaveRequest()
        {
            dynamic content = new { employeeId = "10" };
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(HttpMethod.Post, " http://localhost:7209/api/LeaveRequestList?employeeid=5"))

            using (var httpContent = CreateHttpContent(content))
            {
                request.Content = httpContent;

                using (var response = await client
                    .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                    .ConfigureAwait(false))
                {
                    // response.EnsureSuccessStatusCode();

                    var resualtList = response.Content.ReadAsStringAsync().Result;

                    ViewData["LeaveRequest"] = resualtList;

                    //return RedirectToAction(nameof(LeaveRequest));
                    return View();
                }
            }
        }

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                var ms = new MemoryStream();
                SerializeJsonIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            return httpContent;
        }
        public static void SerializeJsonIntoStream(object value, Stream stream)
        {
            using (var sw = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (var jtw = new JsonTextWriter(sw) { Formatting = Formatting.None })
            {
                var js = new JsonSerializer();
                js.Serialize(jtw, value);
                jtw.Flush();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
