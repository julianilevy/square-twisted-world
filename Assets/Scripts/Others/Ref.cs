public class Ref
{
    public static Ref<T> Create<T>(T value)
    {
        return new Ref<T>(value);
    }
}

public class Ref<T>
{
    public T value;

    public Ref(T value)
    {
        this.value = value;
    }
}