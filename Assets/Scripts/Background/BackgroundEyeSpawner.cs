using UnityEngine;

[ExecuteInEditMode]
public class BackgroundEyeSpawner : MonoBehaviour
{
    public BackgroundEye backgroundEyePrefab;
    public Transform eyeContainer;
    public bool spawnEyes;

    private float xMovement = 74.8f;
    private float yMovement = 50f;
    private float xInitialPoint = -523.6f;
    private float yInitialPoint = -350f;
    private int maxSpawn = 15;

    void Update()
    {
        if (Application.isEditor)
            SpawnEyes();
    }

    void SpawnEyes()
    {
        if (spawnEyes)
        {
            if (eyeContainer.childCount > 0)
            {
                for (int i = 0; i < eyeContainer.childCount; i++)
                    DestroyImmediate(eyeContainer.GetChild(i).gameObject);
            }

            var xCurrentPos = xInitialPoint;
            var yCurrentPos = yInitialPoint;

            for (int x = 0; x < maxSpawn; x++)
            {
                var newEyeX = Instantiate(backgroundEyePrefab.gameObject);
                if (x > 0)
                    xCurrentPos += xMovement;
                newEyeX.name = "New Eye X:[" + x + "] - Y:[0]";
                newEyeX.transform.position = new Vector3(xCurrentPos, yCurrentPos, backgroundEyePrefab.transform.position.z);
                newEyeX.transform.SetParent(eyeContainer);

                for (int y = 0; y < maxSpawn - 1; y++)
                {
                    var newEyeY = Instantiate(backgroundEyePrefab.gameObject);
                    yCurrentPos += yMovement;
                    var yModified = y + 1;
                    newEyeY.name = "New Eye X:[" + x + "] - Y:[" + yModified + "]";
                    newEyeY.transform.position = new Vector3(xCurrentPos, yCurrentPos, backgroundEyePrefab.transform.position.z);
                    newEyeY.transform.SetParent(eyeContainer);

                    if (y == maxSpawn - 2)
                        yCurrentPos = yInitialPoint;
                }
            }

            spawnEyes = false;
        }
    }
}