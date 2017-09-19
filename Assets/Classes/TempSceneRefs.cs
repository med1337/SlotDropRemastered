using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct TempSceneRefs
{
    public StatTracker stat_tracker
    {
        get
        {
            if (stat_tracker_ == null)
                stat_tracker_ = GameObject.FindObjectOfType<StatTracker>();

            return stat_tracker_;
        }
    }

    public RespawnManager respawn_manager
    {
        get
        {
            if (respawn_manager_ == null)
                respawn_manager_ = GameObject.FindObjectOfType<RespawnManager>();

            return respawn_manager_;
        }
    }

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

    public FocusCameraManager focus_camera
    {
        get
        {
            if (focus_camera_ == null)
                focus_camera_ = GameObject.FindObjectOfType<FocusCameraManager>();

            return focus_camera_;
        }
    }

    public GeneralCanvasManager general_canvas_manager
    {
        get
        {
            if (general_canvas_manager_ == null)
                general_canvas_manager_ = GameObject.FindObjectOfType<GeneralCanvasManager>();

            return general_canvas_manager_;
        }
    }

    private StatTracker stat_tracker_;
    private RespawnManager respawn_manager_;
    private SlotManager slot_manager_;
    private PcManager pc_manager_;
    private MeteorManager meteor_manager_;
    private FocusCameraManager focus_camera_;
    private GeneralCanvasManager general_canvas_manager_;

}
