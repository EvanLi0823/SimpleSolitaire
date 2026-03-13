using SimpleSolitaire.Controller;
using SimpleSolitaire.Model.Enum;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSolitaire
{
    [RequireComponent(typeof(OrientationManager))]
    public class OrientationTester : MonoBehaviour
    {
        public OrientationScreen Screen;
        public HandOrientation Hand;

        public OrientationManager Logic;
        public bool TryUpdateExistingOrientation = true;

        public void SetOrientation()
        {
            if (Logic.OrientationContainer == null || Logic.OrientationContainer.Orientations.Count <= 0)
            {
                Debug.LogError("Container is null or empty");
                return;
            }

            StartCoroutine(Logic.SetSpecificOrientation(Screen, Hand));
        }

        public void SetOrientationInEditor()
        {
#if UNITY_EDITOR

            if (Logic.OrientationContainer == null || Logic.OrientationContainer.Orientations.Count <= 0)
            {
                Debug.LogError("Container is null or empty");
                return;
            }

            Logic.SetSpecificOrientationInEditor(Screen, Hand);
#endif
        }

        public void SaveValuesToContainer()
        {
            if (Logic.OrientationContainer == null)
            {
                Debug.LogError("Container is null");
                return;
            }

            var currentOrientation = Logic.OrientationContainer.Orientations.FirstOrDefault(x => x.Hand == Hand && x.ScrOrientation == Screen);

            if (!TryUpdateExistingOrientation || currentOrientation == null)
            {
                currentOrientation = new Orientation
                {
                    Hand = Hand,
                    ScrOrientation = Screen,
                    AspectMode = Screen == OrientationScreen.Portrait ? AspectRatioFitter.AspectMode.WidthControlsHeight : AspectRatioFitter.AspectMode.HeightControlsWidth
                };

                Logic.OrientationContainer.Orientations.Add(currentOrientation);
            }

            var elements = Logic.DeckElements;
            currentOrientation.Elements.Clear();

            foreach (var element in elements)
            {
                var rect = element.RectRoot;
                var key = Enum.TryParse(element.name, out OrientationElementKey result)
                    ? result
                    : OrientationElementKey.Unknown;

                currentOrientation.Elements.Add(new OrientationElement
                {
                    Key = key,
                    Anchor = new OrientationAnchor()
                    {
                        Min = rect.anchorMin,
                        Max = rect.anchorMax
                    },
                    Pivot = new OrientationPivot() { Value = rect.pivot },
                    Position = new OrientationPosition() { Value = rect.anchoredPosition },
                    Size = new OrientationSize() { Value = rect.sizeDelta },
                });
            }
        }
    }
}