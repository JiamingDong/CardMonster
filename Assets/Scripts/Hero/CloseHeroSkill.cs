using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �ر�ѡӢ�ۼ��ܵĽ���
/// </summary>
public class CloseHeroSkill : MonoBehaviour
{
    public void OnClick()
    {
        GameObject selectHeroSkillPrefabInstantiation = GameObject.Find("SelectHeroSkillPrefabInstantiation");
        Destroy(selectHeroSkillPrefabInstantiation);
    }
}
