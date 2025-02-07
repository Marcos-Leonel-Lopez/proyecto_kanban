public class AccesoDenegadoException : Exception
{
    public AccesoDenegadoException(string user, string pass) :
    base($"Intento de acceso invalido- Usuario: {user} Clava ingresada: {pass}") 
    {}
}


