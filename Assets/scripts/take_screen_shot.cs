using UnityEngine;
using System.Collections;

public class take_screen_shot : MonoBehaviour
{
    public Camera _camera;
    public int resWidth = 2550;
    public int resHeight = 3300;
    public Texture2D screenShot;
    public RenderTexture rt_test;
    public SpriteRenderer ground;
    public Texture2D screenShot_tex;

    void Start()
    {
    }


    private bool takeHiResShot = false;
    public static string ScreenShotName(int width, int height)
    {
        return string.Format("{0}/screenshots/screen_{1}x{2}_{3}.png",
                             Application.dataPath,
                             width, height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
    }
    public void TakeHiResShot()
    {
        takeHiResShot = true;
    }
    void LateUpdate()
    {
        if (Input.GetKeyDown("k"))
        {
            Debug.Log("1");

            float targetaspect = 2.0f / 1.0f;

            // determine the game window's current aspect ratio
            float windowaspect = (float)Screen.width / (float)Screen.height;

            /*
            Debug.Log(Screen.width);
            Debug.Log(Screen.height);
            Debug.Log(windowaspect);

            float scaleheight = windowaspect / targetaspect;

            Debug.Log(scaleheight);

            if (scaleheight < 1.0f)
            {
                Rect rect = _camera.rect;

                rect.width = 1.0f;
                rect.height = scaleheight;
                rect.x = 0;
                rect.y = (1.0f - scaleheight) / 2.0f;

                _camera.rect = rect;
            }
            else // add pillarbox
            {
                float scalewidth = 1.0f / scaleheight;

                Rect rect = _camera.rect;

                rect.width = scalewidth;
                rect.height = 1.0f;
                rect.x = (1.0f - scalewidth) / 2.0f;
                rect.y = 0;

                _camera.rect = rect;
            } 
            */
            /*
            _camera.aspect = windowaspect;
            Rect r = _camera.pixelRect;
            print("Camera displays from " + r.xMin + " to " + r.xMax + " pixel");
            print("Camera displays from " + r.yMin + " to " + r.yMax + " pixel");
            */
            //RenderTexture rt = new RenderTexture(resWidth, resHeight, 24);
            //_camera.targetTexture = rt;
            screenShot_tex = new Texture2D(resWidth, resHeight, TextureFormat.ARGB32, false);
            RenderTexture.active = rt_test;
            _camera.Render();
            //RenderTexture.active = rt;
            Rect rect = new Rect(0, 0, resWidth, resHeight);
            screenShot_tex.ReadPixels(rect, 0, 0);

            screenShot_tex.Apply();
            Debug.Log(ground.sprite.textureRect);
            Debug.Log(ground.sprite.pivot);
            ground.sprite = Sprite.Create(screenShot_tex, rect, new Vector2(0.5f, 0.5f));
            //ground.sharedMaterial.mainTexture = screenShot_tex;

            //_camera.targetTexture = null;
            //RenderTexture.active = null; // JC: added to avoid errors
            //Destroy(rt);
            
            byte[] bytes = screenShot_tex.EncodeToPNG();
            string filename = ScreenShotName(resWidth, resHeight);
            System.IO.File.WriteAllBytes(filename, bytes);
            Debug.Log(string.Format("Took screenshot to: {0}", filename));
            takeHiResShot = false;
            Debug.Log("2");
            
        }
    }

}