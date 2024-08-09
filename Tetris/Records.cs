using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class Records
    {
        public void saveScore( int score)
        {
            Record Rec = getRecords();
            for (int i = 0; i < Rec.scores.Count; i++)
            {
                if (Rec.scores[i] <= score)
                {
                    Rec.scores.Insert(i, score);
                    if (Rec.scores.Count > 10)
                    {
                        Rec.scores.RemoveAt(Rec.scores.Count - 1);
                    }
                    break;
                }
            }
            setRecords(Rec);
        }

        public Record getRecords()
        {
            StreamReader srRecords = new StreamReader(@"C:\Users\User\records.txt");
            

            List<int> rec = new List<int>();
            
            string numbers = srRecords.ReadLine();
            foreach (string number in numbers.Split())
            {
                int num = int.Parse(number);
                rec.Add(num);
            }
            
            Record Rec = new Record(rec);
            srRecords.Close();
            return Rec;
        }

        public static void setRecords(Record Rec)
        {
            StreamWriter swRec = new StreamWriter(@"C:\Users\User\records.txt");
            
            foreach (int score in Rec.scores)
            {
                swRec.Write(score);
                if (!(Rec.scores.Last() == score))
                    swRec.Write(' ');
            }
            swRec.Close();
        }
    }

    public class Record
    {
        public List<int> scores;

        public Record()
        {

        }

        public Record( List<int> scores)
        {
            this.scores = scores;
        }
    }
}
