
namespace proiectdaw4.Model
{
    public class Inchiriere
    {
        public int Id { get; set; }

        public int ProprietateId { get; set; }
        public Proprietate Proprietate { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public DateOnly DataInceput { get; set; }
        public DateOnly DataFinal { get; set; }

        public int NumarZile { get; set; }
        public decimal PretTotal { get; set; }

        public Inchiriere()
        {
            NumarZile = 0;
            PretTotal = 0;
        }
        public Inchiriere(int proprietateId, int userId, DateOnly dataInceput, DateOnly dataFinal, decimal pretTotal)
        {
            ProprietateId = proprietateId;
            UserId = userId;
            DataInceput = dataInceput;
            DataFinal = dataFinal;
            PretTotal = pretTotal;
            NumarZile = (DataFinal.ToDateTime(new TimeOnly()) - DataInceput.ToDateTime(new TimeOnly())).Days;
        }
    }
}