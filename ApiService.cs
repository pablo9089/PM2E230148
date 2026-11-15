namespace PM2E230148;

using System.Net.Http.Json;
using System.Text.Json;

public class ApiException : Exception
{
    public int StatusCode { get; }
    public ApiException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
}

public static class ApiService
{
    private static readonly HttpClient client = new()
    {
        BaseAddress = new Uri(DeviceInfo.Platform == DevicePlatform.Android
            ? "http://10.0.2.2:5080/"
            : "http://localhost:5080/"),
        Timeout = TimeSpan.FromSeconds(10)
    };

    public static string Token { get; private set; } = string.Empty;

    public static async Task LoginAsync(string correo, string clave)
    {
        var response = await client.PostAsJsonAsync("api/auth/login", new { Correo = correo, Clave = clave });

        if (!response.IsSuccessStatusCode)
        {
            throw new ApiException("Datos incorrectos", (int)response.StatusCode);
        }

        var resultado = await response.Content.ReadFromJsonAsync<JsonElement>();
        Token = resultado.GetProperty("token").GetString() ?? string.Empty;

        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Token);
    }

    public static async Task<PerfilModel> ObtenerPerfilAsync()
    {
        var response = await client.GetAsync("api/perfil");

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync() ?? "Error desconocido";
            throw new ApiException(errorContent, (int)response.StatusCode);
        }

        var perfil = await response.Content.ReadFromJsonAsync<PerfilModel>();
        return perfil ?? new PerfilModel("Sin nombre", true, 0);
    }

    public static void CerrarSesion()
    {
        Token = string.Empty;
        client.DefaultRequestHeaders.Authorization = null;
    }
}

public record PerfilModel(string Nombre, bool Disponible, int AniosExperiencia);