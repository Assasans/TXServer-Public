using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1470735489716)]
    public class UserCountryComponent : Component
    {
        public UserCountryComponent(string CountryCode)
        {
            this.CountryCode = CountryCode;
        }

        public string CountryCode { get; set; }
    }
}
