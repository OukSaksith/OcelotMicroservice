namespace ClazzService.Service
{
    public interface IClazzRepository
    {
        List<Model.Clazz> GetAll();
        Model.Clazz Get(int id);
    }
}
