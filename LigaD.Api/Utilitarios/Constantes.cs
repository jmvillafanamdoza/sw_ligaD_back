namespace LigaD.Api.Utilitarios;

public static class Constantes
{
    /// <summary>Etiquetas de los grupos de endpoints en OpenAPI/Scalar.</summary>
    public static class Grupos
    {
        public const string Salud = "Salud";
        public const string Catalogos = "Catálogos";
    }

    public static class Mensajes
    {
        public const string OperacionExitosa = "Operación realizada correctamente.";
        public const string ErrorInterno = "Ocurrió un error interno. Intenta nuevamente más tarde.";
        public const string ErrorValidacion = "Errores de validación.";
    }
}
