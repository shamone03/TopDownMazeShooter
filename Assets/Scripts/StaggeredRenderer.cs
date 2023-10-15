using System.Collections;
using UnityEngine;

public class StaggeredRenderer : MonoBehaviour {
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    private float _horizontal;
    private float _vertical;
    [SerializeField] private Transform wall;
    [SerializeField] private float wallGap;

    private bool done;
    IEnumerator Render() {
        foreach (StaggeredGenerator.NodeState[,] maze in StaggeredGenerator.Generate(width, height)) {
            done = false;
            foreach (Transform t in transform) {
                if (t.name != "current") {
                    Destroy(t.gameObject);
                }
            }

            for (int i = 0; i < width; i++) {
                for (int j = 0; j < height; j++) {
                    StaggeredGenerator.NodeState node = maze[i, j];

                    string name = i.ToString() + j.ToString();
                    _horizontal = (wallGap + wall.localScale.x) * Mathf.Cos(30 * Mathf.Deg2Rad);
                    _vertical = (wallGap + wall.localScale.x) + ((wallGap + wall.localScale.x) * Mathf.Sin(30 * Mathf.Deg2Rad));

                    Vector3 pos = new Vector3((i - width / 2) * _horizontal * 2, transform.position.y,
                        (j - height / 2) * _vertical); // center of node

                    if (j % 2 == 0) {
                        pos -= new Vector3(_horizontal, 0, 0);
                    }

                    if (node.HasFlag(StaggeredGenerator.NodeState.UpLeft)) {
                        Transform upLeft = Instantiate(wall, transform);
                    
                        upLeft.eulerAngles = new Vector3(upLeft.transform.rotation.eulerAngles.x, -30, 0);
                        upLeft.position = pos + new Vector3(-_horizontal / 2, 0, _vertical / 2);
                        upLeft.name = "upLeft" + name;
                    
                    }
                    // //
                    if (node.HasFlag(StaggeredGenerator.NodeState.Left)) {
                        Transform left = Instantiate(wall, transform);
                    
                        left.eulerAngles = new Vector3(left.transform.rotation.eulerAngles.x, 90, 0);
                        left.position = pos + new Vector3(-_horizontal, 0, 0);
                        left.name = "left" + name;
                    }
                    // //
                    if (node.HasFlag(StaggeredGenerator.NodeState.DownLeft)) {
                        Transform downLeft = Instantiate(wall, transform);
                    
                        downLeft.eulerAngles = new Vector3(downLeft.transform.rotation.eulerAngles.x, 30, 0);
                        downLeft.position = pos + new Vector3(-_horizontal / 2, 0, -_vertical / 2);
                        downLeft.name = "downLeft" + name;
                    }


                    if (j == 0) {
                        if (node.HasFlag(StaggeredGenerator.NodeState.DownRight)) {
                            Transform downRight = Instantiate(wall, transform);
                        
                            downRight.eulerAngles = new Vector3(downRight.transform.rotation.eulerAngles.x, -30, 0);
                            downRight.position = pos + new Vector3(_horizontal / 2, 0, -_vertical / 2);
                            downRight.name = "downRight" + name;
                        }
                    }

                    if (i == width - 1) {
                        if (node.HasFlag(StaggeredGenerator.NodeState.Right)) {
                            Transform left = Instantiate(wall, transform);
                        

                            left.eulerAngles = new Vector3(left.transform.rotation.eulerAngles.x, 90, 0);
                            left.position = pos + new Vector3(_horizontal, 0, 0);
                            left.name = "right" + name;
                        }

                        if (j != height - 1 && i == width - 1) {
                            if (node.HasFlag(StaggeredGenerator.NodeState.UpRight)) {
                                Transform upRight = Instantiate(wall, transform);
                            

                                upRight.eulerAngles = new Vector3(upRight.transform.rotation.eulerAngles.x, 30, 0);
                                upRight.position = pos + new Vector3(_horizontal / 2, 0, _vertical / 2);
                                upRight.name = "upRight" + name;
                            }
                                            
                            if (node.HasFlag(StaggeredGenerator.NodeState.DownRight)) {
                                Transform downRight = Instantiate(wall, transform);
                            

                                downRight.eulerAngles = new Vector3(downRight.transform.rotation.eulerAngles.x, -30, 0);
                                downRight.position = pos + new Vector3(_horizontal / 2, 0, -_vertical / 2);
                                downRight.name = "downRight" + name;
                            }
                        }
                    }

                

                    if (j == height - 1) {
                        if (node.HasFlag(StaggeredGenerator.NodeState.UpRight)) {
                            Transform upRight = Instantiate(wall, transform);
                        

                            upRight.eulerAngles = new Vector3(upRight.transform.rotation.eulerAngles.x, 30, 0);
                            upRight.position = pos + new Vector3(_horizontal / 2, 0, _vertical / 2);
                            upRight.name = "upRight" + name;
                        }

                        if (i == width - 1) {
                            if (node.HasFlag(StaggeredGenerator.NodeState.DownRight)) {
                                Transform downRight = Instantiate(wall, transform);
                            

                                downRight.eulerAngles = new Vector3(downRight.transform.rotation.eulerAngles.x, -30, 0);
                                downRight.position = pos + new Vector3(_horizontal / 2, 0, -_vertical / 2);
                                downRight.name = "downRight" + name;
                            } 
                        }
                    }
                }
            }
            yield return new WaitForSeconds(.01f);
        }
        done = true;
    }

    private void Start() {
        foreach (Transform t in transform) {
            Destroy(t.gameObject);
        }
        StartCoroutine(Render());
    }

    private void Update() {
        if (done) {
            StopCoroutine(Render());
            foreach (Transform t in transform) {
                Destroy(t.gameObject);
            }
            StartCoroutine(Render());    
        }
    }
}