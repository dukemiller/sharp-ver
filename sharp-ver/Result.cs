namespace sharp_ver
{
    public class Result<T>
    {
        public T Value { get; set; }
        public bool Successful { get; set; } = true;
        public string ErrorMessage { get; set; }
    }
}