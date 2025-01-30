public class NoEncontradoException : Exception
{
    public NoEncontradoException(string entidad, int id) :
    base($"{entidad.ToUpper()} id: {id} - Recurso no encontrado.") 
    {}
}