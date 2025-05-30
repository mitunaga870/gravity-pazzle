using System;
using System.Collections;
using System.Collections.Generic;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using UnityEngine;

namespace ModularBuildingsFramework.Demo
{
	public class Demo : MonoBehaviour
	{
        /// ======================================================================

        [SerializeField] private List<Object> builders;

        private int _builderIndex;
        private float _draftLength;
        private float _draftHeight;
        private float _draftDepth;
        private bool _isRunDecorators;

        private Camera sceneCamera;
        private Transform target;
        private bool isDraftDirty;
                
        /// ======================================================================

        public int builderIndex
        {
            get { return _builderIndex; }
            set
            {
                _builderIndex = value;
                isDraftDirty = true;
            }
        }
        public float draftLength
        {
            get { return _draftLength; }
            set
            {
                _draftLength = value;
                isDraftDirty = true;
            }
        }
        public float draftHeight
        {
            get { return _draftHeight; }
            set
            {
                _draftHeight = value;
                isDraftDirty = true;
            }
        }
        public float draftDepth
        {
            get { return _draftDepth; }
            set
            {
                _draftDepth = value;
                isDraftDirty = true;
            }
        }
        public bool isRunDecorators
        {
            get { return _isRunDecorators; }
            set
            {
                _isRunDecorators = value;
                isDraftDirty = true;
            }
        }

        /// ======================================================================

        private void Awake()
        {
            sceneCamera = GameObject.Find("SCENE/MainCamera").GetComponent<Camera>();
            target = new GameObject("TARGET").transform;

            draftLength = 20f;
            draftHeight = 20f;
            draftDepth = 20f;

            isRunDecorators = true;
            sceneCamera.enabled = false;
        }
        private IEnumerator Start()
        {
            StartCoroutine(DoRebuild());

            yield return new WaitForSeconds(1f);
            sceneCamera.enabled = true;
        }

        public void RequestRebuild()
        {
            isDraftDirty = true;
        }
        private IEnumerator DoRebuild()
        {
            while (true)
            {
                yield return new WaitForEndOfFrame();

                if (isDraftDirty)
                {
                    Utils.DeleteChilds(target);
                    UpdateTargetPosition();

                    var draft = CreateDraft();
                    var builder = GetBuilder();
                    builder.Build(draft);

                    if (isRunDecorators)
                        Utils.RunDecorators(target);

                    isDraftDirty = false;
                }
            }
        }

        private void UpdateTargetPosition()
        {
            target.position = Vector3.zero
                - target.right * draftLength * 0.5f
                - target.forward * draftDepth * 0.5f;
        }
        private BoxDraft CreateDraft()
        {
            var result = BoxDraft.Create();
            result.Parse(target);

            result.length = draftLength;
            result.height = draftHeight;
            result.depth = draftDepth;

            return result;
        }

        public List<Object> GetBuilders()
        {
            return builders;
        }
        private IBoxBuilder GetBuilder()
        {
            return builders[builderIndex] as IBoxBuilder;
        }

        /// ======================================================================

        private Demo()
        {
            builders = new List<Object>();
        }

        /// ======================================================================
    }
}