namespace IUsuarioRepo
{
    public interface IUsuarioRepository{
        Usuario Create(Usuario usuario);
        bool EditarPerfil(DataUsuario usuario, int id);
        List<Usuario> GetAll();
        Usuario GetById(int id);
        bool Remove(int id);
        Usuario UpdatePass(Usuario usuario, int id);
    }
}