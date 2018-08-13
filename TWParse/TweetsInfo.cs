namespace TWParse
{
    //класс, описывающий частотность буквы
    public class TweetsInfo
    {
        public char Letter { get; set; }
        public double Freq { get; set; }

        public TweetsInfo(char ltr, double frq)
        {
            Letter = ltr;
            Freq = frq;
        }
    }
}



