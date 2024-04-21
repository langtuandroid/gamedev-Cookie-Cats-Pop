using System;
using System.Collections;
using UnityEngine;

namespace TactileModules.UrlCaching.Support
{
    public class WWW : IWWW
    {
        public WWW(string url)
        {
            this.www = new UnityEngine.WWW(url);
        }

        public string Error
        {
            get
            {
                return this.www.error;
            }
        }

        public byte[] Bytes
        {
            get
            {
                return this.www.bytes;
            }
        }

        public IEnumerator WaitForCompletion()
        {
            while (!this.www.isDone)
            {
                yield return null;
            }
            yield break;
        }

        private UnityEngine.WWW www;
    }
}
