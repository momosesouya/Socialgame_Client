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

#elif DEVELOP_SERVER
        ///<summary>ログイン</summary>
        public const string login = "";
        ///<summary>アカウント登録</summary>
        public const string Register = "";
#endif
    }
}