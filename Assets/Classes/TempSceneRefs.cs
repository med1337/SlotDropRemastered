using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public SlotManager slot_manager
    {
        get
        {
            if (slot_manager_ == null)
                slot_manager_ = GameObject.FindObjectOfType<SlotManager>();

            return slot_manager_;
        }
    }

    public PcManager pc_manager
    {
        get
        {
            if (pc_manager_ == null)
                pc_manager_ = GameObject.FindObjectOfType<PcManager>();

            return pc_manager_;
        }
    }


    public MeteorManager meteor_manager
    {
        get
        {
            if (meteor_manager_ == null)
                meteor_manager_ = GameObject.FindObjectOfType<MeteorManager>();

            return meteor_manager_;
        }
    }


    private SlotManager slot_manager_;
    private PcManager pc_manager_;
    private MeteorManager meteor_manager_;

}
