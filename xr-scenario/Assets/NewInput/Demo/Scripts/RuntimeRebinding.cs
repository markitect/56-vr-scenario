using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Experimental.Input;

public class RuntimeRebinding : MonoBehaviour
{
    // Scope is action map.
    ActionMapInput m_ActionMapInput;
    PlayerHandle m_PlayerHandle;
    string[] m_ControlSchemeNames;
    Vector2 m_Scroll;

    // Scope is control scheme.
    int m_ControlSchemeIndex;
    List<LabeledBinding> m_Bindings;
    IEndBinding m_BindingToBeAssigned;

    const int k_ReferenceScreenWidth = 800;
    const int k_TotalWidth = 450;
    const int k_BindingWidth = 200;
    const int k_BottomButtonWidth = 100;

    static class Styles
    {
        public static GUIStyle leftAlignedButton;

        static Styles()
        {
            leftAlignedButton = new GUIStyle("button");
            leftAlignedButton.alignment = TextAnchor.MiddleLeft;
        }
    }

    public void Initialize(ActionMapInput actionMapInput, PlayerHandle playerHandle)
    {
        m_ActionMapInput = actionMapInput;
        m_PlayerHandle = playerHandle;
        if (m_ActionMapInput == null || m_PlayerHandle == null)
            return;

        m_ControlSchemeNames = new string[m_ActionMapInput.controlSchemes.Count];
        for (int i = 0; i < m_ControlSchemeNames.Length; i++)
            m_ControlSchemeNames[i] = m_ActionMapInput.controlSchemes[i].name;
        InitializeControlScheme();
    }

    void InitializeControlScheme()
    {
        var devices = m_PlayerHandle.GetApplicableDevices();
        m_ActionMapInput.TryInitializeWithDevices(devices, null, m_ControlSchemeIndex);
        m_Bindings = new List<LabeledBinding>();
        m_ActionMapInput.controlSchemes[m_ControlSchemeIndex].ExtractLabeledEndBindings(m_Bindings);
    }

    void OnGUI()
    {
        float scale = Mathf.Max(1, Screen.width / (float)k_ReferenceScreenWidth);
        Matrix4x4 matrix = Matrix4x4.Scale(Vector3.one * scale);
        GUI.matrix = matrix;

        GUILayout.BeginArea(new Rect(10, 10, k_TotalWidth, Screen.height / scale - 20));
        InnerGUI();
        GUILayout.EndArea();
    }

    void InnerGUI()
    {
        if (m_ActionMapInput == null)
        {
            GUILayout.Label("No action map input assigned.");
            return;
        }

        int newScheme = GUILayout.Toolbar(m_ControlSchemeIndex, m_ControlSchemeNames, GUILayout.ExpandWidth(false));
        if (newScheme != m_ControlSchemeIndex)
        {
            m_ControlSchemeIndex = newScheme;
            InitializeControlScheme();
        }

        GUILayout.Space(10);

        if (m_ControlSchemeIndex < 0 || m_ControlSchemeIndex >= m_ActionMapInput.controlSchemes.Count)
            return;

        ControlScheme scheme = m_ActionMapInput.controlSchemes[m_ControlSchemeIndex];

        if (scheme.bindings.Count != m_ActionMapInput.actionMap.actions.Count)
        {
            GUILayout.Label("Control scheme bindings don't match action map actions.");
            return;
        }

        m_Scroll = GUILayout.BeginScrollView(m_Scroll, "Box", GUILayout.ExpandWidth(false));
        for (int binding = 0; binding < m_Bindings.Count; binding++)
        {
            DisplaySource(m_Bindings[binding], scheme);
        }
        GUILayout.EndScrollView();

        GUILayout.Space(10);

        GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Reset", GUILayout.Width(k_BottomButtonWidth), GUILayout.ExpandWidth(false)))
        {
            m_ActionMapInput.ResetControlSchemes();
            Initialize(m_ActionMapInput, m_PlayerHandle);
        }
        if (GUILayout.Button("Done", GUILayout.Width(k_BottomButtonWidth), GUILayout.ExpandWidth(false)))
        {
            enabled = false;
        }
        GUILayout.EndHorizontal();
    }

    void DisplaySource(LabeledBinding labeledBinding, ControlScheme scheme)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label(labeledBinding.label);

            if (labeledBinding.binding == m_BindingToBeAssigned)
            {
                GUILayout.Button("...", GUILayout.Width(k_BindingWidth), GUILayout.ExpandWidth(false));
            }
            else
            {
                string name = labeledBinding.binding.GetSourceName(scheme, false);

                if (GUILayout.Button(name, Styles.leftAlignedButton, GUILayout.Width(k_BindingWidth)))
                {
                    m_BindingToBeAssigned = labeledBinding.binding;
                    InputSystem.ListenForBinding(BindInputControl);
                }
            }
        }
        GUILayout.EndHorizontal();
    }

    bool BindInputControl(InputControl control)
    {
        if (!m_ActionMapInput.BindControl(m_BindingToBeAssigned, control, true))
            return false;

        m_BindingToBeAssigned = null;
        return true;
    }
}
