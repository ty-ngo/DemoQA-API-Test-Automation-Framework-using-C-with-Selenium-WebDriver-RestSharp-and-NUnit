using Core.Utilities;
using DemoQA.APITesting.Constants;
using DemoQA.Services.Model.DataObject;

namespace DemoQA.APITesting.DataProvider
{
    public class UserInfoDataProvider
    {

        private static readonly Dictionary<string, AccountDto> _accountDto;

        static UserInfoDataProvider()
        {
            _accountDto = JsonUtils.ReadDictionaryJson<AccountDto>(FilePathConstants.AccountPath);
        }

        public static AccountDto GetAccountData(string key)
        {
            if (_accountDto.ContainsKey(key))
                return _accountDto[key];

            return null;
        }
    }
}