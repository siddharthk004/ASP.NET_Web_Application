using Role_Base_Authentication.Models;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

public class AuthApiController : ApiController
{
    private SanmolEnt db = new SanmolEnt();

    [HttpPost]
    [Route("api/auth/login")]
    public IHttpActionResult Login(LoginRequest model)
    {
        if (model == null)
            return BadRequest("Invalid request");

        var user = db.registrations
                     .FirstOrDefault(u =>
                         u.name == model.Name &&
                         u.password == model.Password);

        if (user == null)
        {
            return Unauthorized();
        }

        return Ok(new
        {
            message = "Login successful",
            name = user.name,
            role = user.role
        });
    }

    [HttpPost]
    [Route("api/chat")]
    public async Task<IHttpActionResult> Chat(String MessageX)
    {
        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add(
                "Authorization",
                "Bearer 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"
            );

            var payload = new
            {
                model = "llama3-8b-8192",
                messages = new[]
                {
                    new { role = "user", content = MessageX }
                }
            };

            var response = await client.PostAsJsonAsync(
                "https://api.groq.com/openai/v1/chat/completions",
                payload
            );

            var result = await response.Content.ReadAsStringAsync();
            return new System.Web.Http.Results.ResponseMessageResult(
                new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(result, System.Text.Encoding.UTF8, "application/json")
                }
            );
        }
    }
}
