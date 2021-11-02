// GENERATED AUTOMATICALLY FROM 'Assets/Input/InputMaster.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @InputMaster : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @InputMaster()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""InputMaster"",
    ""maps"": [
        {
            ""name"": ""Jeff"",
            ""id"": ""167bd151-bc8a-4df7-8b86-161994a9eae9"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""Button"",
                    ""id"": ""1f863611-4e38-4a34-a0e0-519c9af14d43"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Rotation"",
                    ""type"": ""Button"",
                    ""id"": ""485f3c04-1cce-4265-b361-bf55d9671e4e"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""022b57db-5e93-46ed-bb09-de0bcd4880c9"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""a8c9f44c-ca00-442b-b2ab-a2ae32c5e04c"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""43f964d3-0d00-4670-8d34-6415e380b252"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""8eb9eabe-ed33-42af-9b25-3f268ac37636"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""e77c3c75-6233-4dd8-9fd9-c1249ef0b233"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""eddb058f-debc-4876-9f7d-c42013bc556f"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Keyboard"",
                    ""id"": ""eb42c27b-3343-41a7-bd2f-7d830de7fd7f"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""b765a728-2f21-4701-bacd-aeb42bc389fd"",
                    ""path"": ""<Keyboard>/leftArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""31846176-cb44-42bc-a46e-0e5f635221a6"",
                    ""path"": ""<Keyboard>/rightArrow"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""Mouse"",
                    ""id"": ""b8f2e807-1034-4bb7-ab93-a9907f872f69"",
                    ""path"": ""1DAxis"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""negative"",
                    ""id"": ""a54020f1-47ca-4efe-a4b1-755092c08914"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""positive"",
                    ""id"": ""4eff8fb3-1409-462b-a20d-e31d3eacbf95"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Rotation"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""300d1758-df59-442a-bd9a-e045ffcedcdb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // Jeff
        m_Jeff = asset.FindActionMap("Jeff", throwIfNotFound: true);
        m_Jeff_Movement = m_Jeff.FindAction("Movement", throwIfNotFound: true);
        m_Jeff_Rotation = m_Jeff.FindAction("Rotation", throwIfNotFound: true);
        m_Jeff_Shoot = m_Jeff.FindAction("Shoot", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // Jeff
    private readonly InputActionMap m_Jeff;
    private IJeffActions m_JeffActionsCallbackInterface;
    private readonly InputAction m_Jeff_Movement;
    private readonly InputAction m_Jeff_Rotation;
    private readonly InputAction m_Jeff_Shoot;
    public struct JeffActions
    {
        private @InputMaster m_Wrapper;
        public JeffActions(@InputMaster wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_Jeff_Movement;
        public InputAction @Rotation => m_Wrapper.m_Jeff_Rotation;
        public InputAction @Shoot => m_Wrapper.m_Jeff_Shoot;
        public InputActionMap Get() { return m_Wrapper.m_Jeff; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(JeffActions set) { return set.Get(); }
        public void SetCallbacks(IJeffActions instance)
        {
            if (m_Wrapper.m_JeffActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_JeffActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_JeffActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_JeffActionsCallbackInterface.OnMovement;
                @Rotation.started -= m_Wrapper.m_JeffActionsCallbackInterface.OnRotation;
                @Rotation.performed -= m_Wrapper.m_JeffActionsCallbackInterface.OnRotation;
                @Rotation.canceled -= m_Wrapper.m_JeffActionsCallbackInterface.OnRotation;
                @Shoot.started -= m_Wrapper.m_JeffActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_JeffActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_JeffActionsCallbackInterface.OnShoot;
            }
            m_Wrapper.m_JeffActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Rotation.started += instance.OnRotation;
                @Rotation.performed += instance.OnRotation;
                @Rotation.canceled += instance.OnRotation;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
            }
        }
    }
    public JeffActions @Jeff => new JeffActions(this);
    public interface IJeffActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnRotation(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
    }
}
