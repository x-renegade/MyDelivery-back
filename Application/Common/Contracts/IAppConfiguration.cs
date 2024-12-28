namespace Application.Common.Contracts;

public interface IAppConfiguration
{
    string GetValue(string key);
}
