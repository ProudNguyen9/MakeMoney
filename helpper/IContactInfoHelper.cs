using WebThuMuaPheLieu.Models;

namespace WebThuMuaPheLieu.helpper;

public interface IContactInfoHelper
{
    Task<ContactInfoSettings> GetContactInfoAsync();
}
