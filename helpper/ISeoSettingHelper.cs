namespace WebThuMuaPheLieu.helpper;

public interface ISeoSettingHelper
{
    Task<IReadOnlyDictionary<string, string>> GetSeoSettingsAsync();

    Task<string> GetSeoSettingValueAsync(string key);
}
