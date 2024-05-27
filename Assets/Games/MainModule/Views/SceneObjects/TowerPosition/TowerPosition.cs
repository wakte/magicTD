using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XFGameFramework;

public class TowerPosition : MonoBehaviour
{

    private Module module;

    public TowerBase Tower { get; set; }

    UpgradeTowerPanel panel;
    public void InitModule(Module module)
    {
        this.module = module;   
    }
    public void OnClick()
    {
        // TODO

        if (Tower == null || !Tower.gameObject.activeSelf)
        {
            module.LoadPanel<CreateTowerPanel>(UIType.UI, null, this);
        }
        else
        {
            Tower.ShowAttackRange();
            // 打开升级炮塔界面   
            panel = module.LoadPanel<UpgradeTowerPanel>(UIType.UI, null, this);
            panel.onDisable += OnUpgradeDisable;
        }

    }

    public void OnUpgradeDisable()
    {
        panel.onDisable -= OnUpgradeDisable;
        if (Tower != null)
        {
            Tower.HideAttackRange();
        }
            
    }
}
