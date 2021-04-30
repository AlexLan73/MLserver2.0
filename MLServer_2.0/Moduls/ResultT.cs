namespace Convert.Moduls
{
    public struct SResulT0
    {
        public SResulT0(int error, string nameError, string nameRazdel, int? id = null)
        {
            Error = error;
            NameError = nameError;
            NameRazdel = nameRazdel;
            Id = id;
        }

        public int? Error { get; set; }
        public string NameError { get; set; }
        public string NameRazdel { get; set; }
        public int? Id { get; set; }
    }

    public class ResultT<T>
    {
        public readonly T Value;
        public readonly SResulT0? Error;

        public ResultT(T value)
        {
            Value = value;
        }

        public ResultT(SResulT0 error)
        {
            this.Error = error;
        }

        public bool HasValue => Error == null;

        public static implicit operator bool(ResultT<T> resultT)
        {
            return resultT.HasValue;
        }
    }

    public class ResultTd1<T, E>
    {
        private bool Is { get; set; }
        public ResultTd1(T value)
        {
            Is = false;
            Value = value;
        }

        public ResultTd1(E error)
        {
            Is = true;
            Error = error;
        }

        public bool HasValue => Is;


        public static implicit operator bool(ResultTd1<T, E> result)
        {
            return result.HasValue;
        }
        public T Value { get; private set; }
        public E Error { get; private set; }
    }
}

