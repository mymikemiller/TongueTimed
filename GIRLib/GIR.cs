using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GIRLib
{
    public class GIR<T>
    {

        public static List<T> GetGIR(List<T> objectsToGIR)
        {
            

            Driver<T> driver = new Driver<T>(objectsToGIR);

            int tick = 0;
            List<T> objects = new List<T>();
            T obj = default(T);
            while ((obj = driver.GetNextRequiredObject()) != null)
            {
                objects.Add(obj);
                //Console.WriteLine("Recallable: " + obj);

                tick++;
            }

            return objects;
        }
    }
}
