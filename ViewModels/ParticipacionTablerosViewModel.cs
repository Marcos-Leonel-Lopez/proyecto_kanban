public class ParticipacionTablerosViewModel
{
    private List<TableroViewModel> tablero_propio;
    private List<TableroViewModel> tablero_participante;
    public ParticipacionTablerosViewModel(){}
    public ParticipacionTablerosViewModel(List<TableroViewModel> tablero_propio, List<TableroViewModel> tablero_participante){
        this.tablero_propio = tablero_propio;
        this.tablero_participante = tablero_participante;
    }
    public List<TableroViewModel> Tablero_propio { get => tablero_propio; set => tablero_propio = value; }
    public List<TableroViewModel> Tablero_participante { get => tablero_participante; set => tablero_participante = value; }
}