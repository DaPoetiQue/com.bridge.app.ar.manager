using UnityEngine;
using Bridge.Core.Debug;
using UnityEngine.XR.ARFoundation;
using System.Collections.Generic;

namespace Bridge.Core.App.AR.Manager
{
    [RequireComponent(typeof(ARPlaneMeshVisualizer), typeof(ARPlane), typeof(MeshRenderer))]

    public class ARFeatheredPlaneHandler : MonoDebug
    {
        #region Components

        [SerializeField]
        private float featheringWidth = 0.2f;

        private static List<Vector3> featheringUVs = new List<Vector3>();
        private static List<Vector3> vertices = new List<Vector3>();

        private ARPlaneMeshVisualizer planeVisualizer;
        private ARPlane arPlane;
        private Material planeMaterial;

        public float FeatheringWidth
        {
            get { return featheringWidth; }
            set { featheringWidth = value; }
        }

        #endregion

        #region Unity

        private void Awake() => Init();

        private void OnEnable() => Subscriptions(true);

        private void OnDisable() => Subscriptions(false);

        private void OnDestroy() => Subscriptions(false);

        #endregion

        #region Initialization

        private void Init()
        {
            planeVisualizer = this.GetComponent<ARPlaneMeshVisualizer>();
            arPlane = this.GetComponent<ARPlane>();
            planeMaterial = this.GetComponent<MeshRenderer>().material;
        }

        private void Subscriptions(bool subscribe)
        {
            if(subscribe)
            {

                arPlane.boundaryChanged += ARPlaneBoundryUpdate;
            }
            else
            {
                arPlane.boundaryChanged -= ARPlaneBoundryUpdate;
            }
        }

        #endregion

        #region Main

        private void ARPlaneBoundryUpdate(ARPlaneBoundaryChangedEventArgs boundaryChangedEventArgs)
        {
            GenerateBoundryUVs(planeVisualizer.mesh);
        }

        private void GenerateBoundryUVs(Mesh mesh)
        {
            int vertCount = mesh.vertexCount;

            featheringUVs.Clear();

            if(featheringUVs.Capacity < vertCount)
            {
                featheringUVs.Capacity = vertCount;
            }

            mesh.GetVertices(vertices);

            Vector3 centerInPlaneSpace = vertices[vertices.Count - 1];
            Vector3 uv = new Vector3(0, 0, 0);

            float shortestUVMapping = float.MaxValue;

            for (int i = 0; i < vertCount - 1; i++)
            {
                float vertexDist = Vector3.Distance(vertices[i], centerInPlaneSpace);
                float uvMapping = vertexDist / Mathf.Max(vertexDist - featheringWidth, 0.001f);

                uv.x = uvMapping;

                if(shortestUVMapping > uvMapping)
                {
                    shortestUVMapping = uvMapping;
                }

                featheringUVs.Add(uv);
            }

            planeMaterial.SetFloat("_ShortestUVMapping", shortestUVMapping);
            uv.Set(0, 0, 0);
            featheringUVs.Add(uv);

            mesh.SetUVs(1, featheringUVs);
            mesh.UploadMeshData(false);
        }

        #endregion

    }
}
