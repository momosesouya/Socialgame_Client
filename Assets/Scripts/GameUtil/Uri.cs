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
        ///<summary>マスタデータチェックURL</summary>
        public const string Master_Check_URL = "http://localhost/api/masterCheck";
        ///<summary>マスタデータ取得URL </summary>
        public const string Master_Get_URL = "http://localhost/api/masterGet";
        ///<summary>ウォレット</summary>
        public const string Wallet = "http://localhost/api/home";
        ///<summary>通貨購入</summary>
        public const string Buy_Currency = "http://localhost/api/buyCurrency";
        ///<summary>スタミナ回復</summary>
        public const string Stamina_Recovery = "http://localhost/api/staminaRecovery";

#elif DEVELOP_SERVER
        ///<summary>ログイン</summary>
        public const string login = "";
        ///<summary>アカウント登録</summary>
        public const string Register = "";
#endif
    }
}