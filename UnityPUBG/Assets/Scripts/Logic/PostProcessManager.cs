using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.Rendering.PostProcessing;
using UnityPUBG.Scripts.Utilities;

namespace UnityPUBG.Scripts.Logic
{
    public class PostProcessManager : Singleton<PostProcessManager>
    {
        public PostProcessVolume postProcessVolume;

        private void Awake()
        {
            EntityManager.Instance.OnMyPlayerSpawn += OnMyPlayerSpawn;
            postProcessVolume.enabled = false;
        }

        private void OnMyPlayerSpawn(object sender, EventArgs e)
        {
            EntityManager.Instance.MyPlayer.OnLand += MyPlayer_OnLand;
        }

        private void MyPlayer_OnLand(object sender, EventArgs e)
        {
            postProcessVolume.enabled = true;
        }
    }
}
