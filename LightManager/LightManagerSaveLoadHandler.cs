﻿using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;
using Studio;
using IllusionUtility.GetUtility;
using System.Collections;
using UnityEngine;

namespace LightManager
{
    class SaveDataHandler : MonoBehaviour
    {
        const string MOD_NAME = "LightManager";
        const string ELEMENT_NAME = "Light";
        const string LIGHT_ID = "LightID";
        const string TARGET_ID = "TargetID";
        const string TARGET_NAME = "TargetName";
        const string ROTATION_SPEED = "RotationSpeed";

        void Start()
        {
            HSExtSave.HSExtSave.RegisterHandler(MOD_NAME, null, null, OnLoad, null, OnSave, null, null);
        }

        void OnDestroy()
        {
            HSExtSave.HSExtSave.UnregisterHandler(MOD_NAME);
        }

        void OnLoad(string path, XmlNode node)
        {
            if(node != null)
                StartCoroutine(OnLoadDelay(node)); 
        }

        IEnumerator OnLoadDelay(XmlNode node)
        {
            if(node.InnerXml.IsNullOrWhiteSpace()) yield break;
            for(int i = 0; i < 3; i++) yield return null;

            var mainElement = XElement.Parse(node.InnerXml);
            if(mainElement != null)
            {
                foreach(var element in mainElement.Elements())
                {
                    if(Studio.Studio.Instance.dicObjectCtrl.TryGetValue((int)element.Element(LIGHT_ID), out ObjectCtrlInfo lightInfo))
                    {
                        if(lightInfo is OCILight)
                        {
                            if(Studio.Studio.Instance.dicObjectCtrl.TryGetValue((int)element.Element(TARGET_ID), out ObjectCtrlInfo targetInfo))
                            {
                                var ocilight = lightInfo as OCILight;
                                var light = (lightInfo as OCILight).light;
                                var tracker = light.gameObject.AddComponent<TrackTransform>();

                                var chara = (targetInfo as OCIChar).charInfo;
                                string prefix = chara is CharFemale ? "cf" : "cm";
                                tracker.target = chara.chaBody.transform.FindLoop(prefix + "_J_Mune00").transform;
                                tracker.targetKey = targetInfo.objectInfo.dicKey;
                                tracker.rotationSpeed = (float?)element.Element(ROTATION_SPEED) ?? 1f;
                                tracker.targetName = (string)element.Element(TARGET_NAME) ?? "Blank";
                            }
                        }
                    }
                }

                Console.WriteLine("Load LightManager savedata from XML");
            }
        }

        void OnSave(string path, XmlTextWriter xmlWriter)
        {
            var objectList = new List<XElement>();
            foreach(ObjectCtrlInfo objectCtrlInfo in Studio.Studio.Instance.dicObjectCtrl.Values)
            {
                if(objectCtrlInfo is OCILight)
                {
                    var light = (objectCtrlInfo as OCILight).light;
                    var tracker = light.gameObject.GetComponent<TrackTransform>();
                    if(tracker)
                    {
                        if(Studio.Studio.Instance.dicObjectCtrl.TryGetValue(tracker.targetKey, out ObjectCtrlInfo info))
                        {
                            var element = new XElement(ELEMENT_NAME, new object[]
                            {
                                new XElement(LIGHT_ID, objectCtrlInfo.objectInfo.dicKey),
                                new XElement(TARGET_ID, tracker.targetKey),
                                new XElement(ROTATION_SPEED, tracker.rotationSpeed),
                                new XElement(TARGET_NAME, tracker.targetName),
                            });

                            objectList.Add(element);
                        }
                    }
                }
            }

            if(objectList.Count > 0)
            {
                xmlWriter.WriteStartElement(MOD_NAME);
                objectList.ForEach(x => x.WriteTo(xmlWriter));
                xmlWriter.WriteEndElement();
            }
        }
    }
}
