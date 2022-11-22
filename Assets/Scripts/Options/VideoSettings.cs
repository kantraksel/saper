using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace TheGame.Options
{
    public class VideoSettings : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        private Resolution[] resolutions;

        private void OnValidate()
        {
            Debug.Assert(dropdown != null, "[VideoSettings] Dropdown is null");
        }

        private void Start()
        {
            GetResolutions();
            SetCurrentResolution();
        }

        private void GetResolutions()
        {
            var resolutions = Screen.resolutions;
            var list = new List<Resolution>();
            var nameList = new List<string>();
            foreach (var resolution in resolutions)
            {
                if (resolution.refreshRate != 60)
                    continue;

                list.Add(resolution);
                nameList.Add($"{resolution.width}x{resolution.height}");
            }
            list.Reverse();
            nameList.Reverse();

            this.resolutions = list.ToArray();
            dropdown.AddOptions(nameList);
        }

        private void SetCurrentResolution()
        {
            var currentRes = new Resolution() { width = Screen.width, height = Screen.height };
            for (int i = 0; i < resolutions.Length; ++i)
            {
                var resolution = resolutions[i];
                if (resolution.width == currentRes.width)
                    if (resolution.height == currentRes.height)
                    {
                        dropdown.SetValueWithoutNotify(i);
                        break;
                    }
            }
        }

        public void OnResolutionChanged(int index)
        {
            var resolution = resolutions[index];
            Screen.SetResolution(resolution.width, resolution.height, false);
        }

        public void OnOptionRefresh()
        {
            SetCurrentResolution();
        }
    }
}
