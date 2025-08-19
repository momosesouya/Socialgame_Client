using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameUtil
{
    public class Common
    {
        #region
        /// <summary>SQLiteファイル名</summary>
        public const string DBFileName = "service.db";
        #endregion

        #region
        /// <summary>バージョンチェック</summary>
        public const string SAVE_KEY_MASTER_VERSION = "";
        #endregion

        #region エラーコード関連
        /// <summary>バリデーションエラー</summary>
        public const int ErrCode_DbUpdate = 100;
        /// <summary>マスターバージョンエラー</summary>
        public const int ErrCode_MasterVersion = 200;
        /// <summary>認証エラー</summary>
        public const int ErrCode_Auth = 403;
        #endregion

        #region エラーメッセージ
        /// <summary>リクエスト失敗</summary>
        public const string ErrMsg_RequestFailed = "通信に失敗しました。";
        /// <summary>通信失敗</summary>
        public const string ErrMsg_RegisterFailed = "登録に失敗しました。";
        /// <summary>ユーザー名入力エラー</summary>
        public const string ErrMsg_NameInput = "入力エラーです。";
        #endregion

        #region
        /** エラーID */
        public const string ERROR_DB_UPDATE = "1";
        public const string ERROR_MASTER_DATA_UPDATE = "2";
        #endregion
    }

    public class RequestResult
    {
        public int result;
    }
}
