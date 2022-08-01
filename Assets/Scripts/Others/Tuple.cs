public class Tuple
{
    public static Tuple<T1, T2> Create<T1, T2>(T1 t1, T2 t2)
    {
        return new Tuple<T1, T2>(t1, t2);
    }

    public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 t1, T2 t2, T3 t3)
    {
        return new Tuple<T1, T2, T3>(t1, t2, t3);
    }

    public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 t1, T2 t2, T3 t3, T4 t4)
    {
        return new Tuple<T1, T2, T3, T4>(t1, t2, t3, t4);
    }

    public static Tuple<T1, T2, T3, T4, T5> Create<T1, T2, T3, T4, T5>(T1 t1, T2 t2, T3 t3, T4 t4, T5 t5)
    {
        return new Tuple<T1, T2, T3, T4, T5>(t1, t2, t3, t4, t5);
    }
}

public class Tuple<T1, T2>
{
    public T1 first;
    public T2 second;

    public Tuple(T1 first, T2 second)
    {
        this.first = first;
        this.second = second;
    }
}

public class Tuple<T1, T2, T3>
{
    public T1 first;
    public T2 second;
    public T3 third;

    public Tuple(T1 first, T2 second, T3 third)
    {
        this.first = first;
        this.second = second;
        this.third = third;
    }
}

public class Tuple<T1, T2, T3, T4>
{
    public T1 first;
    public T2 second;
    public T3 third;
    public T4 fourth;

    public Tuple(T1 first, T2 second, T3 third, T4 fourth)
    {
        this.first = first;
        this.second = second;
        this.third = third;
        this.fourth = fourth;
    }
}

public class Tuple<T1, T2, T3, T4, T5>
{
    public T1 first;
    public T2 second;
    public T3 third;
    public T4 fourth;
    public T5 fifth;

    public Tuple(T1 first, T2 second, T3 third, T4 fourth, T5 fifth)
    {
        this.first = first;
        this.second = second;
        this.third = third;
        this.fourth = fourth;
        this.fifth = fifth;
    }
}