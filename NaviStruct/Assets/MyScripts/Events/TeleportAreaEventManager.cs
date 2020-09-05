using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportAreaEventManager : TeleportationArea
{
    public GameObject m_Reticle;

    protected override void Awake()
    {
        base.Awake();          

        if (this.customReticle != null)
            m_Reticle = this.customReticle;
    }

    private void Start()
    {
        this.customReticle = null;
    }
     
    protected override void OnSelectEnter(XRBaseInteractor interactor)
    {
        if (this.customReticle == null)
            this.customReticle = m_Reticle;

        if (m_Reticle != null)
            this.AttachCustomReticle(interactor);

        base.OnSelectEnter(interactor);
    }

    protected override void OnSelectExit(XRBaseInteractor interactor)
    {
        if (m_Reticle != null)
            this.RemoveCustomReticle(interactor);

        this.customReticle = null;

        base.OnSelectExit(interactor);
    }

}
