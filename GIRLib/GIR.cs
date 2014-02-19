using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRLib
{
    public class GIR
    {

        public static List<int> GetGIR(int numIndices)
        {
            Driver driver = new Driver(numIndices);

            int tick = 0;
            List<int> ints = new List<int>();
            int obj = 0;
            while ((obj = driver.GetNextRequiredIndex()) != -1)
            {
                ints.Add(obj);

                tick++;
            }

            return ints;
        }
    }
}
