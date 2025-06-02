#define LOCAL_SERVER
//#define DEVELOP_SERVER

namespace GameUtil
{
    public class Uri
    {
#if LOCAL_SERVER
        ///<summary>ログイン</summary>
        public const string Login = "http://localhost/api/login";
        ///<summary>アカウント登録</summary>
        public const string Register = "http://localhost/api/register";
        ///<summary>ホーム</summary>
        public const string Home = "http://localhost/api/home";
        ///<summary>ウォレット</summary>
        public const string Wallet = "http://localhost/api/home";

#elif DEVELOP_SERVER
        ///<summary>ログイン</summary>
        public const string login = "";
        ///<summary>アカウント登録</summary>
        public const string Register = "";
#endif
    }
}