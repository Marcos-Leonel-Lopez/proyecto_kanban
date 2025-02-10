public class NoAutorizadoException : Exception
{
    public NoAutorizadoException(string usuario, int idUsuario, string ruta)
    :base($"Acceso denegado - Usuario: {usuario} (id {idUsuario}) intentó acceder a {ruta} sin permisos.") 
    {}
}
