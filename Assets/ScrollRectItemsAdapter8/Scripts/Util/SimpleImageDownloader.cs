using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace frame8.ScrollRectItemsAdapter.Util
{
    public class SimpleImageDownloader : MonoBehaviour
    {
        public static SimpleImageDownloader Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new GameObject(typeof(SimpleImageDownloader).Name).AddComponent<SimpleImageDownloader>();

                return _Instance;
            }
        }

        static SimpleImageDownloader _Instance;


		public void Download(string path, Action<Texture2D> onDone, Action onError, Action<float> onProgress = null)
        {
            StartCoroutine(DownloadCoroutine(path, onProgress, onDone, onError));
        }

        IEnumerator DownloadCoroutine(string path, Action<float> onProgress, Action<Texture2D> onDone, Action onError)
        {
            var www = new WWW(path);

            // yield return www;
            while (!www.isDone)
            {
                if (onProgress != null)
                    onProgress(www.progress);

                yield return null;
            }

            if (string.IsNullOrEmpty(www.error))
            {
                if (onProgress != null)
                    onProgress(1f);

                if (onDone != null)
                    onDone(www.texture);
            }
            else
            {
                if (onError != null)
                    onError();
            }
        }
    }
}