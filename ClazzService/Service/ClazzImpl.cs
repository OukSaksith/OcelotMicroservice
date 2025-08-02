using ClazzService.Model;
using System.Reflection;

namespace ClazzService.Service
{
    public class ClazzImpl : List<Model.Clazz>, IClazzRepository
    {
        private readonly static List<Model.Clazz> _students = StudentsSeed();

        private static List<Clazz> StudentsSeed()
        {
            var result = new List<Model.Clazz>()
            {
                new Clazz
                {
                    Id = 1,
                    Name = "Prasad",
                    School = "Mount Fort"                    
                },
                new Clazz
                {
                    Id = 2,
                    Name = "Praveen",
                    School = "Secret Heart"
                },
                new Clazz {
                    Id = 3, 
                    Name = "Pramod", 
                    School = "Bishop Memorial"
                }
            };

            return result;
        }

        public Clazz Get(int id)
        {
            return _students[id];
        }

        public List<Clazz> GetAll()
        {
            return _students;
        }
    }
}
