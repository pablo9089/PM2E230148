namespace PM2E230148;

public partial class Perfil : ContentPage
{
    public Perfil()
    {
        InitializeComponent();
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        try
        {
            var perfil = await ApiService.ObtenerPerfilAsync();
            if (perfil != null)
            {
                LblNombre.Text = perfil.Nombre ?? "Usuario";
                SwitchDisponible.IsToggled = perfil.Disponible;

                SliderExperiencia.ValueChanged -= OnSliderValueChanged;
                SliderExperiencia.Value = perfil.AniosExperiencia;
                ActualizarInterfazSlider(perfil.AniosExperiencia);
                SliderExperiencia.ValueChanged += OnSliderValueChanged;
            }
        }
        catch (ApiException ex)
        {
            await DisplayAlert("Sesión", ex.Message, "OK");
            await Navigation.PopAsync();
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "No se pudo conectar con el servidor para obtener el perfil.", "OK");
        }
    }

    private void OnSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        ActualizarInterfazSlider((int)e.NewValue);
    }

    private void ActualizarInterfazSlider(int anios)
    {
        LblAnios.Text = $"{anios} años de experiencia";

        if (anios >= 0 && anios <= 2)
        {
            LblNivel.Text = "Nivel Inicial";
            BorderContenedor.BackgroundColor = Color.FromArgb("#DCFCE7");
        }
        else if (anios >= 3 && anios <= 7)
        {
            LblNivel.Text = "Nivel Intermedio";
            BorderContenedor.BackgroundColor = Color.FromArgb("#E0F2FE");
        }
        else
        {
            LblNivel.Text = "Nivel Avanzado";
            BorderContenedor.BackgroundColor = Color.FromArgb("#FEF3C7");
        }
    }

    private async void OnCerrarSesionClicked(object sender, EventArgs e)
    {
        ApiService.CerrarSesion();
        await Navigation.PopToRootAsync();
    }
}