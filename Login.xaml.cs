namespace PM2E230148;

public partial class Login : ContentPage
{
    private int intentosFallidos = 0;

    public Login()
    {
        InitializeComponent();
    }

    private void OnMostrarClaveChanged(object sender, CheckedChangedEventArgs e)
    {
        EntryClave.IsPassword = !e.Value;
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(EntryCorreo.Text) || string.IsNullOrWhiteSpace(EntryClave.Text))
        {
            await DisplayAlert("Error", "Falta el correo o la contraseña", "OK");
            return;
        }

        ActivityCarga.IsRunning = true;
        ActivityCarga.IsVisible = true;
        BtnLogin.IsEnabled = false;

        try
        {
            await ApiService.LoginAsync(EntryCorreo.Text.Trim(), EntryClave.Text.Trim());
            intentosFallidos = 0;
            await Navigation.PushAsync(new Perfil());
        }
        catch (ApiException ex)
        {
            if (ex.StatusCode == 401)
            {
                intentosFallidos++;
                if (intentosFallidos >= 3)
                {
                    BtnLogin.IsEnabled = false;
                    await DisplayAlert("Bloqueado", "Demasiados intentos fallidos (401). Acceso bloqueado.", "OK");
                    return;
                }
                await DisplayAlert("Error", $"Datos incorrectos. Intento {intentosFallidos} de 3.", "OK");
            }
            else
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        catch (Exception)
        {
            await DisplayAlert("Error de conexión", "La API está apagada o no se puede conectar al servidor.", "OK");
        }
        finally
        {
            ActivityCarga.IsRunning = false;
            ActivityCarga.IsVisible = false;
            if (intentosFallidos < 3)
            {
                BtnLogin.IsEnabled = true;
            }
        }
    }
}