namespace Settings
{
	public interface ISetting<T>
	{
		T Value { get; set; }
	}
}