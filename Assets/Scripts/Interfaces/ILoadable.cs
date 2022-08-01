public interface ILoadable<T> where T : TransformData
{
    void LoadData(T data);
}