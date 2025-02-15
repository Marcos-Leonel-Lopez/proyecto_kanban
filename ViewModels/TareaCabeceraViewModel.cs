    public class TareaCabeceraViewModel
    {
        public int? IdUsuarioAsignado { get; set; }
        public bool EsPropietario { get; set; }
        public List<UsuarioViewModel> Usuarios { get; set; }
        public int IdTarea {get; set;}
    }