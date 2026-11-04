namespace LigaD.Api.Config;

/// <summary>
/// Sección "ConnectionStrings" de appsettings. En desarrollo apunta a BD_LIGAD en .\SQLEXPRESS
/// (autenticación de Windows, sin secretos); en el servidor va en la variable de entorno
/// ConnectionStrings__DefaultConnection.
/// </summary>
public class DatabaseSettings
{
    public string DefaultConnection { get; set; } = string.Empty;
}
