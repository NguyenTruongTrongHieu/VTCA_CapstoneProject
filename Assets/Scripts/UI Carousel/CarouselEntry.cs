using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "New Carousel Entry", menuName = "UI/Carousel Entry", order = 0)]
public class CarouselEntry : ScriptableObject
{
    [field: SerializeField] public GameObject characterPanel { get; private set; }
    [field: SerializeField] public Sprite EntryGraphic { get; private set; }
    [field: SerializeField] public string ClassText { get; private set; }
}
