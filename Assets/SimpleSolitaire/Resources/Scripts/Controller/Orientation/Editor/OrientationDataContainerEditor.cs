using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SimpleSolitaire.Controller.Editor
{
    public class OrientationDataContainerEditor
    {
        [MenuItem("CONTEXT/OrientationDataContainer/AddAnchors")]
        static void TestMenu(MenuCommand command)
        {
            OrientationDataContainer body = (OrientationDataContainer)command.context;
            if (body == null)
            {
                return;
            }

            var selections = Selection.transforms.Select(x => x as RectTransform).ToList();

            var last = body.Orientations.Last();

            foreach (var t in selections)
            {
                var name = t.name.Replace("_Data", "");
                var key = Enum.TryParse(name, out OrientationElementKey result) ? result : OrientationElementKey.Unknown;

                last.Elements.Add(new OrientationElement
                {
                    Key = key,
                    Anchor = new OrientationAnchor()
                    {
                        Min = t.anchorMin,
                        Max = t.anchorMax
                    },
                    Pivot = new OrientationPivot() { Value = t.pivot },
                    Position = new OrientationPosition() { Value = t.anchoredPosition },
                    Size = new OrientationSize() { Value = t.sizeDelta },
                });
            }
        }
    }
}