using UnityEngine;
using Assets.SimpleLocalization;
using Assets.SimpleLocalization.Scripts;

public class MultiLanguage : MonoBehaviour
{
    public static MultiLanguage Instance;

    private void Awake()
    {
        LocalizationManager.Read();
       
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                LocalizationManager.Language = "English";
                break;
            case SystemLanguage.Vietnamese:
                LocalizationManager.Language = "Vietnamese";
                break;
            default:
                LocalizationManager.Language = "English";
                break;
        }
    }

    public void Language(string language)
    {
        LocalizationManager.Language = language;
    }
}
