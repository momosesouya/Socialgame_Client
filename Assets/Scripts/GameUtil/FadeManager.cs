using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace GameUtil
{
    /// <summary>
    /// シーン遷移用簡易フェードクラス
    /// </summary>
    public class FadeManager : MonoBehaviour
    {
        private static Canvas canvas;
        private static Image image;
        private static Text loadingText;

        private static FadeManager instance;
        public static FadeManager Instance
        {
            get
            {
                if (instance == null) { Init(); }
                return instance;
            }
        }

        IEnumerator fadeCoroutine = null;
        AsyncOperation async;

        private FadeManager() { }

        private static void Init()
        {
            // Canvas作成
            GameObject canvasObject = new GameObject("CanvasFade");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 100;

            // Image作成
            image = new GameObject("ImageFade").AddComponent<Image>();
            image.transform.SetParent(canvas.transform, false);

            // 画面中央をアンカーとし、Imageのサイズをスクリーンサイズに合わせる
            image.rectTransform.anchoredPosition = Vector3.zero;
            image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);

            // Text作成(Now Loading)
            loadingText = new GameObject("LoadingText").AddComponent<Text>();
            loadingText.transform.SetParent(canvas.transform, false);
            // フォント、幅、文字サイズ、文字揃えなどをセット
            loadingText.font = Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf") as Font;
            loadingText.rectTransform.pivot = new Vector2(1, 0);
            loadingText.rectTransform.anchorMax = new Vector2(1, 0);
            loadingText.rectTransform.anchorMin = new Vector2(1, 0);
            loadingText.rectTransform.sizeDelta = new Vector2(500, 200);    // 今回はとりあえず直指定
            loadingText.fontSize = 60;
            loadingText.alignment = TextAnchor.MiddleLeft;
            loadingText.color = Color.white;
            loadingText.enabled = false;    // フェード終わるまで非表示

            // 遷移先シーンでもオブジェクトを破棄しない
            DontDestroyOnLoad(canvas.gameObject);

            // シングルトンオブジェクトを保持
            canvasObject.AddComponent<FadeManager>();
            instance = canvasObject.GetComponent<FadeManager>();
        }

        // フェード付きシーン遷移を行う
        public void LoadScene(string sceneName, float interval = 1f)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            fadeCoroutine = null;

            fadeCoroutine = Fade(sceneName, interval);
            StartCoroutine(fadeCoroutine);
        }
        private IEnumerator Fade(string sceneName, float interval)
        {
            float time = 0f;
            canvas.enabled = true;

            // フェードアウト
            while (time <= interval)
            {
                float fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
                image.color = new Color(0.0f, 0f, 0f, fadeAlpha);
                time += Time.deltaTime;
                yield return null;
            }

            // シーンの読み込みを開始
            async = SceneManager.LoadSceneAsync(sceneName);
            // ロードが完了してもすぐシーンのアクティブ化をしない
            async.allowSceneActivation = false;

            time = 0f;
            loadingText.enabled = true;
            while (async.progress < 0.9f)
            {
                time += Time.deltaTime;
                // 0.3秒ごとに表示切り替え
                if (time < 0.3f) { loadingText.text = "Now Loading"; }
                else if (time < 0.6f) { loadingText.text = "Now Loading."; }
                else if (time < 0.9f) { loadingText.text = "Now Loading.."; }
                else if (time < 1.2f) { loadingText.text = "Now Loading..."; }
                else { time = 0f; }
                yield return null;
            }
            // ロード完了後、0.5秒待ってからシーン遷移
            yield return new WaitForSeconds(0.5f);
            loadingText.enabled = false;
            async.allowSceneActivation = true;

            // フェードイン
            time = 0f;
            while (time <= interval)
            {
                float fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
                image.color = new Color(0f, 0f, 0f, fadeAlpha);
                time += Time.deltaTime;
                yield return null;
            }

            // 描画を更新しない
            canvas.enabled = false;
        }
    }
}

