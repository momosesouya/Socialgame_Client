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
        ///<summary>通貨購入</summary>
        public const string Buy_Currency = "http://localhost/api/buyCurrency";
        ///<summary>スタミナ消費</summary>
        public const string Stamina_Consumption = "http://localhost/api/staminaConsumption";
        ///<summary>ガチャ</summary>
        public const string Gacha_Execute = "http://localhost/api/gachaExecute";
        ///<summary>ガチャログ</summary>
        public const string Gacha_Log = "http://localhost/api/gachaLog";
        ///<summary>武器強化</summary>
        public const string LevelUp = "http://localhost/api/levelUp";
#elif DEVELOP_SERVER
        ///<summary>ログイン</summary>
        public const string Login = "https://sg.momosegame.com/api/login";
        ///<summary>アカウント登録</summary>
        public const string Register = "https://sg.momosegame.com/api/register";
        ///<summary>ホーム</summary>
        public const string Home = "https://sg.momosegame.com/api/home";
        ///<summary>マスタデータチェックURL</summary>
        public const string Master_Check_URL = "https://sg.momosegame.com/api/masterCheck";
        ///<summary>マスタデータ取得URL </summary>
        public const string Master_Get_URL = "https://sg.momosegame.com/api/masterGet";
        ///<summary>通貨購入</summary>
        public const string Buy_Currency = "https://sg.momosegame.com/api/buyCurrency";
        ///<summary>スタミナ消費</summary>
        public const string Stamina_Consumption = "https://sg.momosegame.com/api/staminaConsumption";
        ///<summary>ガチャ</summary>
        public const string Gacha_Execute = "https://sg.momosegame.com/api/gachaExecute";
        ///<summary>ガチャログ</summary>
        public const string Gacha_Log = "https://sg.momosegame.com/api/gachaLog";
        ///<summary>武器強化</summary>
        public const string LevelUp = "https://sg.momosegame.com/api/levelUp";
#endif
    }
}