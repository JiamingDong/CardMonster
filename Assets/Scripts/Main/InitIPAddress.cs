using UnityEngine;
using UnityEngine.UI;

public class InitIPAddress : MonoBehaviour
{
    public InputField allyPortInputField;
    public InputField enemyIPInputField;
    public InputField enemyPortInputField;

    void Start()
    {
        allyPortInputField.text = Database.cardMonster.Query("ConfigParameter", "and itemname='defalutAllyPort'")[0]["itemvalue"];
        enemyIPInputField.text = Database.cardMonster.Query("ConfigParameter", "and itemname='defalutEnemyIP'")[0]["itemvalue"];
        enemyPortInputField.text = Database.cardMonster.Query("ConfigParameter", "and itemname='defalutEnemyPort'")[0]["itemvalue"];
    }
}
