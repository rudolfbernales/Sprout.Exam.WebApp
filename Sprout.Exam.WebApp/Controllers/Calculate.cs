namespace Sprout.Exam.WebApp.Controllers
{
    public class Calculate
    {
        public int Id { get; set; }
        public decimal workedDays { get; set; }
        public decimal absentDays { get; set; }

        public decimal rate { get; set; }

        public decimal tax { get; set; }
    }
}